using System;
using System.Collections.Generic;
using System.IO;

public static class FolderWatcher
{
    private static readonly List<string> folderPaths = new();

    private static readonly Dictionary<string, FileSystemWatcher> fileWatchers =
        new(StringComparer.OrdinalIgnoreCase);

    /* ---------------- Public API ---------------- */

    public static void SetFolderPaths(IEnumerable<string> paths)
    {
        foreach (var path in paths)
            AddFolderToWatch(path);
    }

    public static void AddFolderToWatch(string folderPath)
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

    public static void RemoveFolderToWatch(string folderPath)
    {
        if (!fileWatchers.TryGetValue(folderPath, out var watcher))
            return;

        watcher.EnableRaisingEvents = false;
        watcher.Dispose();

        fileWatchers.Remove(folderPath);
        folderPaths.Remove(folderPath);

        Console.WriteLine($"Stopped watching: {folderPath}");
    }

    public static void StopAll()
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

    private static FileSystemWatcher CreateWatcher(string folderPath)
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

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine($"[{e.ChangeType}] {e.FullPath}");
    }

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"[Renamed] {e.OldFullPath} -> {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine("FileSystemWatcher error:");

        if (e.GetException() is InternalBufferOverflowException ex)
            Console.WriteLine("Buffer overflow: " + ex.Message);
        else
            Console.WriteLine(e.GetException().Message);
    }
}
