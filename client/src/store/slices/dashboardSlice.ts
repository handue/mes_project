import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { DashboardStats } from '../../types/api';
import { apiService } from '../../services/api';

interface DashboardState {
  stats: DashboardStats | null;
  loading: boolean;
  error: string | null;
  lastUpdated: string | null;
  autoRefresh: boolean;
  refreshInterval: number; // in seconds
}

const initialState: DashboardState = {
  stats: null,
  loading: false,
  error: null,
  lastUpdated: null,
  autoRefresh: true,
  refreshInterval: 30,
};

// Async thunks
export const fetchDashboardStats = createAsyncThunk(
  'dashboard/fetchStats',
  async (_, { rejectWithValue }) => {
    try {
      const stats = await apiService.getDashboardStats();
      return stats;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch dashboard stats');
    }
  }
);

const dashboardSlice = createSlice({
  name: 'dashboard',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    toggleAutoRefresh: (state) => {
      state.autoRefresh = !state.autoRefresh;
    },
    setRefreshInterval: (state, action) => {
      state.refreshInterval = action.payload;
    },
    resetDashboard: (state) => {
      state.stats = null;
      state.error = null;
      state.lastUpdated = null;
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchDashboardStats.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchDashboardStats.fulfilled, (state, action) => {
        state.loading = false;
        state.stats = action.payload;
        state.lastUpdated = new Date().toISOString();
      })
      .addCase(fetchDashboardStats.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { 
  clearError, 
  toggleAutoRefresh, 
  setRefreshInterval, 
  resetDashboard 
} = dashboardSlice.actions;

export default dashboardSlice.reducer;