import React from 'react';
import type { InventoryResponse } from '../types/api';
import { AlertTriangle, Package } from 'lucide-react';

interface LowStockAlertProps {
  inventory: InventoryResponse[];
}

const LowStockAlert: React.FC<LowStockAlertProps> = ({ inventory }) => {
  if (inventory.length === 0) {
    return null;
  }

  return (
    <div className="bg-white rounded-lg shadow-sm border border-yellow-200">
      <div className="px-6 py-4 border-b border-yellow-200 bg-yellow-50">
        <h3 className="text-lg font-medium text-yellow-900 flex items-center">
          <AlertTriangle className="h-5 w-5 mr-2" />
          Low Stock Alert
        </h3>
        <p className="text-sm text-yellow-700 mt-1">Items requiring immediate attention</p>
      </div>
      
      <div className="p-6">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {inventory.map((item) => (
            <div key={item.itemId} className="border border-yellow-200 rounded-lg p-4 bg-yellow-50">
              <div className="flex items-center justify-between mb-2">
                <div className="flex items-center space-x-2">
                  <Package className="h-4 w-4 text-yellow-600" />
                  <h4 className="font-medium text-yellow-900">{item.name}</h4>
                </div>
                <span className="text-xs text-yellow-700 bg-yellow-200 px-2 py-1 rounded">
                  {item.category}
                </span>
              </div>
              
              <div className="space-y-1 text-sm">
                <div className="flex justify-between">
                  <span className="text-yellow-700">Current Stock:</span>
                  <span className="font-medium text-yellow-900">{item.quantity}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-yellow-700">Reorder Level:</span>
                  <span className="font-medium text-yellow-900">{item.reorderLevel}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-yellow-700">Location:</span>
                  <span className="font-medium text-yellow-900">{item.location || 'N/A'}</span>
                </div>
                {item.cost && (
                  <div className="flex justify-between">
                    <span className="text-yellow-700">Unit Cost:</span>
                    <span className="font-medium text-yellow-900">${item.cost.toFixed(2)}</span>
                  </div>
                )}
              </div>

              <div className="mt-3 pt-3 border-t border-yellow-200">
                <div className="flex justify-between items-center">
                  <span className="text-xs text-yellow-700">Stock Level</span>
                  <span className="text-xs font-medium text-red-600">
                    {Math.round((item.quantity / item.reorderLevel) * 100)}% of reorder level
                  </span>
                </div>
                <div className="w-full bg-yellow-200 rounded-full h-2 mt-1">
                  <div 
                    className="bg-red-500 h-2 rounded-full" 
                    style={{ width: `${Math.min((item.quantity / item.reorderLevel) * 100, 100)}%` }}
                  ></div>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default React.memo(LowStockAlert);