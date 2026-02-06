export interface SharedFolder {
    FolderName: string;
    LocalPath: string;
    FolderGuid: string | null;
    IsPaused: boolean;
    Size: number;
    Permissions?: string;
    NumFiles: number;
    NumSubFolders: number;
    CreatedAt: string | null;
    LastSyncedAt: string | null;
}