const url = import.meta.env.VITE_API_URL;
import type { SharedFolder } from "./types/sharedFolder";

export async function addFolder(folder: SharedFolder): Promise<void> {
    console.log('[API] Adding folder:', {
        folderName: folder.FolderName,
        localPath: folder.LocalPath,
        folderGuid: folder.FolderGuid
    });

    try {
        const response = await fetch(url + '/addFolder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(folder)
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('[API] Failed to add folder:', {
                status: response.status,
                statusText: response.statusText,
                error: errorText
            });
            throw new Error(`Failed to add folder: ${response.status} ${response.statusText}`);
        }

        console.log('[API] Successfully added folder:', folder.FolderName);
    } catch (error) {
        console.error('[API] Error adding folder:', error);
        throw error;
    }
}