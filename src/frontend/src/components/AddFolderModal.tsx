// AddFolderModal.tsx
import { useState } from 'react';
import { X, FolderPlus } from 'lucide-react';

interface Folder {
  name: string;
  status: string;
  files: number;
  size: string;
  id: string;
  path: string;
}

interface AddFolderModalProps {
  isOpen: boolean;
  onClose: () => void;
  onAddFolder: (folder: Folder) => void;
}

const AddFolderModal = ({ isOpen, onClose, onAddFolder }: AddFolderModalProps) => {
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
        {/* Modal Header */}
        <div className="px-6 py-4 border-b border-gray-700 flex items-center justify-between bg-gray-750">
          <div className="flex items-center gap-2">
            <FolderPlus className="w-5 h-5 text-blue-400" />
            <h2 className="text-lg font-medium text-white">
              Add Folder {folderForm.id && `(${folderForm.id})`}
            </h2>
          </div>
          <button
            onClick={handleClose}
            className="text-gray-400 hover:text-white transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Tabs */}
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

        {/* Modal Content */}
        <div className="px-6 py-6 overflow-y-auto max-h-[calc(90vh-180px)]">
          {activeTab === 'general' && (
            <div className="space-y-6">
              <div>
                <label className="block text-sm font-medium text-white mb-2">
                  Folder Label
                </label>
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
                <label className="block text-sm font-medium text-white mb-2">
                  Folder ID
                </label>
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
                  Required identifier for the folder. Must be the same on all cluster devices. When adding a new folder,
                  keep in mind that the Folder ID is used to tie folders together between devices. They are case sensitive
                  and must match exactly between all devices.
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium text-white mb-2">
                  Folder Path
                </label>
                <input
                  type="text"
                  value={folderForm.path}
                  onChange={(e) => setFolderForm({ ...folderForm, path: e.target.value })}
                  className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500 font-mono text-sm"
                  placeholder="C:\Users\kenwi"
                />
                <p className="text-xs text-gray-400 mt-1">
                  Path to the folder on the local computer. Will be created if it does not exist. The tilde character (~) can be
                  used as a shortcut for <code className="text-gray-300">C:\Users\kenwi</code>.
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

        {/* Modal Footer */}
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

export default AddFolderModal;