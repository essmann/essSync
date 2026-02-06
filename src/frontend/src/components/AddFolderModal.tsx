// AddFolderModal.tsx
import { useState } from 'react';
import { X, FolderPlus } from 'lucide-react';
import { addFolder } from '../api/addFolder';
import type { SharedFolder } from '../api/types/sharedFolder';

interface AddFolderModalProps {
  isOpen: boolean;
  onClose: () => void;
  onAddFolder: (folder: SharedFolder) => void; // Trigger a refresh after successful save
}

const AddFolderModal = ({ isOpen, onClose, onAddFolder }: AddFolderModalProps) => {
  const [activeTab, setActiveTab] = useState('general');
  const [folderForm, setFolderForm] = useState({
    label: '',
    id: '',
    path: ''
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);



  const handleSave = async () => {
    console.log("Attempting to save folder:", folderForm);

    // Validate required fields
    if (!folderForm.path) {
      setError('Folder Path is required');
      return;
    }

    setIsSubmitting(true);
    setError(null);

    try {
      const newFolder: SharedFolder = {
        FolderName: folderForm.label,
        LocalPath: folderForm.path,
        FolderGuid: null,
        IsPaused: false,
        Size: 0,
        NumFiles: 0,
        NumSubFolders: 0,
        CreatedAt: new Date().toISOString(),
        LastSyncedAt: null
      };

      console.log("Calling addFolder API with:", newFolder);
      const folderResponse = await addFolder(newFolder);
      console.log("Folder added successfully");

      // Reset form and close
      setFolderForm({ label: '', id: '', path: '' });
      setActiveTab('general');
      setError(null);
      onClose();
      onAddFolder(folderResponse); // Trigger parent to refresh folder list
    } catch (err) {
      console.error("Error adding folder:", err);
      setError(err instanceof Error ? err.message : 'Failed to add folder');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleClose = () => {
    setFolderForm({ label: '', id: '', path: '' });
    setActiveTab('general');
    setError(null);
    onClose();
  };

  // Check if form is valid
  const isFormValid = Boolean(folderForm.path);

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
      <div className="bg-gray-800 rounded-lg w-full max-w-2xl max-h-[90vh] overflow-hidden shadow-2xl">
        {/* Modal Header */}
        <div className="px-6 py-4 border-b border-gray-700 flex items-center justify-between bg-gray-750">
          <div className="flex items-center gap-2">
            <FolderPlus className="w-5 h-5 text-blue-400" />
            <h2 className="text-lg font-medium text-white">
              Add Folder {`(${folderForm.id})`}
            </h2>
          </div>
          <button
            onClick={handleClose}
            className="text-gray-400 hover:text-white transition-colors"
            disabled={isSubmitting}
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
              disabled={isSubmitting}
              className={`pb-3 px-1 text-sm font-medium transition-colors border-b-2 ${activeTab === tab
                ? 'text-blue-400 border-blue-400'
                : 'text-gray-400 border-transparent hover:text-gray-300'
                } ${isSubmitting ? 'cursor-not-allowed opacity-50' : ''}`}
            >
              {tab.split('-').map(w => w.charAt(0).toUpperCase() + w.slice(1)).join(' ')}
            </button>
          ))}
        </div>

        {/* Modal Content */}
        <div className="px-6 py-6 overflow-y-auto max-h-[calc(90vh-180px)]">
          {/* Error Message */}
          {error && (
            <div className="mb-4 p-3 bg-red-500/10 border border-red-500/50 rounded text-red-400 text-sm">
              {error}
            </div>
          )}

          {activeTab === 'general' && (
            <div className="space-y-6">
              {/* Folder Label */}
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
                  disabled={isSubmitting}
                />
                <p className="text-xs text-gray-400 mt-1">
                  Optional descriptive label for the folder. Can be different on each device.
                </p>
              </div>



              {/* Folder Path */}
              <div>
                <label className="block text-sm font-medium text-white mb-2">
                  Folder Path <span className="text-red-400">*</span>
                </label>
                <input
                  type="text"
                  value={folderForm.path}
                  onChange={(e) => setFolderForm({ ...folderForm, path: e.target.value })}
                  className="w-full bg-gray-700 border border-gray-600 rounded px-3 py-2 text-white focus:outline-none focus:border-blue-500 font-mono text-sm"
                  placeholder="C:\Users\kenwi"
                  disabled={isSubmitting}
                />
                <p className="text-xs text-gray-400 mt-1">
                  Path to the folder on the local computer. Will be created if it does not exist. The tilde character (~) can be used as a shortcut for `C:\Users\kenwi`.
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
            disabled={isSubmitting}
            className="px-4 py-2 bg-gray-700 hover:bg-gray-600 text-white rounded transition-colors text-sm disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Close
          </button>
          <button
            onClick={handleSave}
            disabled={isSubmitting || !isFormValid}
            className={`px-4 py-2 rounded font-medium transition-colors text-sm ${isSubmitting || !isFormValid
              ? 'bg-gray-600 text-gray-400 cursor-not-allowed'
              : 'bg-blue-600 hover:bg-blue-700 text-white'
              }`}
          >
            {isSubmitting ? 'Saving...' : 'Save'}
          </button>
        </div>
      </div>
    </div>
  );
};

export default AddFolderModal;