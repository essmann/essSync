export interface SharedFolder {
    FolderName: string;
    LocalPath: string;
    FolderGuid: string | null;
    IsPaused: boolean;
    Size: number;
    CreatedAt: string;
    LastSyncedAt: string | null;
}