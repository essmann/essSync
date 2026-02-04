import { useState } from 'react';
import { X, Monitor } from 'lucide-react';

interface Device {
  name: string;
  status: string;
  id: string;
  addresses?: string[]; // added addresses array
}

interface AddDeviceModalProps {
  isOpen: boolean;
  onClose: () => void;
  onAddDevice: (device: Device) => Promise<void>; // async
}

const AddDeviceModal = ({ isOpen, onClose, onAddDevice }: AddDeviceModalProps) => {
  const [deviceForm, setDeviceForm] = useState({
    deviceId: '',
    name: '',
    address: ''
  });

  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSave = async () => {
    if (!deviceForm.deviceId) return;

    setIsSaving(true);
    setError(null);

    try {
      // Split comma-separated addresses into an array and trim spaces
      const addressesArray = deviceForm.address
        .split(',')
        .map(addr => addr.trim())
        .filter(addr => addr.length > 0);

      await onAddDevice({
        name: deviceForm.name || 'New Device',
        status: 'Disconnected',
        id: deviceForm.deviceId,
        addresses: addressesArray
      });

      setDeviceForm({ deviceId: '', name: '', address: '' });
      onClose();
    } catch (err: any) {
      console.error('Error adding device:', err);
      setError(err.message || 'Failed to add device. Check your network connection.');
    } finally {
      setIsSaving(false);
    }
  };

  const handleClose = () => {
    setDeviceForm({ deviceId: '', name: '', address: '' });
    setError(null);
    onClose();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
      <div className="bg-gray-800 rounded-lg w-full max-w-xl shadow-2xl">
        {/* Modal Header */}
        <div className="px-6 py-4 border-b border-gray-700 flex items-center justify-between bg-gray-750">
          <div className="flex items-center gap-2">
            <Monitor className="w-5 h-5 text-blue-400" />
            <h2 className="text-lg font-medium text-white">Add Remote Device</h2>
          </div>
          <button
            onClick={handleClose}
            className="text-gray-400 hover:text-white transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Modal Content */}
        <div className="px-6 py-6 space-y-6">
          {error && <div className="text-sm text-red-400">{error}</div>}

          <div>
            <label className="block text-sm font-medium text-white mb-2">
              Device ID
            </label>
            <input
              type="text"
              value={deviceForm.deviceId}
              onChange={(e) =>
                setDeviceForm({ ...deviceForm, deviceId: e.target.value.toUpperCase() })
              }
              className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500 font-mono text-sm"
              placeholder="A8F3C9E2-1B4D-5F7A-8C9E-2D3F4A5B6C7D"
            />
            <p className="text-xs text-gray-400 mt-1">
              The device ID to add. Spaces and dashes are optional (ignored).
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium text-white mb-2">
              Device Name
            </label>
            <input
              type="text"
              value={deviceForm.name}
              onChange={(e) => setDeviceForm({ ...deviceForm, name: e.target.value })}
              className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500"
              placeholder="My Device"
            />
            <p className="text-xs text-gray-400 mt-1">
              Optional friendly name for the device. If not set, the device ID will be used.
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium text-white mb-2">
              Addresses
            </label>
            <input
              type="text"
              value={deviceForm.address}
              onChange={(e) => setDeviceForm({ ...deviceForm, address: e.target.value })}
              className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500 font-mono text-sm"
              placeholder="tcp://192.168.1.2:1234, tcp://192.168.1.3:1234"
            />
            <p className="text-xs text-gray-400 mt-1">
              Enter addresses separated by commas. Use <code className="text-gray-300">dynamic</code> for automatic discovery.
            </p>
          </div>
        </div>

        {/* Modal Footer */}
        <div className="px-6 py-4 border-t border-gray-700 flex justify-end gap-3 bg-gray-750">
          <button
            onClick={handleClose}
            className="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-white rounded transition-colors text-sm"
          >
            Cancel
          </button>
          <button
            onClick={handleSave}
            disabled={!deviceForm.deviceId || isSaving}
            className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded transition-colors text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isSaving ? 'Adding...' : 'Add Device'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default AddDeviceModal;
