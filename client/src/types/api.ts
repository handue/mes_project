// API Response Types
export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

// Machine Types
export interface MachineResponse {
  machineId: string;
  name: string;
  type: string;
  status: string;
  location: string;
  workcenterId?: string;
}

// Workorder Types
export interface WorkorderResponse {
  orderId: string;
  productId: string;
  workcenterId: string;
  machineId: string;
  employeeId: string;
  quantity: number;
  plannedStartTime: string;
  plannedEndTime: string;
  actualStartTime?: string;
  actualEndTime?: string;
  status: string;
  priority: number;
  leadTime: number;
  lotNumber?: string;
  actualProduction?: number;
  scrap?: number;
  setupTimeActual?: number;
}

// Inventory Types
export interface InventoryResponse {
  itemId: string;
  name: string;
  category: string;
  quantity: number;
  reorderLevel: number;
  supplierId?: string;
  leadTime?: number;
  cost?: number;
  lotNumber?: string;
  location?: string;
  lastReceivedDate?: string;
}

// Dashboard Stats Types
export interface DashboardStats {
  totalMachines: number;
  activeMachines: number;
  totalWorkorders: number;
  completedWorkorders: number;
  lowStockItems: number;
  totalInventoryValue: number;
}