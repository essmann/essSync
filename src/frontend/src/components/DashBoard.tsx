// DashBoard.tsx
import { useState } from 'react';
import { Folder, Wifi, HardDrive, Upload, Download, Clock, Hash, Tag } from 'lucide-react';
import AddFolderModal from './AddFolderModal';
import AddDeviceModal from './AddDeviceModal';
import { addDevice } from '../api/addDevice';

interface FolderType {
  name: string;
  status: string;
  files: number;
  size: string;
  id: string;
  path: string;
}

interface DeviceType {
  name: string;
  status: string;
  id: string;
  addresses?: string[]; // <-- added addresses array
}

const DashBoard = () => {
  const [folders] = useState<FolderType[]>([
    { name: 'Documents', status: 'Up to Date', files: 1247, size: '4.2 GB', id: 'doc-123', path: 'C:\\Users\\Documents' },
    { name: 'Photos', status: 'Syncing', files: 3891, size: '12.8 GB', id: 'pho-456', path: 'C:\\Users\\Photos' }
  ]);

  const [devices, setDevices] = useState<DeviceType[]>([
    { name: 'Desktop-PC', status: 'Connected', id: 'A8F3C9E2' },
    { name: 'Laptop', status: 'Up to Date', id: '9B2D4F7A' }
  ]);

  const [showAddFolder, setShowAddFolder] = useState(false);
  const [showAddDevice, setShowAddDevice] = useState(false);

  // Handler to refresh folders (placeholder)
  const handleAddFolder = () => {
    console.log("Folder added successfully, refresh list here...");
  };

  // Add Device handler - let modal handle errors
  // Add Device handler - let modal handle errors
  const handleAddDevice = async (device: DeviceType) => {
    console.log("Adding device via API:", device);

    // Call API, now including addresses
    await addDevice({
      DeviceGuid: device.id,
      DeviceName: device.name,
      DeviceIps: device.addresses || [] // pass array
    });

    // Update local state
    setDevices((prev) => [...prev, device]);
  };

  return (
    <div className="min-h-screen bg-gray-900 text-gray-200 p-6">
      {/* Header */}
      <div className="flex items-center justify-between mb-8 pb-4 border-b border-gray-700">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
            <Wifi className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="text-2xl font-semibold text-white">essSync</h1>
            <p className="text-sm text-gray-400">Device: WORK_5120</p>
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
            <div className="divide-y divide-gray-700">
              {folders.map((folder, idx) => (
                <div key={idx} className="px-5 py-4 hover:bg-gray-750 transition-colors">
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center gap-3">
                      <Folder className="w-5 h-5 text-gray-400" />
                      <span className="font-medium text-white">{folder.name}</span>
                    </div>
                    <span className={`text-sm px-3 py-1 rounded ${folder.status === 'Up to Date'
                      ? 'text-green-400 bg-green-400/10'
                      : folder.status === 'Syncing'
                        ? 'text-yellow-400 bg-yellow-400/10'
                        : 'text-gray-400 bg-gray-400/10'
                      }`}>
                      {folder.status}
                    </span>
                  </div>
                  <div className="flex gap-6 text-sm text-gray-400">
                    <span>{folder.files} files</span>
                    <span>{folder.size}</span>
                  </div>
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
                <span className="text-blue-400 text-xs">E7A9B3F2</span>
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
    </div>
  );
};

export default DashBoard;
