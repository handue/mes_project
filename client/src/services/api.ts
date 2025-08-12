import axios from "axios";
import type { AxiosInstance, AxiosResponse } from "axios";

import type {
  ApiResponse,
  MachineResponse,
  WorkorderResponse,
  InventoryResponse,
  DashboardStats,
} from "../types/api";

class ApiService {
  private api: AxiosInstance;

  constructor() {
    this.api = axios.create({
      baseURL: "http://localhost:5087/api",
      timeout: 10000,
      headers: {
        "Content-Type": "application/json",
      },
    });

    // Request interceptor
    this.api.interceptors.request.use(
      (config: any) => {
        console.log(
          "🚀 API Request:",
          config.method?.toUpperCase(),
          config.url
        );
        return config;
      },
      (error: any) => {
        console.error("❌ Request Error:", error);
        return Promise.reject(error);
      }
    );

    // Response interceptor
    this.api.interceptors.response.use(
      (response: any) => {
        console.log("✅ API Response:", response.status, response.config.url);
        return response;
      },
      (error: any) => {
        console.error(
          "❌ Response Error:",
          error.response?.status,
          error.response?.data
        );
        
        // Handle different error types
        let errorMessage = 'An unexpected error occurred';
        
        if (error.code === 'ECONNABORTED' || error.code === 'ERR_NETWORK') {
          errorMessage = 'Network connection failed. Please check if the API server is running on http://localhost:5087';
        } else if (error.response) {
          // Server responded with error status
          const status = error.response.status;
          switch (status) {
            case 404:
              errorMessage = 'API endpoint not found';
              break;
            case 500:
              errorMessage = 'Internal server error';
              break;
            case 503:
              errorMessage = 'Service unavailable';
              break;
            default:
              errorMessage = `Server error: ${status}`;
          }
        } else if (error.request) {
          errorMessage = 'No response from server. Please check if the API server is running.';
        }
        
        // Create enhanced error object
        const enhancedError = new Error(errorMessage);
        (enhancedError as any).originalError = error;
        (enhancedError as any).status = error.response?.status;
        
        return Promise.reject(enhancedError);
      }
    );
  }

  // Machine API methods
  async getMachines(): Promise<MachineResponse[]> {
    try {
      const response: AxiosResponse<ApiResponse<MachineResponse[]>> =
        await this.api.get("/machine");
      return response.data.data || [];
    } catch (error) {
      console.error("Error fetching machines:", error);
      throw error;
    }
  }

  async getMachine(id: string): Promise<MachineResponse> {
    try {
      const response: AxiosResponse<ApiResponse<MachineResponse>> =
        await this.api.get(`/machine/${id}`);
      return response.data.data;
    } catch (error) {
      console.error("Error fetching machine:", error);
      throw error;
    }
  }

  // Workorder API methods
  async getWorkorders(): Promise<WorkorderResponse[]> {
    try {
      const response: AxiosResponse<ApiResponse<WorkorderResponse[]>> =
        await this.api.get("/workorder");
      return response.data.data || [];
    } catch (error) {
      console.error("Error fetching workorders:", error);
      throw error;
    }
  }

  async getWorkorder(id: string): Promise<WorkorderResponse> {
    try {
      const response: AxiosResponse<ApiResponse<WorkorderResponse>> =
        await this.api.get(`/workorder/${id}`);
      return response.data.data;
    } catch (error) {
      console.error("Error fetching workorder:", error);
      throw error;
    }
  }

  // Inventory API methods
  async getInventory(): Promise<InventoryResponse[]> {
    try {
      const response: AxiosResponse<ApiResponse<InventoryResponse[]>> =
        await this.api.get("/inventory");
      return response.data.data || [];
    } catch (error) {
      console.error("Error fetching inventory:", error);
      throw error;
    }
  }

  async getInventoryItem(id: string): Promise<InventoryResponse> {
    try {
      const response: AxiosResponse<ApiResponse<InventoryResponse>> =
        await this.api.get(`/inventory/${id}`);
      return response.data.data;
    } catch (error) {
      console.error("Error fetching inventory item:", error);
      throw error;
    }
  }

  // Dashboard Stats (computed from other APIs)
  async getDashboardStats(): Promise<DashboardStats> {
    try {
      const [machines, workorders, inventory] = await Promise.all([
        this.getMachines().catch(() => []),
        this.getWorkorders().catch(() => []),
        this.getInventory().catch(() => []),
      ]);

      // Ensure arrays are valid
      const safeMachines = machines || [];
      const safeWorkorders = workorders || [];
      const safeInventory = inventory || [];

      const stats: DashboardStats = {
        totalMachines: safeMachines.length,
        activeMachines: safeMachines.filter(
          (m) => m.status === "running" || m.status === "idle"
        ).length,
        totalWorkorders: safeWorkorders.length,
        completedWorkorders: safeWorkorders.filter((w) => w.status === "Completed")
          .length,
        lowStockItems: safeInventory.filter((i) => i.quantity <= i.reorderLevel)
          .length,
        totalInventoryValue: safeInventory.reduce(
          (sum, item) => sum + (item.cost || 0) * item.quantity,
          0
        ),
      };

      return stats;
    } catch (error) {
      console.error("Error calculating dashboard stats:", error);
      // Return default stats instead of throwing
      return {
        totalMachines: 0,
        activeMachines: 0,
        totalWorkorders: 0,
        completedWorkorders: 0,
        lowStockItems: 0,
        totalInventoryValue: 0,
      };
    }
  }
}

// Export singleton instance
export const apiService = new ApiService();
export default apiService;
