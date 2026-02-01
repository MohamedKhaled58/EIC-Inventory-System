import { create } from 'zustand';
import { Requisition, RequisitionItem, RequisitionStatus } from '../types';

interface RequisitionState {
    requisitions: Requisition[];
    isLoading: boolean;
    error: string | null;

    fetchRequisitions: (status?: RequisitionStatus) => Promise<void>;
    fetchRequisition: (id: number) => Promise<Requisition>;
    createRequisition: (requisition: Partial<Requisition>) => Promise<Requisition>;
    updateRequisition: (id: number, requisition: Partial<Requisition>) => Promise<void>;
    approveRequisition: (id: number, comment?: string) => Promise<void>;
    rejectRequisition: (id: number, reason: string) => Promise<void>;
    deleteRequisition: (id: number) => Promise<void>;
    clearError: () => void;
}

export const useRequisitionStore = create<RequisitionState>((set, get) => ({
    requisitions: [],
    isLoading: false,
    error: null,

    fetchRequisitions: async (status?: RequisitionStatus) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const url = status
                ? `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/requisitions?status=${status}`
                : `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/requisitions`;

            const response = await fetch(url, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch requisitions');

            const data = await response.json();
            set({ requisitions: data, isLoading: false });
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch requisitions',
                isLoading: false,
            });
        }
    },

    fetchRequisition: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/requisitions/${id}`, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch requisition');

            const data = await response.json();
            set({ isLoading: false });
            return data;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch requisition',
                isLoading: false,
            });
            throw error;
        }
    },

    createRequisition: async (requisition: Partial<Requisition>) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/requisitions`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify(requisition),
            });

            if (!response.ok) throw new Error('Failed to create requisition');

            const data = await response.json();
            set((state) => ({ requisitions: [...state.requisitions, data], isLoading: false }));
            return data;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to create requisition',
                isLoading: false,
            });
            throw error;
        }
    },

    updateRequisition: async (id: number, requisition: Partial<Requisition>) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/requisitions/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify(requisition),
            });

            if (!response.ok) throw new Error('Failed to update requisition');

            const data = await response.json();
            set((state) => ({
                requisitions: state.requisitions.map((r) => (r.id === id ? data : r)),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to update requisition',
                isLoading: false,
            });
            throw error;
        }
    },

    approveRequisition: async (id: number, comment?: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/requisitions/${id}/approve`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify({ comment }),
            });

            if (!response.ok) throw new Error('Failed to approve requisition');

            const data = await response.json();
            set((state) => ({
                requisitions: state.requisitions.map((r) => (r.id === id ? data : r)),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to approve requisition',
                isLoading: false,
            });
            throw error;
        }
    },

    rejectRequisition: async (id: number, reason: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/requisitions/${id}/reject`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify({ reason }),
            });

            if (!response.ok) throw new Error('Failed to reject requisition');

            const data = await response.json();
            set((state) => ({
                requisitions: state.requisitions.map((r) => (r.id === id ? data : r)),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to reject requisition',
                isLoading: false,
            });
            throw error;
        }
    },

    deleteRequisition: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/requisitions/${id}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to delete requisition');

            set((state) => ({
                requisitions: state.requisitions.filter((r) => r.id !== id),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to delete requisition',
                isLoading: false,
            });
            throw error;
        }
    },

    clearError: () => {
        set({ error: null });
    },
}));
