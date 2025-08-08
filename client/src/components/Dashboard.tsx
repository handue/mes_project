import React, { useState, useEffect } from 'react';
import { Activity, Settings, Package, ClipboardList } from 'lucide-react';
import { apiService } from '../services/api';
import type { DashboardStats, MachineResponse, WorkorderResponse, InventoryResponse } from '../types/api.ts';
import StatsCard from './StatsCard.tsx';
import MachineStatus from './MachineStatus';
import RecentWorkorders from './RecentWorkorders';
import LowStockAlert from './LowStockAlert.tsx';

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [machines, setMachines] = useState<MachineResponse[]>([]);
  const [workorders, setWorkorders] = useState<WorkorderResponse[]>([]);
  const [inventory, setInventory] = useState<InventoryResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadDashboardData();
    
    // Auto-refresh every 30 seconds
    const interval = setInterval(loadDashboardData, 30000);
    return () => clearInterval(interval);
  }, []);

  const loadDashboardData = async () => {
    try {
      setLoading(true);
      setError(null);

      const [statsData, machinesData, workordersData, inventoryData] = await Promise.all([
        apiService.getDashboardStats(),
        apiService.getMachines(),
        apiService.getWorkorders(),
        apiService.getInventory()
      ]);

      setStats(statsData);
      setMachines(machinesData);
      setWorkorders(workordersData);
      setInventory(inventoryData);
    } catch (err) {
      console.error('Error loading dashboard data:', err);
      setError('Failed to load dashboard data. Please check if the API server is running.');
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading dashboard...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-center max-w-md mx-auto p-6">
          <div className="text-red-500 text-6xl mb-4">⚠️</div>
          <h2 className="text-2xl font-bold text-gray-800 mb-2">Connection Error</h2>
          <p className="text-gray-600 mb-4">{error}</p>
          <button
            onClick={loadDashboardData}
            className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            Retry Connection
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-6">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">Oracle MES Dashboard</h1>
              <p className="text-gray-500 mt-1">Manufacturing Execution System Overview</p>
            </div>
            <div className="flex items-center space-x-2">
              <div className="h-3 w-3 bg-green-400 rounded-full animate-pulse"></div>
              <span className="text-sm text-gray-500">Live Data</span>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <StatsCard
            title="Total Machines"
            value={stats?.totalMachines || 0}
            subtitle={`${stats?.activeMachines || 0} active`}
            icon={<Settings className="h-6 w-6" />}
            color="blue"
          />
          <StatsCard
            title="Work Orders"
            value={stats?.totalWorkorders || 0}
            subtitle={`${stats?.completedWorkorders || 0} completed`}
            icon={<ClipboardList className="h-6 w-6" />}
            color="green"
          />
          <StatsCard
            title="Low Stock Items"
            value={stats?.lowStockItems || 0}
            subtitle="Need reorder"
            icon={<Package className="h-6 w-6" />}
            color="yellow"
          />
          <StatsCard
            title="Inventory Value"
            value={`$${(stats?.totalInventoryValue || 0).toLocaleString()}`}
            subtitle="Total value"
            icon={<Activity className="h-6 w-6" />}
            color="purple"
          />
        </div>

        {/* Main Dashboard Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Machine Status */}
          <MachineStatus machines={machines} />

          {/* Recent Work Orders */}
          <RecentWorkorders workorders={workorders.slice(0, 5)} />
        </div>

        {/* Low Stock Alert */}
        {stats && stats.lowStockItems > 0 && (
          <div className="mt-8">
            <LowStockAlert inventory={inventory.filter(item => item.quantity <= item.reorderLevel)} />
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;