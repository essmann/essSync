const url = import.meta.env.VITE_API_URL;
import type { SharedFolder } from "./types/sharedFolder";

export async function getFolders(): Promise<SharedFolder[]> {
    console.log('[API] Fetching folders...');

    try {
        const response = await fetch(url + '/getFolders', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('[API] Failed to fetch folders:', {
                status: response.status,
                statusText: response.statusText,
                error: errorText
            });
            throw new Error(`Failed to fetch folders: ${response.status} ${response.statusText}`);
        }

        const folders: SharedFolder[] = await response.json();
        console.log('[API] Folders fetched:', folders);
        return folders;
    } catch (error) {
        console.error('[API] Error fetching folders:', error);
        throw error;
    }
}
