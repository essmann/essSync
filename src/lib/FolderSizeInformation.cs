public class FolderSizeInformation
{
    public long size;
    public int numFiles;
    public int numFolders;

    public FolderSizeInformation(long size, int numFiles, int numFolders)
    {
        this.size = size;
        this.numFiles = numFiles;
        this.numFolders = numFolders;
    }
}