import React, { useState } from 'react';
import { Settings, X, Clock, Wifi } from 'lucide-react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { toggleAutoRefresh, setRefreshInterval } from '../store/slices/dashboardSlice';

interface DashboardSettingsProps {
  isOpen: boolean;
  onClose: () => void;
}

const DashboardSettings: React.FC<DashboardSettingsProps> = ({ isOpen, onClose }) => {
  const dispatch = useAppDispatch();
  const { autoRefresh, refreshInterval } = useAppSelector((state) => state.dashboard);
  const [tempInterval, setTempInterval] = useState(refreshInterval);

  if (!isOpen) return null;

  const handleSave = () => {
    if (tempInterval !== refreshInterval) {
      dispatch(setRefreshInterval(tempInterval));
    }
    onClose();
  };

  const intervalOptions = [
    { value: 10, label: '10 seconds' },
    { value: 30, label: '30 seconds' },
    { value: 60, label: '1 minute' },
    { value: 300, label: '5 minutes' },
  ];

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
        <div className="flex items-center justify-between p-6 border-b">
          <div className="flex items-center space-x-2">
            <Settings className="h-5 w-5 text-gray-600" />
            <h2 className="text-lg font-semibold text-gray-900">Dashboard Settings</h2>
          </div>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition-colors"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="p-6 space-y-6">
          {/* Auto Refresh Toggle */}
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-3">
              <Wifi className="h-5 w-5 text-blue-600" />
              <div>
                <h3 className="text-sm font-medium text-gray-900">Auto Refresh</h3>
                <p className="text-sm text-gray-500">Automatically update data</p>
              </div>
            </div>
            <button
              onClick={() => dispatch(toggleAutoRefresh())}
              className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors ${
                autoRefresh ? 'bg-blue-600' : 'bg-gray-200'
              }`}
            >
              <span
                className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
                  autoRefresh ? 'translate-x-6' : 'translate-x-1'
                }`}
              />
            </button>
          </div>

          {/* Refresh Interval */}
          {autoRefresh && (
            <div className="space-y-3">
              <div className="flex items-center space-x-3">
                <Clock className="h-5 w-5 text-blue-600" />
                <div>
                  <h3 className="text-sm font-medium text-gray-900">Refresh Interval</h3>
                  <p className="text-sm text-gray-500">How often to update data</p>
                </div>
              </div>
              <div className="grid grid-cols-2 gap-2">
                {intervalOptions.map((option) => (
                  <button
                    key={option.value}
                    onClick={() => setTempInterval(option.value)}
                    className={`px-3 py-2 text-sm rounded-md border transition-colors ${
                      tempInterval === option.value
                        ? 'bg-blue-50 border-blue-200 text-blue-700'
                        : 'bg-white border-gray-200 text-gray-700 hover:bg-gray-50'
                    }`}
                  >
                    {option.label}
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* Theme Toggle (Future Enhancement) */}
          <div className="opacity-50">
            <div className="flex items-center space-x-3">
              <div className="h-5 w-5 bg-gray-300 rounded"></div>
              <div>
                <h3 className="text-sm font-medium text-gray-900">Dark Mode</h3>
                <p className="text-sm text-gray-500">Coming soon...</p>
              </div>
            </div>
          </div>
        </div>

        <div className="flex items-center justify-end space-x-3 p-6 border-t">
          <button
            onClick={onClose}
            className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-md hover:bg-gray-50 transition-colors"
          >
            Cancel
          </button>
          <button
            onClick={handleSave}
            className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-md hover:bg-blue-700 transition-colors"
          >
            Save Settings
          </button>
        </div>
      </div>
    </div>
  );
};

export default React.memo(DashboardSettings);