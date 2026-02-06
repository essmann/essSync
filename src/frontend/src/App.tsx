import './App.css'
import './index.css'
import DashBoard from './components/DashBoard'
import { useState, useEffect } from 'react';
import { getHostNameAndId } from './api/getHostNameAndId';
import { getFolders } from './api/getFolderMetadata';
import { type SharedFolder } from './api/types/sharedFolder';
import { testSharedFolders } from './test/testFolders';
import { WebsocketHandler } from './Websocket/WebSocketHandler';
import { type WebSocketHandlerCallbacks } from './Websocket/WebSocketHandler';
// import { type SharedFolder } from './api/types/sharedFolder';
function App() {
  const [deviceId, setDeviceId] = useState("");
  const [hostName, setHostName] = useState("");
  const [sharedFolders, setSharedFolders] = useState<SharedFolder[]>(testSharedFolders);


  let callbacks: WebSocketHandlerCallbacks = {
    onFolderDelete(folder) {

      setSharedFolders((prev) => [...prev].filter((f) => f.LocalPath !== folder.LocalPath));

    },
    onFolderUpdate(folder) {
      console.log(folder);
    },
  }

  useEffect(() => {
    // Create WebSocket connection
    const socket = new WebSocket("ws://localhost:8080");

    new WebsocketHandler(socket, callbacks);



    // Cleanup on unmount
    return () => {
      socket.close();
    };
  }, []); // Empty dependency → runs once

  useEffect(() => {
    (async () => {
      try {
        let response = await getHostNameAndId();
        console.log(response);

        // Assuming response has deviceId
        if (response?.DeviceGuid) {
          setDeviceId(response.DeviceGuid);
          setHostName(response.HostName);
        }
      } catch (err) {
        console.error("Failed to fetch device info:", err);
      }
    })();
  }, []);


  useEffect(() => {
    (async () => {
      try {
        const folders = await getFolders();
        setSharedFolders(folders);
        debugger;


      }
      catch (err) {
        console.error("failed to fetch shared folders:", err);
      }
    })();
  }, []);



  return (
    <>
      <DashBoard host={hostName} deviceId={deviceId} sharedFolders={sharedFolders} setSharedFolders={setSharedFolders} />
    </>
  );
}

export default App;
