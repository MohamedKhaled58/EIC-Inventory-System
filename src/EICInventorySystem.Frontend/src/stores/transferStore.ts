import { create } from 'zustand';
import { Transfer, TransferStatus } from '../types';

interface TransferState {
    transfers: Transfer[];
    isLoading: boolean;
    error: string | null;

    fetchTransfers: (status?: TransferStatus) => Promise<void>;
    fetchTransfer: (id: number) => Promise<Transfer>;
    createTransfer: (transfer: Partial<Transfer>) => Promise<Transfer>;
    approveTransfer: (id: number, comment?: string) => Promise<void>;
    rejectTransfer: (id: number, reason: string) => Promise<void>;
    completeTransfer: (id: number) => Promise<void>;
    deleteTransfer: (id: number) => Promise<void>;
    clearError: () => void;
}

export const useTransferStore = create<TransferState>((set, get) => ({
    transfers: [],
    isLoading: false,
    error: null,

    fetchTransfers: async (status?: TransferStatus) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const url = status
                ? `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/transfers?status=${status}`
                : `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/transfers`;

            const response = await fetch(url, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch transfers');

            const data = await response.json();
            set({ transfers: data, isLoading: false });
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch transfers',
                isLoading: false,
            });
        }
    },

    fetchTransfer: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/transfers/${id}`, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch transfer');

            const data = await response.json();
            set({ isLoading: false });
            return data;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch transfer',
                isLoading: false,
            });
            throw error;
        }
    },

    createTransfer: async (transfer: Partial<Transfer>) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/transfers`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify(transfer),
            });

            if (!response.ok) throw new Error('Failed to create transfer');

            const data = await response.json();
            set((state) => ({ transfers: [...state.transfers, data], isLoading: false }));
            return data;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to create transfer',
                isLoading: false,
            });
            throw error;
        }
    },

    approveTransfer: async (id: number, comment?: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/transfers/${id}/approve`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify({ comment }),
            });

            if (!response.ok) throw new Error('Failed to approve transfer');

            const data = await response.json();
            set((state) => ({
                transfers: state.transfers.map((t) => (t.id === id ? data : t)),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to approve transfer',
                isLoading: false,
            });
            throw error;
        }
    },

    rejectTransfer: async (id: number, reason: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/transfers/${id}/reject`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify({ reason }),
            });

            if (!response.ok) throw new Error('Failed to reject transfer');

            const data = await response.json();
            set((state) => ({
                transfers: state.transfers.map((t) => (t.id === id ? data : t)),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to reject transfer',
                isLoading: false,
            });
            throw error;
        }
    },

    completeTransfer: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/transfers/${id}/complete`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to complete transfer');

            const data = await response.json();
            set((state) => ({
                transfers: state.transfers.map((t) => (t.id === id ? data : t)),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to complete transfer',
                isLoading: false,
            });
            throw error;
        }
    },

    deleteTransfer: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/transfers/${id}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to delete transfer');

            set((state) => ({
                transfers: state.transfers.filter((t) => t.id !== id),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to delete transfer',
                isLoading: false,
            });
            throw error;
        }
    },

    clearError: () => {
        set({ error: null });
    },
}));
