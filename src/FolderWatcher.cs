using System;
using System.Collections.Generic;
using System.IO;
using essSync.src.Database;

public class FolderWatcher
{
    private readonly List<string> folderPaths = new();
    private DbApi _dbApi;
    private SharedFolderApi _folderApi;
    private readonly Dictionary<string, FileSystemWatcher> fileWatchers =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Lock _contextLock = new();
    /* ---------------- Public API ---------------- */

    public void SetDbApi(DbApi dbApi, SharedFolderApi folderApi)
    {
        _dbApi = dbApi;
        _folderApi = folderApi;
    }

    public void SetFolderPaths(IEnumerable<string> paths)
    {
        foreach (var path in paths)
            AddFolderToWatch(path);
    }

    public void PrintPaths()
    {
        string paths = "";
        foreach (string path in folderPaths)
        {
            paths += path + "\n";
        }
        Console.WriteLine(paths);
    }
    public void AddFolderToWatch(string folderPath)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return;

        if (!Directory.Exists(folderPath))
            return;

        if (fileWatchers.ContainsKey(folderPath))
            return;

        var watcher = CreateWatcher(folderPath);
        fileWatchers.Add(folderPath, watcher);
        folderPaths.Add(folderPath);

        Console.WriteLine($"Started watching: {folderPath}");
    }

    public void RemoveFolderToWatch(string folderPath)
    {
        if (!fileWatchers.TryGetValue(folderPath, out var watcher))
            return;

        watcher.EnableRaisingEvents = false;
        watcher.Dispose();
        fileWatchers.Remove(folderPath);
        folderPaths.Remove(folderPath);

        Console.WriteLine($"Stopped watching: {folderPath}");
    }

    public void StopAll()
    {
        foreach (var watcher in fileWatchers.Values)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        fileWatchers.Clear();
        folderPaths.Clear();
    }

    /* ---------------- Watcher setup ---------------- */

    private FileSystemWatcher CreateWatcher(string folderPath)
    {
        var watcher = new FileSystemWatcher(folderPath)
        {
            IncludeSubdirectories = true,
            NotifyFilter =
                NotifyFilters.LastWrite |
                NotifyFilters.FileName |
                NotifyFilters.DirectoryName
        };

        watcher.Changed += OnChanged;
        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;

        watcher.EnableRaisingEvents = true;
        return watcher;
    }

    /* ---------------- Event handlers ---------------- */

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        lock (_contextLock)
        {
            Console.WriteLine($"[{e.ChangeType}] {e.FullPath}");

            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    Console.WriteLine("Watcher detected a delete.");
                    _folderApi.DeleteSharedFolder(e.FullPath, WatcherChangeTypes.Deleted);
                    break;

            }
        }
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        lock (_contextLock)
        {
            Console.WriteLine($"[Renamed] {e.OldFullPath} -> {e.FullPath}");

            if (_dbApi == null)
                return;

            try
            {
                // Handle rename in DB
                var folder = _dbApi.GetAllSharedFoldersWithoutContents()
                                   .FirstOrDefault(f => f.LocalPath == Path.GetDirectoryName(e.OldFullPath));

                if (folder == null)
                    return;

                folder.LocalPath = Path.GetDirectoryName(e.FullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating renamed folder in DB: {ex.Message}");
            }
        }
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine("FileSystemWatcher error:");

        if (e.GetException() is InternalBufferOverflowException ex)
            Console.WriteLine("Buffer overflow: " + ex.Message);
        else
            Console.WriteLine(e.GetException().Message);
    }
}