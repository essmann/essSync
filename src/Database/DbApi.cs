using essSync.src.Database;
using Microsoft.EntityFrameworkCore;

public class DbApi
{
    private SharedContext context;

    public DbApi(DbContext context)
    {
        this.context = (SharedContext)context;
    }

    public List<SharedFolder> GetAllSharedFolders()
    {
        Log("INFO", "Retrieving all shared folders from database");
        try
        {
            var folders = context.SharedFolders.Include(f => f.Files).ToList();
            Log("INFO", $"Successfully retrieved {folders.Count} shared folders");
            return folders;
        }
        catch (Exception ex)
        {
            Log("ERROR", $"Error retrieving shared folders: {ex.Message}");
            throw;
        }
    }

    public SharedFolder? GetSharedFolderByGuid(string folderGuid)
    {
        Log("INFO", $"Retrieving shared folder with GUID: {folderGuid}");
        try
        {
            var folder = context.SharedFolders
                .Include(f => f.Files)
                .FirstOrDefault(f => f.FolderGuid == folderGuid);

            if (folder != null)
            {
                Log("INFO", $"Found shared folder: {folder.FolderName} (GUID: {folderGuid})");
            }
            else
            {
                Log("WARN", $"No shared folder found with GUID: {folderGuid}");
            }

            return folder;
        }
        catch (Exception ex)
        {
            Log("ERROR", $"Error retrieving shared folder with GUID {folderGuid}: {ex.Message}");
            throw;
        }
    }

    public void AddSharedFolder(SharedFolder folder)
    {
        Log("INFO", $"Attempting to add shared folder: {folder.FolderName}, Path: {folder.LocalPath}, GUID: {folder.FolderGuid}");

        if (folder.FolderGuid == null)
        {
            folder.FolderGuid = Guid.NewGuid().ToString();
            Log("INFO", $"Generated new GUID for folder: {folder.FolderGuid}");
        }
        try
        {
            // Check if folder path already exists
            if (context.SharedFolders.Any(f => f.LocalPath == folder.LocalPath))
            {
                Log("WARN", $"Folder path already exists in database: {folder.LocalPath}");
                return;
            }

            context.SharedFolders.Add(folder);
            context.SaveChanges();

            Log("INFO", $"Successfully added shared folder to database: {folder.FolderName} (ID: {folder.SharedFolderId})");

            FolderWatcher.AddFolderToWatch(folder.LocalPath);
            Log("INFO", $"Added folder to watcher: {folder.LocalPath}");
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.InnerException?.Message);

            Log("ERROR", $"Error adding shared folder '{folder.FolderName}': {ex.Message}");
            throw;
        }
    }

    public void UpdateSharedFolder(SharedFolder folder)
    {
        Log("INFO", $"Updating shared folder: {folder.FolderName} (ID: {folder.SharedFolderId}, GUID: {folder.FolderGuid})");

        try
        {
            context.SharedFolders.Update(folder);
            context.SaveChanges();

            Log("INFO", $"Successfully updated shared folder: {folder.FolderName} (ID: {folder.SharedFolderId})");
        }
        catch (Exception ex)
        {
            Log("ERROR", $"Error updating shared folder '{folder.FolderName}' (ID: {folder.SharedFolderId}): {ex.Message}");
            throw;
        }
    }

    public void DeleteSharedFolder(SharedFolder folder)
    {
        Log("INFO", $"Deleting shared folder: {folder.FolderName} (ID: {folder.SharedFolderId}, Path: {folder.LocalPath})");

        try
        {
            context.SharedFolders.Remove(folder);
            context.SaveChanges();

            Log("INFO", $"Successfully deleted shared folder from database: {folder.FolderName} (ID: {folder.SharedFolderId})");

            FolderWatcher.RemoveFolderToWatch(folder.LocalPath);
            Log("INFO", $"Removed folder from watcher: {folder.LocalPath}");
        }
        catch (Exception ex)
        {
            Log("ERROR", $"Error deleting shared folder '{folder.FolderName}' (ID: {folder.SharedFolderId}): {ex.Message}");
            throw;
        }
    }

    private void Log(string level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"[{timestamp}] [{level}] [DbApi] {message}");
    }
}