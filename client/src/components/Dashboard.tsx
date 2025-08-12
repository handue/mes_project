import React, { useEffect, useState } from 'react';
import { Activity, Settings, Package, ClipboardList, RefreshCw } from 'lucide-react';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { fetchMachines } from '../store/slices/machineSlice';
import { fetchWorkorders } from '../store/slices/workorderSlice';
import { fetchInventory } from '../store/slices/inventorySlice';
import { fetchDashboardStats } from '../store/slices/dashboardSlice';
import StatsCard from './StatsCard.tsx';
import MachineStatus from './MachineStatus';
import RecentWorkorders from './RecentWorkorders';
import LowStockAlert from './LowStockAlert.tsx';
import ConnectionStatus from './ConnectionStatus';
import LoadingSkeleton from './LoadingSkeleton';
import DashboardSettings from './DashboardSettings';

const Dashboard: React.FC = () => {
  const dispatch = useAppDispatch();
  const [showSettings, setShowSettings] = useState(false);
  
  // Redux state selectors
  const { stats, loading: dashboardLoading, error: dashboardError, autoRefresh } = useAppSelector((state) => state.dashboard);
  const { machines, loading: machinesLoading, error: machinesError } = useAppSelector((state) => state.machines);
  const { workorders, loading: workordersLoading, error: workordersError } = useAppSelector((state) => state.workorders);
  const { inventory, loading: inventoryLoading, error: inventoryError } = useAppSelector((state) => state.inventory);

  // Combined loading and error states
  const loading = dashboardLoading || machinesLoading || workordersLoading || inventoryLoading;
  const error = dashboardError || machinesError || workordersError || inventoryError;

  useEffect(() => {
    loadDashboardData();
    
    // Auto-refresh every 30 seconds if enabled
    if (autoRefresh) {
      const interval = setInterval(loadDashboardData, 30000);
      return () => clearInterval(interval);
    }
  }, [autoRefresh]);

  const loadDashboardData = () => {
    dispatch(fetchMachines());
    dispatch(fetchWorkorders());
    dispatch(fetchInventory());
    dispatch(fetchDashboardStats());
  };

  if (loading && !stats && (!machines || machines.length === 0)) {
    return (
      <div className="min-h-screen bg-gray-50">
        {/* Header Skeleton */}
        <div className="bg-white shadow-sm border-b">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex justify-between items-center py-6">
              <div className="animate-pulse">
                <div className="h-8 bg-gray-200 rounded w-64 mb-2"></div>
                <div className="h-4 bg-gray-200 rounded w-48"></div>
              </div>
              <div className="animate-pulse">
                <div className="h-10 bg-gray-200 rounded w-32"></div>
              </div>
            </div>
          </div>
        </div>

        {/* Content Skeleton */}
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          {/* Stats Cards Skeleton */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            {Array.from({ length: 4 }).map((_, i) => (
              <LoadingSkeleton key={i} type="card" />
            ))}
          </div>

          {/* Dashboard Grid Skeleton */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
            <LoadingSkeleton type="list" count={3} />
            <LoadingSkeleton type="table" count={3} />
          </div>
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
            className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
          >
            <RefreshCw className="h-4 w-4" />
            <span>Retry Connection</span>
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
            <div className="flex items-center space-x-4">
              <button
                onClick={() => setShowSettings(true)}
                className="flex items-center space-x-2 bg-gray-100 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-200 transition-colors"
              >
                <Settings className="h-4 w-4" />
                <span>Settings</span>
              </button>
              <button
                onClick={loadDashboardData}
                disabled={loading}
                className="flex items-center space-x-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
              >
                <RefreshCw className={`h-4 w-4 ${loading ? 'animate-spin' : ''}`} />
                <span>Refresh</span>
              </button>
              <ConnectionStatus />
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
          <RecentWorkorders workorders={(workorders || []).slice(0, 5)} />
        </div>

        {/* Low Stock Alert */}
        {stats && stats.lowStockItems > 0 && (
          <div className="mt-8">
            <LowStockAlert inventory={(inventory || []).filter(item => item.quantity <= item.reorderLevel)} />
          </div>
        )}
      </div>

      {/* Dashboard Settings Modal */}
      <DashboardSettings 
        isOpen={showSettings} 
        onClose={() => setShowSettings(false)} 
      />
    </div>
  );
};

export default Dashboard;