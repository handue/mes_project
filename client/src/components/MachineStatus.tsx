import React from 'react';
import type { MachineResponse } from '../types/api';
import { Settings, CheckCircle, AlertCircle, Clock } from 'lucide-react';

interface MachineStatusProps {
  machines: MachineResponse[];
}

const MachineStatus: React.FC<MachineStatusProps> = ({ machines }) => {
  const getStatusIcon = (status: string) => {
    switch (status.toLowerCase()) {
      case 'running':
        return <CheckCircle className="h-5 w-5 text-green-500" />;
      case 'idle':
        return <Clock className="h-5 w-5 text-yellow-500" />;
      case 'maintenance':
        return <Settings className="h-5 w-5 text-blue-500" />;
      default:
        return <AlertCircle className="h-5 w-5 text-red-500" />;
    }
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'running':
        return 'bg-green-100 text-green-800';
      case 'idle':
        return 'bg-yellow-100 text-yellow-800';
      case 'maintenance':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-red-100 text-red-800';
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border">
      <div className="px-6 py-4 border-b">
        <h3 className="text-lg font-medium text-gray-900 flex items-center">
          <Settings className="h-5 w-5 mr-2" />
          Machine Status
        </h3>
        <p className="text-sm text-gray-500 mt-1">Real-time equipment monitoring</p>
      </div>
      
      <div className="p-6">
        {machines.length === 0 ? (
          <div className="text-center py-8">
            <Settings className="h-12 w-12 text-gray-300 mx-auto mb-4" />
            <p className="text-gray-500">No machines found</p>
          </div>
        ) : (
          <div className="space-y-4">
            {machines.map((machine) => (
              <div key={machine.machineId} className="flex items-center justify-between p-4 border rounded-lg hover:bg-gray-50 transition-colors">
                <div className="flex items-center space-x-3">
                  {getStatusIcon(machine.status)}
                  <div>
                    <h4 className="font-medium text-gray-900">{machine.name}</h4>
                    <p className="text-sm text-gray-500">{machine.type} • {machine.location}</p>
                  </div>
                </div>
                <div className="flex items-center space-x-3">
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(machine.status)}`}>
                    {machine.status}
                  </span>
                  <span className="text-sm text-gray-500">ID: {machine.machineId}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default MachineStatus;