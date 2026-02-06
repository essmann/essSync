// DashBoard.tsx
import { useState, type Context } from 'react';
import { Folder, Wifi, HardDrive, Upload, Download, Clock, Hash, Tag, File, FolderOpen, Database, Calendar, Pause, Play } from 'lucide-react';
import AddFolderModal from './AddFolderModal';
import AddDeviceModal from './AddDeviceModal';
import { addDevice } from '../api/addDevice';
import { type SharedFolder } from '../api/types/sharedFolder';
import { useRef } from 'react';
import { useClickOutside } from '../hooks/useClickOutside';
interface DeviceType {
  name: string;
  status: string;
  id: string;
  addresses?: string[];
}

interface DashBoardProps {
  host: string;
  deviceId: string;
  sharedFolders: SharedFolder[] | [];
  setSharedFolders: React.Dispatch<React.SetStateAction<SharedFolder[]>>
}

interface ContextMenuPosition {
  x: number;
  y: number;
}
const DashBoard = ({ deviceId, host, sharedFolders, setSharedFolders }: DashBoardProps) => {
  const [devices, setDevices] = useState<DeviceType[]>([
    { name: 'Desktop-PC', status: 'Connected', id: 'A8F3C9E2' },
    { name: 'Laptop', status: 'Up to Date', id: '9B2D4F7A' }
  ]);


  const [showAddFolder, setShowAddFolder] = useState(false);
  const [showAddDevice, setShowAddDevice] = useState(false);
  const [contextPos, setContextPos] = useState<ContextMenuPosition | null>(null);
  const contextRef = useRef<HTMLDivElement>(null);
  const handleCLickOutside = () => {
    setContextPos(null);
    console.log("You clicked outside");
  }
  useClickOutside(contextRef, handleCLickOutside);

  const handleRightClick = (event: React.MouseEvent<HTMLDivElement, MouseEvent>, folder: SharedFolder) => {
    event.preventDefault();
    setContextPos({ x: event.clientX, y: event.clientY });
    console.log("Right-click folder:", folder);
  };

  // Handler to refresh folders (placeholder)
  const handleAddFolder = (folder: SharedFolder): void => {
    console.log("Folder added successfully, refresh list here...");
    setSharedFolders([...sharedFolders, folder]);

  };

  // Add Device handler
  const handleAddDevice = async (device: DeviceType) => {
    console.log("Adding device via API:", device);

    await addDevice({
      DeviceGuid: device.id,
      DeviceName: device.name,
      DeviceIps: device.addresses || []
    });

    setDevices((prev) => [...prev, device]);
  };

  // Format size helper
  const formatSize = (bytes: number): string => {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return `${(bytes / Math.pow(k, i)).toFixed(1)} ${sizes[i]}`;
  };

  // Format date helper
  const formatDate = (dateString: string | null): string => {
    if (!dateString) return 'Never';
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  };

  return (
    <div className="min-h-screen bg-gray-900 text-gray-200 p-6 relative" >
      {/* Header */}
      <div className="flex items-center justify-between mb-8 pb-4 border-b border-gray-700">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
            <Wifi className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="text-2xl font-semibold text-white">essSync</h1>
            <p className="text-sm text-gray-400">Device: {host}</p>
          </div>
        </div>
        <div className="flex gap-3">
          <button className="px-4 py-2 bg-gray-800 hover:bg-gray-700 rounded text-sm transition-colors">
            Settings
          </button>
          <button

            onClick={() => setShowAddFolder(true)}
            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 rounded text-sm transition-colors"
          >
            Add Folder
          </button>
        </div>
      </div>

      {/* Main Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Folders Section */}
        <div className="lg:col-span-2">
          <div className="bg-gray-800 rounded-lg overflow-hidden">
            <div className="px-5 py-4 border-b border-gray-700 flex items-center justify-between">
              <h2 className="text-lg font-medium text-white">Folders</h2>
              <div className="flex gap-2">
                <button className="px-3 py-1 text-xs bg-gray-700 hover:bg-gray-600 rounded transition-colors">
                  Pause All
                </button>
                <button className="px-3 py-1 text-xs bg-gray-700 hover:bg-gray-600 rounded transition-colors">
                  Rescan All
                </button>
              </div>
            </div>
            <div className="p-2 space-y-2">
              {sharedFolders.map((folder, idx) => (
                <div
                  onContextMenu={(e) => {
                    handleRightClick(e, folder);
                  }}
                  key={idx}
                  className="px-4 py-3 rounded-lg bg-gray-750 hover:bg-gray-700 hover:shadow-lg hover:shadow-blue-500/10 transition-all duration-200 cursor-pointer border border-transparent hover:border-gray-600"
                >
                  <div className="flex items-center justify-between mb-2">
                    <div className="flex items-center gap-3">
                      <Folder className="w-4 h-4 text-blue-400" />
                      <div>
                        <span className="font-medium text-white text-sm block">{folder.FolderName}</span>
                        <span className="text-xs text-gray-500">{folder.LocalPath}</span>
                      </div>
                    </div>
                    <div className="flex items-center gap-2">
                      {folder.IsPaused ? (
                        <span className="flex items-center gap-1 text-xs text-yellow-400">
                          <Pause className="w-3 h-3" />
                          Paused
                        </span>
                      ) : (
                        <span className="flex items-center gap-1 text-xs text-green-400">
                          <Play className="w-3 h-3" />
                          Active
                        </span>
                      )}
                    </div>
                  </div>

                  <div className="grid grid-cols-2 md:grid-cols-4 gap-3 text-xs text-gray-400">
                    <div className="flex items-center gap-1.5">
                      <Database className="w-3.5 h-3.5 text-gray-500" />
                      <span>{formatSize(folder.Size)}</span>
                    </div>
                    <div className="flex items-center gap-1.5">
                      <File className="w-3.5 h-3.5 text-gray-500" />
                      <span>{folder.NumFiles} files</span>
                    </div>
                    <div className="flex items-center gap-1.5">
                      <FolderOpen className="w-3.5 h-3.5 text-gray-500" />
                      <span>{folder.NumSubFolders} folders</span>
                    </div>
                    {folder.Permissions && (
                      <div className="flex items-center gap-1.5">
                        <Tag className="w-3.5 h-3.5 text-gray-500" />
                        <span>{folder.Permissions}</span>
                      </div>
                    )}
                  </div>

                  <div className="mt-2 grid grid-cols-1 md:grid-cols-2 gap-2 text-xs text-gray-500">
                    {folder.CreatedAt && (
                      <div className="flex items-center gap-1.5">
                        <Calendar className="w-3 h-3" />
                        <span>Created: {formatDate(folder.CreatedAt)}</span>
                      </div>
                    )}
                    {folder.LastSyncedAt && (
                      <div className="flex items-center gap-1.5">
                        <Clock className="w-3 h-3" />
                        <span>Last synced: {formatDate(folder.LastSyncedAt)}</span>
                      </div>
                    )}
                  </div>

                  {folder.FolderGuid && (
                    <div className="mt-2 text-xs text-gray-600">
                      ID: {folder.FolderGuid}
                    </div>
                  )}
                </div>
              ))}
            </div>
          </div>
        </div>

        {/* Devices Section */}
        <div>
          {/* This Device */}
          <div className="bg-gray-800 rounded-lg overflow-hidden mb-6">
            <div className="px-5 py-4 border-b border-gray-700">
              <h2 className="text-lg font-medium text-white">This Device</h2>
            </div>
            <div className="px-5 py-4 space-y-3">
              <div className="flex items-center justify-between text-sm">
                <div className="flex items-center gap-2 text-gray-400">
                  <Download className="w-4 h-4" />
                  <span>Download Rate</span>
                </div>
                <span className="text-white">0 B/s</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <div className="flex items-center gap-2 text-gray-400">
                  <Upload className="w-4 h-4" />
                  <span>Upload Rate</span>
                </div>
                <span className="text-white">0 B/s</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <div className="flex items-center gap-2 text-gray-400">
                  <HardDrive className="w-4 h-4" />
                  <span>Local State</span>
                </div>
                <span className="text-white">17.0 GB</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <div className="flex items-center gap-2 text-gray-400">
                  <Clock className="w-4 h-4" />
                  <span>Uptime</span>
                </div>
                <span className="text-white">2d 14h 33m</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <div className="flex items-center gap-2 text-gray-400">
                  <Hash className="w-4 h-4" />
                  <span>Device ID</span>
                </div>
                <span className="text-blue-400 text-xs">{deviceId}</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <div className="flex items-center gap-2 text-gray-400">
                  <Tag className="w-4 h-4" />
                  <span>Version</span>
                </div>
                <span className="text-gray-300 text-xs">v1.2.4</span>
              </div>
            </div>
          </div>

          {/* Remote Devices */}
          <div className="bg-gray-800 rounded-lg overflow-hidden">
            <div className="px-5 py-4 border-b border-gray-700 flex items-center justify-between">
              <h2 className="text-lg font-medium text-white">Remote Devices</h2>
              <button
                onClick={() => setShowAddDevice(true)}
                className="text-xs text-blue-400 hover:text-blue-300"
              >
                + Add Device
              </button>
            </div>
            <div className="divide-y divide-gray-700">
              {devices.map((device, idx) => (
                <div key={idx} className="px-5 py-3 hover:bg-gray-750 transition-colors">
                  <div className="flex items-center justify-between mb-1">
                    <span className="text-sm font-medium text-white">{device.name}</span>
                    <span className={`text-xs ${device.status === 'Connected' ? 'text-green-400' : 'text-gray-400'}`}>
                      {device.status}
                    </span>
                  </div>
                  <div className="text-xs text-gray-500">{device.id}</div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      <AddFolderModal
        isOpen={showAddFolder}
        onClose={() => setShowAddFolder(false)}
        onAddFolder={handleAddFolder}
      />

      <AddDeviceModal
        isOpen={showAddDevice}
        onClose={() => setShowAddDevice(false)}
        onAddDevice={handleAddDevice}
      />

      {contextPos !== null && (
        <div
          className='context_menu bg-gray-800 rounded-lg flex max-w-xs'
          ref={contextRef}
          style={{
            position: 'absolute',
            left: contextPos.x,
            top: contextPos.y,
          }}
        >
          <div>
            Header

          </div>
          <div> </div>
        </div>
      )}



    </div>
  );
};

export default DashBoard;