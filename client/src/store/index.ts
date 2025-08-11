import { configureStore } from '@reduxjs/toolkit';
import machineSlice from './slices/machineSlice.ts';
import workorderSlice from './slices/workorderSlice.ts';
import inventorySlice from './slices/inventorySlice.ts';
import dashboardSlice from './slices/dashboardSlice.ts';

export const store = configureStore({
  reducer: {
    machines: machineSlice,
    workorders: workorderSlice,
    inventory: inventorySlice,
    dashboard: dashboardSlice,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: ['persist/PERSIST'],
      },
    }),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;