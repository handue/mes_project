import React from 'react';
import type { WorkorderResponse } from '../types/api';
import { ClipboardList, Calendar, User, Package } from 'lucide-react';

interface RecentWorkordersProps {
  workorders: WorkorderResponse[];
}

const RecentWorkorders: React.FC<RecentWorkordersProps> = ({ workorders }) => {
  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'completed':
        return 'bg-green-100 text-green-800';
      case 'in progress':
        return 'bg-blue-100 text-blue-800';
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'cancelled':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('ko-KR', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="bg-white rounded-lg shadow-sm border">
      <div className="px-6 py-4 border-b">
        <h3 className="text-lg font-medium text-gray-900 flex items-center">
          <ClipboardList className="h-5 w-5 mr-2" />
          Recent Work Orders
        </h3>
        <p className="text-sm text-gray-500 mt-1">Latest production orders</p>
      </div>
      
      <div className="p-6">
        {!workorders || workorders.length === 0 ? (
          <div className="text-center py-8">
            <ClipboardList className="h-12 w-12 text-gray-300 mx-auto mb-4" />
            <p className="text-gray-500">No work orders found</p>
          </div>
        ) : (
          <div className="space-y-4">
            {(workorders || []).map((workorder) => (
              <div key={workorder.orderId} className="border rounded-lg p-4 hover:bg-gray-50 transition-colors">
                <div className="flex items-center justify-between mb-3">
                  <div className="flex items-center space-x-2">
                    <h4 className="font-medium text-gray-900">Order #{workorder.orderId}</h4>
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(workorder.status)}`}>
                      {workorder.status}
                    </span>
                  </div>
                  <div className="text-sm text-gray-500">
                    Priority: {workorder.priority}
                  </div>
                </div>
                
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div className="flex items-center space-x-2">
                    <Package className="h-4 w-4 text-gray-400" />
                    <span className="text-gray-600">Product: {workorder.productId}</span>
                  </div>
                  <div className="flex items-center space-x-2">
                    <User className="h-4 w-4 text-gray-400" />
                    <span className="text-gray-600">Machine: {workorder.machineId}</span>
                  </div>
                  <div className="flex items-center space-x-2">
                    <Calendar className="h-4 w-4 text-gray-400" />
                    <span className="text-gray-600">Start: {formatDate(workorder.plannedStartTime)}</span>
                  </div>
                  <div className="flex items-center space-x-2">
                    <Package className="h-4 w-4 text-gray-400" />
                    <span className="text-gray-600">Qty: {workorder.quantity}</span>
                  </div>
                </div>

                {workorder.actualProduction !== undefined && (
                  <div className="mt-3 pt-3 border-t">
                    <div className="flex justify-between items-center text-sm">
                      <span className="text-gray-600">Progress:</span>
                      <span className="font-medium">
                        {workorder.actualProduction} / {workorder.quantity} 
                        ({Math.round((workorder.actualProduction / workorder.quantity) * 100)}%)
                      </span>
                    </div>
                    <div className="w-full bg-gray-200 rounded-full h-2 mt-2">
                      <div 
                        className="bg-blue-600 h-2 rounded-full" 
                        style={{ width: `${Math.min((workorder.actualProduction / workorder.quantity) * 100, 100)}%` }}
                      ></div>
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default React.memo(RecentWorkorders);