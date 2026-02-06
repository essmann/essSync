//Message types
//Message types
interface FolderUpdateMessage {
    type: 'folder',
    test: string;

}
interface DeviceUpdateMessage {
    type: 'device',
    test: string;

}

interface FolderDeleteMessage {
    type: "delete";
    FolderName: string;
    LocalPath: string;
    FolderGuid: string;
}

type WebSocketMessage = FolderUpdateMessage | DeviceUpdateMessage | FolderDeleteMessage;

export interface WebSocketHandlerCallbacks {
    onFolderUpdate: (folder: FolderUpdateMessage) => void;
    onFolderDelete: (folder: FolderDeleteMessage) => void;
}
export class WebsocketHandler {


    private _socket: WebSocket;
    private _callbacks: WebSocketHandlerCallbacks;


    constructor(socket: WebSocket, callbacks: WebSocketHandlerCallbacks) {
        this._socket = socket;
        this._callbacks = callbacks;

        this._socket.addEventListener("open", () => {
            console.log("WebSocket connected");
        });
        this._socket.addEventListener("error", (error) => {
            console.error("WebSocket error:", error);
        });
        this._socket.addEventListener("message", (event) => {
            console.log("Message from server:", event.data);

            try {
                const data: WebSocketMessage = JSON.parse(event.data);

                switch (data.type) {

                    case 'delete':
                        this._callbacks.onFolderDelete(data);    // data is FolderDeleteMessage
                        break;
                }
            } catch (error) {
                console.error("Failed to parse message:", error);
            }



        });

        this._socket.addEventListener("close", () => {
            console.log("WebSocket disconnected");
        });

    }


}