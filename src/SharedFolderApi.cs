

using essSync.src.Database;

public class SharedFolderApi
{

    private DbApi _dbApi;
    private FolderWatcher _watcher;
    public SharedFolderApi(DbApi dbApi, FolderWatcher watcher)
    {
        this._dbApi = dbApi;
        this._watcher = watcher;
    }

    // --- Minimal observer events ---
    public event Action<SharedFolder>? FolderAdded;
    public event Action<SharedFolder>? FolderUpdated;
    public event Action<SharedFolder>? FolderDeleted;
    public void Init()
    {
        //1) Get all folders from database
        //2) Add these folders to the watcher
        var folders = _dbApi.GetAllSharedFoldersWithoutContents();
        List<string> folderPaths = folders.Select(folder => folder.LocalPath).ToList();
        _watcher.SetFolderPaths(folderPaths);

    }
    public void AddSharedFolder(string absolutePath)
    {   //=== METADATA
        //1) Add to the FolderWatcher
        //2) On success, retrieve information about this folder from the system.
        //3) Construct a SharedFolder object with all the information
        //4) Relay this to the database with all the necessary information.
        //5) Invoke the FolderAdded event

        //=== FILE CONTENTS


        try
        {
            // Add folder to watcher
            _watcher.AddFolderToWatch(absolutePath);

            // Get folder info
            FolderSizeInformation folderInformation = GetFolderInformation(absolutePath);

            // Get folder name from path
            string folderName = Path.GetFileName(absolutePath.TrimEnd(Path.DirectorySeparatorChar));

            // Create shared folder object
            SharedFolder sharedFolder = new SharedFolder
            {
                FolderName = folderName,
                LocalPath = absolutePath,
                Size = folderInformation.size,
                NumFiles = folderInformation.numFiles,
                NumSubFolders = folderInformation.numFolders
            };

            // Add to database
            SharedFolder folder = _dbApi.AddSharedFolder(sharedFolder);
            FolderAdded?.Invoke(folder);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding folder '{absolutePath}': {e}");
        }

    }



    public void DeleteSharedFolder(string absolutePath, WatcherChangeTypes watcherState)
    {
        try
        {
            // Skip disk existence check since folder is already gone (called from watcher)

            // Remove folder from watcher
            _watcher.RemoveFolderToWatch(absolutePath);

            // Remove folder from database
            SharedFolder? folder = _dbApi.GetAllSharedFoldersWithoutContents()
                                         .Find(f => f.LocalPath == absolutePath);
            if (folder != null)
            {
                _dbApi.DeleteSharedFolder(folder);
                Console.WriteLine($"Folder '{absolutePath}' successfully deleted (WatcherState: {watcherState}).");
                FolderDeleted?.Invoke(folder);
            }
            else
            {
                Console.WriteLine($"Folder '{absolutePath}' is not registered in the database.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting folder '{absolutePath}': {e}");
        }
    }
    public void DeleteSharedFolder(string absolutePath)
    {
        try
        {
            // Check if folder exists on disk
            if (!Directory.Exists(absolutePath))
            {
                Console.WriteLine($"Folder '{absolutePath}' does not exist.");
                return;
            }

            // Remove folder from watcher
            _watcher.RemoveFolderToWatch(absolutePath);

            // Remove folder from database
            SharedFolder? folder = _dbApi.GetAllSharedFoldersWithoutContents()
                                         .Find(f => f.LocalPath == absolutePath);
            if (folder != null)
            {
                _dbApi.DeleteSharedFolder(folder);
                Console.WriteLine($"Folder '{absolutePath}' successfully deleted.");

                FolderDeleted?.Invoke(folder);
            }
            else
            {
                Console.WriteLine($"Folder '{absolutePath}' is not registered in the database.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting folder '{absolutePath}': {e}");
        }
    }




    public void Rescan()
    {
        //1) Go through every folder, subfolder and file.
        //2) Compare their state to the database state, update accordingly.

    }
    private FolderSizeInformation GetFolderInformation(string absolutePath)
    {
        int fileCount = Directory.GetFiles(absolutePath).Length;
        int folderCount = Directory.GetDirectories(absolutePath).Length;
        string[] files = Directory.GetFiles(absolutePath);
        long totalSize = files.Sum(f => new FileInfo(f).Length);

        return new FolderSizeInformation(totalSize, fileCount, folderCount);


    }

}