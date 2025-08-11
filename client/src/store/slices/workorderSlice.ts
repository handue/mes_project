import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { WorkorderResponse } from '../../types/api';
import { apiService } from '../../services/api';

interface WorkorderState {
  workorders: WorkorderResponse[];
  loading: boolean;
  error: string | null;
  lastUpdated: string | null;
}

const initialState: WorkorderState = {
  workorders: [],
  loading: false,
  error: null,
  lastUpdated: null,
};

// Async thunks
export const fetchWorkorders = createAsyncThunk(
  'workorders/fetchWorkorders',
  async (_, { rejectWithValue }) => {
    try {
      const workorders = await apiService.getWorkorders();
      return workorders;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch workorders');
    }
  }
);

export const fetchWorkorder = createAsyncThunk(
  'workorders/fetchWorkorder',
  async (id: string, { rejectWithValue }) => {
    try {
      const workorder = await apiService.getWorkorder(id);
      return workorder;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch workorder');
    }
  }
);

const workorderSlice = createSlice({
  name: 'workorders',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    resetWorkorders: (state) => {
      state.workorders = [];
      state.error = null;
      state.lastUpdated = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch all workorders
      .addCase(fetchWorkorders.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchWorkorders.fulfilled, (state, action) => {
        state.loading = false;
        state.workorders = action.payload;
        state.lastUpdated = new Date().toISOString();
      })
      .addCase(fetchWorkorders.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      // Fetch single workorder
      .addCase(fetchWorkorder.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchWorkorder.fulfilled, (state, action) => {
        state.loading = false;
        // Update workorder in the array or add if doesn't exist
        const existingIndex = state.workorders.findIndex(
          (w) => w.orderId === action.payload.orderId
        );
        if (existingIndex >= 0) {
          state.workorders[existingIndex] = action.payload;
        } else {
          state.workorders.push(action.payload);
        }
        state.lastUpdated = new Date().toISOString();
      })
      .addCase(fetchWorkorder.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearError, resetWorkorders } = workorderSlice.actions;
export default workorderSlice.reducer;