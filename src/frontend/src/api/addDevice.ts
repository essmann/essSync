const url = import.meta.env.VITE_API_URL;
import type { DeviceRequest } from "./types/device";

// Sends a request to add a device to the API
export async function addDevice(device: DeviceRequest): Promise<void> {
    console.log('[API] Adding device:', {
        DeviceName: device.DeviceName,
        DeviceGuid: device.DeviceGuid,
        DeviceIps: device.DeviceIps || [] // log addresses array
    });

    try {
        const response = await fetch(`${url}/addDevice`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(device)
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('[API] Failed to add device:', {
                status: response.status,
                statusText: response.statusText,
                error: errorText
            });
            throw new Error(`Failed to add device: ${response.status} ${response.statusText} - ${errorText}`);
        }

        console.log('[API] Successfully added device:', device.DeviceName);
    } catch (error) {
        console.error('[API] Error adding device:', error);
        throw error;
    }
}
