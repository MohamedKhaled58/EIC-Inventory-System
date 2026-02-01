import { create } from 'zustand';
import { InventoryItem, InventoryRecord, Warehouse } from '../types';

interface InventoryState {
    items: InventoryItem[];
    records: InventoryRecord[];
    warehouses: Warehouse[];
    isLoading: boolean;
    error: string | null;

    fetchItems: () => Promise<void>;
    fetchRecords: (warehouseId?: number) => Promise<void>;
    fetchWarehouses: () => Promise<void>;
    createItem: (item: Partial<InventoryItem>) => Promise<void>;
    updateItem: (id: number, item: Partial<InventoryItem>) => Promise<void>;
    deleteItem: (id: number) => Promise<void>;
    adjustStock: (recordId: number, adjustment: number, reason: string) => Promise<void>;
    clearError: () => void;
}

export const useInventoryStore = create<InventoryState>((set, get) => ({
    items: [],
    records: [],
    warehouses: [],
    isLoading: false,
    error: null,

    fetchItems: async () => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/inventory/items`, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch items');

            const data = await response.json();
            set({ items: data, isLoading: false });
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch items',
                isLoading: false,
            });
        }
    },

    fetchRecords: async (warehouseId?: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const url = warehouseId
                ? `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/inventory/records?warehouseId=${warehouseId}`
                : `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/inventory/records`;

            const response = await fetch(url, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch records');

            const data = await response.json();
            set({ records: data, isLoading: false });
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch records',
                isLoading: false,
            });
        }
    },

    fetchWarehouses: async () => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/inventory/warehouses`, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch warehouses');

            const data = await response.json();
            set({ warehouses: data, isLoading: false });
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch warehouses',
                isLoading: false,
            });
        }
    },

    createItem: async (item: Partial<InventoryItem>) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/inventory/items`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify(item),
            });

            if (!response.ok) throw new Error('Failed to create item');

            const data = await response.json();
            set((state) => ({ items: [...state.items, data], isLoading: false }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to create item',
                isLoading: false,
            });
            throw error;
        }
    },

    updateItem: async (id: number, item: Partial<InventoryItem>) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/inventory/items/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify(item),
            });

            if (!response.ok) throw new Error('Failed to update item');

            const data = await response.json();
            set((state) => ({
                items: state.items.map((i) => (i.id === id ? data : i)),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to update item',
                isLoading: false,
            });
            throw error;
        }
    },

    deleteItem: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/inventory/items/${id}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to delete item');

            set((state) => ({
                items: state.items.filter((i) => i.id !== id),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to delete item',
                isLoading: false,
            });
            throw error;
        }
    },

    adjustStock: async (recordId: number, adjustment: number, reason: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/inventory/records/${recordId}/adjust`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify({ adjustment, reason }),
            });

            if (!response.ok) throw new Error('Failed to adjust stock');

            await get().fetchRecords();
            set({ isLoading: false });
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to adjust stock',
                isLoading: false,
            });
            throw error;
        }
    },

    clearError: () => {
        set({ error: null });
    },
}));
