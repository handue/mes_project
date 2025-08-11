import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { MachineResponse } from '../../types/api';
import { apiService } from '../../services/api';

interface MachineState {
  machines: MachineResponse[];
  loading: boolean;
  error: string | null;
  lastUpdated: string | null;
}

const initialState: MachineState = {
  machines: [],
  loading: false,
  error: null,
  lastUpdated: null,
};

// Async thunks
export const fetchMachines = createAsyncThunk(
  'machines/fetchMachines',
  async (_, { rejectWithValue }) => {
    try {
      const machines = await apiService.getMachines();
      return machines;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch machines');
    }
  }
);

export const fetchMachine = createAsyncThunk(
  'machines/fetchMachine',
  async (id: string, { rejectWithValue }) => {
    try {
      const machine = await apiService.getMachine(id);
      return machine;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch machine');
    }
  }
);

const machineSlice = createSlice({
  name: 'machines',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    resetMachines: (state) => {
      state.machines = [];
      state.error = null;
      state.lastUpdated = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch all machines
      .addCase(fetchMachines.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchMachines.fulfilled, (state, action) => {
        state.loading = false;
        state.machines = action.payload;
        state.lastUpdated = new Date().toISOString();
      })
      .addCase(fetchMachines.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      // Fetch single machine
      .addCase(fetchMachine.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchMachine.fulfilled, (state, action) => {
        state.loading = false;
        // Update machine in the array or add if doesn't exist
        const existingIndex = state.machines.findIndex(
          (m) => m.machineId === action.payload.machineId
        );
        if (existingIndex >= 0) {
          state.machines[existingIndex] = action.payload;
        } else {
          state.machines.push(action.payload);
        }
        state.lastUpdated = new Date().toISOString();
      })
      .addCase(fetchMachine.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearError, resetMachines } = machineSlice.actions;
export default machineSlice.reducer;