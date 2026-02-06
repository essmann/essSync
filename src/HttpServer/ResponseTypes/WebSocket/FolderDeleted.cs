using essSync.src.Database;

public class FolderDeleteMessage
{
    public string Type = "delete";
    public string FolderName { get; set; }
    public string LocalPath { get; set; }
    public string FolderGuid { get; set; }


    public FolderDeleteMessage(SharedFolder sf)
    {
        Type = "delete";
        FolderName = sf.FolderName;
        LocalPath = sf.LocalPath;
        FolderGuid = sf.FolderGuid;

    }
}
