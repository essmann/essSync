import { type HostNameInformation } from "./types/HostNameInformation";
const url = import.meta.env.VITE_API_URL;

// Sends a request to get the current device info
export async function getHostNameAndId(): Promise<HostNameInformation> {
    console.log('[API] Fetching /me');

    try {
        const response = await fetch(`${url}/me`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('[API] Failed to get /me:', {
                status: response.status,
                statusText: response.statusText,
                error: errorText
            });
            throw new Error(`Failed to fetch /me: ${response.status} ${response.statusText} - ${errorText}`);
        }

        const data = await response.json();
        console.log('[API] Successfully fetched /me', data);
        return data; // Now properly typed
    } catch (error) {
        console.error('[API] Error fetching /me:', error);
        throw error;
    }
}
