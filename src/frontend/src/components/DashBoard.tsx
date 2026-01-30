import { useState } from 'react';
import { Folder, Wifi, HardDrive, Upload, Download, Clock, Hash, Tag, X, FolderPlus, Monitor } from 'lucide-react';

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
}

// Add Folder Modal Component
const AddFolderModal = ({ isOpen, onClose, onAddFolder }: {
  isOpen: boolean;
  onClose: () => void;
  onAddFolder: (folder: FolderType) => void;
}) => {
  const [activeTab, setActiveTab] = useState('general');
  const [folderForm, setFolderForm] = useState({
    label: '',
    id: '',
    path: ''
  });

  const generateFolderId = () => {
    const chars = 'abcdefghijklmnopqrstuvwxyz0123456789';
    let id = '';
    for (let i = 0; i < 12; i++) {
      id += chars[Math.floor(Math.random() * chars.length)];
      if (i === 4 || i === 7) id += '-';
    }
    setFolderForm({ ...folderForm, id });
  };

  const handleSave = () => {
    if (folderForm.id && folderForm.path) {
      onAddFolder({
        name: folderForm.label || folderForm.id,
        status: 'Idle',
        files: 0,
        size: '0 B',
        id: folderForm.id,
        path: folderForm.path
      });
      setFolderForm({ label: '', id: '', path: '' });
      setActiveTab('general');
      onClose();
    }
  };

  const handleClose = () => {
    setFolderForm({ label: '', id: '', path: '' });
    setActiveTab('general');
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
      <div className="bg-gray-800 rounded-lg w-full max-w-2xl max-h-[90vh] overflow-hidden shadow-2xl">
        <div className="px-6 py-4 border-b border-gray-700 flex items-center justify-between bg-gray-750">
          <div className="flex items-center gap-2">
            <FolderPlus className="w-5 h-5 text-blue-400" />
            <h2 className="text-lg font-medium text-white">
              Add Folder {folderForm.id && `(${folderForm.id})`}
            </h2>
          </div>
          <button onClick={handleClose} className="text-gray-400 hover:text-white transition-colors">
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="px-6 pt-4 border-b border-gray-700 flex gap-6">
          {['general', 'sharing', 'file-versioning', 'ignore-patterns', 'advanced'].map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`pb-3 px-1 text-sm font-medium transition-colors border-b-2 ${activeTab === tab
                  ? 'text-blue-400 border-blue-400'
                  : 'text-gray-400 border-transparent hover:text-gray-300'
                }`}
            >
              {tab.split('-').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ')}
            </button>
          ))}
        </div>

        <div className="px-6 py-6 overflow-y-auto max-h-[calc(90vh-180px)]">
          {activeTab === 'general' && (
            <div className="space-y-6">
              <div>
                <label className="block text-sm font-medium text-white mb-2">Folder Label</label>
                <input
                  type="text"
                  value={folderForm.label}
                  onChange={(e) => setFolderForm({ ...folderForm, label: e.target.value })}
                  className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500"
                  placeholder="Optional descriptive label for the folder"
                />
                <p className="text-xs text-gray-400 mt-1">
                  Optional descriptive label for the folder. Can be different on each device.
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-white mb-2">Folder ID</label>
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={folderForm.id}
                    onChange={(e) => setFolderForm({ ...folderForm, id: e.target.value })}
                    className="flex-1 bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500 font-mono text-sm"
                    placeholder="oqfst-qgc5y"
                  />
                  <button
                    onClick={generateFolderId}
                    className="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-white rounded transition-colors text-sm"
                  >
                    Generate
                  </button>
                </div>
                <p className="text-xs text-gray-400 mt-1">
                  Required identifier for the folder. Must be the same on all cluster devices.
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-white mb-2">Folder Path</label>
                <input
                  type="text"
                  value={folderForm.path}
                  onChange={(e) => setFolderForm({ ...folderForm, path: e.target.value })}
                  className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500 font-mono text-sm"
                  placeholder="C:\Users\kenwi"
                />
                <p className="text-xs text-gray-400 mt-1">
                  Path to the folder on the local computer. Will be created if it does not exist.
                </p>
              </div>
            </div>
          )}

          {activeTab !== 'general' && (
            <div className="text-center py-12 text-gray-400">
              <p>{activeTab.split('-').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ')} options will be displayed here</p>
            </div>
          )}
        </div>

        <div className="px-6 py-4 border-t border-gray-700 flex justify-end gap-3 bg-gray-750">
          <button
            onClick={handleClose}
            className="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-white rounded transition-colors text-sm"
          >
            Close
          </button>
          <button
            onClick={handleSave}
            disabled={!folderForm.id || !folderForm.path}
            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded transition-colors text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Save
          </button>
        </div>
      </div>
    </div>
  );
};

// Add Device Modal Component
const AddDeviceModal = ({ isOpen, onClose, onAddDevice }: {
  isOpen: boolean;
  onClose: () => void;
  onAddDevice: (device: DeviceType) => void;
}) => {
  const [deviceForm, setDeviceForm] = useState({
    deviceId: '',
    name: '',
    address: ''
  });

  const handleSave = () => {
    if (deviceForm.deviceId) {
      onAddDevice({
        name: deviceForm.name || 'New Device',
        status: 'Disconnected',
        id: deviceForm.deviceId
      });
      setDeviceForm({ deviceId: '', name: '', address: '' });
      onClose();
    }
  };

  const handleClose = () => {
    setDeviceForm({ deviceId: '', name: '', address: '' });
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
      <div className="bg-gray-800 rounded-lg w-full max-w-xl shadow-2xl">
        <div className="px-6 py-4 border-b border-gray-700 flex items-center justify-between bg-gray-750">
          <div className="flex items-center gap-2">
            <Monitor className="w-5 h-5 text-blue-400" />
            <h2 className="text-lg font-medium text-white">Add Remote Device</h2>
          </div>
          <button onClick={handleClose} className="text-gray-400 hover:text-white transition-colors">
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="px-6 py-6 space-y-6">
          <div>
            <label className="block text-sm font-medium text-white mb-2">Device ID</label>
            <input
              type="text"
              value={deviceForm.deviceId}
              onChange={(e) => setDeviceForm({ ...deviceForm, deviceId: e.target.value.toUpperCase() })}
              className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500 font-mono text-sm"
              placeholder="A8F3C9E2-1B4D-5F7A-8C9E-2D3F4A5B6C7D"
            />
            <p className="text-xs text-gray-400 mt-1">
              The device ID to add. Spaces and dashes are optional (ignored).
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium text-white mb-2">Device Name</label>
            <input
              type="text"
              value={deviceForm.name}
              onChange={(e) => setDeviceForm({ ...deviceForm, name: e.target.value })}
              className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500"
              placeholder="My Device"
            />
            <p className="text-xs text-gray-400 mt-1">
              Optional friendly name for the device.
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium text-white mb-2">Addresses</label>
            <input
              type="text"
              value={deviceForm.address}
              onChange={(e) => setDeviceForm({ ...deviceForm, address: e.target.value })}
              className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500 font-mono text-sm"
              placeholder="dynamic"
            />
            <p className="text-xs text-gray-400 mt-1">
              Enter addresses as <code className="text-gray-300">tcp://ip:port</code> or <code className="text-gray-300">dynamic</code> to use automatic discovery.
            </p>
          </div>
        </div>

        <div className="px-6 py-4 border-t border-gray-700 flex justify-end gap-3 bg-gray-750">
          <button
            onClick={handleClose}
            className="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-white rounded transition-colors text-sm"
          >
            Cancel
          </button>
          <button
            onClick={handleSave}
            disabled={!deviceForm.deviceId}
            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded transition-colors text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Add Device
          </button>
        </div>
      </div>
    </div>
  );
};

// Main Dashboard Component
const DashBoard = () => {
  const [folders, setFolders] = useState<FolderType[]>([
    { name: 'Documents', status: 'Up to Date', files: 1247, size: '4.2 GB', id: 'doc-123', path: 'C:\\Users\\Documents' },
    { name: 'Photos', status: 'Syncing', files: 3891, size: '12.8 GB', id: 'pho-456', path: 'C:\\Users\\Photos' }
  ]);

  const [devices, setDevices] = useState<DeviceType[]>([
    { name: 'Desktop-PC', status: 'Connected', id: 'A8F3C9E2' },
    { name: 'Laptop', status: 'Up to Date', id: '9B2D4F7A' }
  ]);

  const [showAddFolder, setShowAddFolder] = useState(false);
  const [showAddDevice, setShowAddDevice] = useState(false);

  const handleAddFolder = (folder: FolderType) => {
    setFolders([...folders, folder]);
  };

  const handleAddDevice = (device: DeviceType) => {
    setDevices([...devices, device]);
  };

  return (
    <div className="min-h-screen bg-gray-900 text-gray-200 p-6">
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

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
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

        <div>
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