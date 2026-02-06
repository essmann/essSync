// testData.ts
import { type SharedFolder } from '../api/types/sharedFolder';

export const testSharedFolders: SharedFolder[] = [
    {
        FolderName: 'Documents',
        LocalPath: '/home/user/Documents',
        FolderGuid: 'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
        IsPaused: false,
        Size: 2147483648, // 2 GB
        Permissions: 'Read/Write',
        NumFiles: 1247,
        NumSubFolders: 38,
        CreatedAt: '2024-01-15T10:30:00Z',
        LastSyncedAt: '2026-02-05T14:22:15Z'
    },
    {
        FolderName: 'Photos',
        LocalPath: '/home/user/Pictures/Photos',
        FolderGuid: 'b2c3d4e5-f6a7-8901-bcde-f12345678901',
        IsPaused: false,
        Size: 15728640000, // 14.6 GB
        Permissions: 'Read/Write',
        NumFiles: 3892,
        NumSubFolders: 127,
        CreatedAt: '2023-11-20T08:15:30Z',
        LastSyncedAt: '2026-02-05T13:45:22Z'
    },
    {
        FolderName: 'Projects',
        LocalPath: '/home/user/Work/Projects',
        FolderGuid: 'c3d4e5f6-a7b8-9012-cdef-123456789012',
        IsPaused: true,
        Size: 5368709120, // 5 GB
        Permissions: 'Read/Write',
        NumFiles: 2156,
        NumSubFolders: 64,
        CreatedAt: '2024-03-10T16:45:00Z',
        LastSyncedAt: '2026-02-04T09:18:33Z'
    },
    {
        FolderName: 'Music',
        LocalPath: '/home/user/Music',
        FolderGuid: 'd4e5f6a7-b8c9-0123-def1-234567890123',
        IsPaused: false,
        Size: 8589934592, // 8 GB
        Permissions: 'Read Only',
        NumFiles: 1876,
        NumSubFolders: 42,
        CreatedAt: '2023-09-05T12:00:00Z',
        LastSyncedAt: '2026-02-05T11:30:44Z'
    },
    {
        FolderName: 'Videos',
        LocalPath: '/media/external/Videos',
        FolderGuid: 'e5f6a7b8-c9d0-1234-ef12-345678901234',
        IsPaused: false,
        Size: 53687091200, // 50 GB
        Permissions: 'Read/Write',
        NumFiles: 423,
        NumSubFolders: 18,
        CreatedAt: '2024-06-22T14:20:10Z',
        LastSyncedAt: '2026-02-05T15:05:12Z'
    },
    {
        FolderName: 'Backup',
        LocalPath: '/backup/system',
        FolderGuid: null,
        IsPaused: true,
        Size: 1073741824, // 1 GB
        Permissions: 'Read/Write',
        NumFiles: 567,
        NumSubFolders: 12,
        CreatedAt: null,
        LastSyncedAt: null
    },
    {
        FolderName: 'Code',
        LocalPath: '/home/user/Development/Code',
        FolderGuid: 'f6a7b8c9-d0e1-2345-f123-456789012345',
        IsPaused: false,
        Size: 3221225472, // 3 GB
        Permissions: 'Read/Write',
        NumFiles: 8934,
        NumSubFolders: 234,
        CreatedAt: '2023-12-01T09:00:00Z',
        LastSyncedAt: '2026-02-05T15:30:00Z'
    },
    {
        FolderName: 'Downloads',
        LocalPath: '/home/user/Downloads',
        FolderGuid: 'a7b8c9d0-e1f2-3456-1234-567890123456',
        IsPaused: false,
        Size: 4294967296, // 4 GB
        Permissions: 'Read/Write',
        NumFiles: 892,
        NumSubFolders: 15,
        CreatedAt: '2024-02-14T11:30:00Z',
        LastSyncedAt: '2026-02-05T14:55:18Z'
    }
];