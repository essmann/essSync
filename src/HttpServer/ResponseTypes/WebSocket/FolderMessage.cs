using essSync.src.Database;

public class FolderMessage
{
    public string FolderName { get; set; }
    public string LocalPath { get; set; }
    public string FolderGuid { get; set; }
    public long Size { get; set; }
    public string? Permissions { get; set; }

    public int NumFiles { get; set; }
    public int NumSubFolders { get; set; }


    public FolderMessage(SharedFolder sf)
    {
        FolderName = sf.FolderName;
        LocalPath = sf.LocalPath;
        FolderGuid = sf.FolderGuid;
        Size = sf.Size;
        Permissions = sf.Permissions;
        NumFiles = sf.NumFiles;
        NumSubFolders = sf.NumSubFolders;
    }
}
