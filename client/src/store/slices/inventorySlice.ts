import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { InventoryResponse } from '../../types/api';
import { apiService } from '../../services/api';

interface InventoryState {
  inventory: InventoryResponse[];
  loading: boolean;
  error: string | null;
  lastUpdated: string | null;
}

const initialState: InventoryState = {
  inventory: [],
  loading: false,
  error: null,
  lastUpdated: null,
};

// Async thunks
export const fetchInventory = createAsyncThunk(
  'inventory/fetchInventory',
  async (_, { rejectWithValue }) => {
    try {
      const inventory = await apiService.getInventory();
      return inventory;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch inventory');
    }
  }
);

export const fetchInventoryItem = createAsyncThunk(
  'inventory/fetchInventoryItem',
  async (id: string, { rejectWithValue }) => {
    try {
      const item = await apiService.getInventoryItem(id);
      return item;
    } catch (error: any) {
      return rejectWithValue(error.message || 'Failed to fetch inventory item');
    }
  }
);

const inventorySlice = createSlice({
  name: 'inventory',
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    resetInventory: (state) => {
      state.inventory = [];
      state.error = null;
      state.lastUpdated = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch all inventory
      .addCase(fetchInventory.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchInventory.fulfilled, (state, action) => {
        state.loading = false;
        state.inventory = action.payload;
        state.lastUpdated = new Date().toISOString();
      })
      .addCase(fetchInventory.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      })
      // Fetch single inventory item
      .addCase(fetchInventoryItem.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchInventoryItem.fulfilled, (state, action) => {
        state.loading = false;
        // Update item in the array or add if doesn't exist
        const existingIndex = state.inventory.findIndex(
          (i) => i.itemId === action.payload.itemId
        );
        if (existingIndex >= 0) {
          state.inventory[existingIndex] = action.payload;
        } else {
          state.inventory.push(action.payload);
        }
        state.lastUpdated = new Date().toISOString();
      })
      .addCase(fetchInventoryItem.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearError, resetInventory } = inventorySlice.actions;
export default inventorySlice.reducer;