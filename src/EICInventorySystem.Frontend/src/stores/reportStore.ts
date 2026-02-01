import { create } from 'zustand';

interface ReportState {
    isLoading: boolean;
    error: string | null;

    generateInventoryReport: (warehouseId?: number, period?: string) => Promise<Blob>;
    generateRequisitionReport: (status?: string, period?: string) => Promise<Blob>;
    generateTransferReport: (period?: string) => Promise<Blob>;
    generateProjectReport: (projectId?: number, period?: string) => Promise<Blob>;
    generateAuditReport: (userId?: number, period?: string) => Promise<Blob>;
    generateCommanderReserveReport: (warehouseId?: number) => Promise<Blob>;
    clearError: () => void;
}

export const useReportStore = create<ReportState>((set) => ({
    isLoading: false,
    error: null,

    generateInventoryReport: async (warehouseId?: number, period?: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const params = new URLSearchParams();
            if (warehouseId) params.append('warehouseId', warehouseId.toString());
            if (period) params.append('period', period);

            const response = await fetch(
                `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/reports/inventory?${params}`,
                {
                    headers: {
                        Authorization: `Bearer ${auth?.state?.token}`,
                    },
                }
            );

            if (!response.ok) throw new Error('Failed to generate inventory report');

            const blob = await response.blob();
            set({ isLoading: false });
            return blob;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to generate inventory report',
                isLoading: false,
            });
            throw error;
        }
    },

    generateRequisitionReport: async (status?: string, period?: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const params = new URLSearchParams();
            if (status) params.append('status', status);
            if (period) params.append('period', period);

            const response = await fetch(
                `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/reports/requisitions?${params}`,
                {
                    headers: {
                        Authorization: `Bearer ${auth?.state?.token}`,
                    },
                }
            );

            if (!response.ok) throw new Error('Failed to generate requisition report');

            const blob = await response.blob();
            set({ isLoading: false });
            return blob;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to generate requisition report',
                isLoading: false,
            });
            throw error;
        }
    },

    generateTransferReport: async (period?: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const params = new URLSearchParams();
            if (period) params.append('period', period);

            const response = await fetch(
                `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/reports/transfers?${params}`,
                {
                    headers: {
                        Authorization: `Bearer ${auth?.state?.token}`,
                    },
                }
            );

            if (!response.ok) throw new Error('Failed to generate transfer report');

            const blob = await response.blob();
            set({ isLoading: false });
            return blob;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to generate transfer report',
                isLoading: false,
            });
            throw error;
        }
    },

    generateProjectReport: async (projectId?: number, period?: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const params = new URLSearchParams();
            if (projectId) params.append('projectId', projectId.toString());
            if (period) params.append('period', period);

            const response = await fetch(
                `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/reports/projects?${params}`,
                {
                    headers: {
                        Authorization: `Bearer ${auth?.state?.token}`,
                    },
                }
            );

            if (!response.ok) throw new Error('Failed to generate project report');

            const blob = await response.blob();
            set({ isLoading: false });
            return blob;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to generate project report',
                isLoading: false,
            });
            throw error;
        }
    },

    generateAuditReport: async (userId?: number, period?: string) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const params = new URLSearchParams();
            if (userId) params.append('userId', userId.toString());
            if (period) params.append('period', period);

            const response = await fetch(
                `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/reports/audit?${params}`,
                {
                    headers: {
                        Authorization: `Bearer ${auth?.state?.token}`,
                    },
                }
            );

            if (!response.ok) throw new Error('Failed to generate audit report');

            const blob = await response.blob();
            set({ isLoading: false });
            return blob;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to generate audit report',
                isLoading: false,
            });
            throw error;
        }
    },

    generateCommanderReserveReport: async (warehouseId?: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const params = new URLSearchParams();
            if (warehouseId) params.append('warehouseId', warehouseId.toString());

            const response = await fetch(
                `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/reports/commander-reserve?${params}`,
                {
                    headers: {
                        Authorization: `Bearer ${auth?.state?.token}`,
                    },
                }
            );

            if (!response.ok) throw new Error('Failed to generate commander reserve report');

            const blob = await response.blob();
            set({ isLoading: false });
            return blob;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to generate commander reserve report',
                isLoading: false,
            });
            throw error;
        }
    },

    clearError: () => {
        set({ error: null });
    },
}));
