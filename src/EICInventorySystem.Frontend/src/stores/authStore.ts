import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface User {
    id: number;
    userId?: number;
    username: string;
    name?: string;
    email: string;
    fullName: string;
    fullNameAr: string;
    role: string;
    factoryId?: number;
    factoryName?: string;
    departmentId?: number;
    warehouseId?: number;
    permissions: string[];
}

interface AuthState {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    error: string | null;

    login: (username: string, password: string) => Promise<void>;
    logout: () => void;
    refreshToken: () => Promise<void>;
    hasPermission: (permission: string) => boolean;
    hasRole: (role: string) => boolean;
    clearError: () => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set, get) => ({
            user: null,
            token: null,
            isAuthenticated: false,
            isLoading: false,
            error: null,

            login: async (username: string, password: string) => {
                set({ isLoading: true, error: null });
                try {
                    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'}/api/Auth/login`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ username, password }),
                    });

                    if (!response.ok) {
                        const error = await response.json();
                        throw new Error(error.message || 'Login failed');
                    }

                    const data = await response.json();
                    set({
                        user: data.user,
                        token: data.token,
                        isAuthenticated: true,
                        isLoading: false,
                        error: null,
                    });
                } catch (error) {
                    set({
                        error: error instanceof Error ? error.message : 'Login failed',
                        isLoading: false,
                    });
                    throw error;
                }
            },

            logout: () => {
                set({
                    user: null,
                    token: null,
                    isAuthenticated: false,
                    error: null,
                });
            },

            refreshToken: async () => {
                try {
                    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'}/api/Auth/refresh`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            Authorization: `Bearer ${get().token}`,
                        },
                    });

                    if (!response.ok) {
                        throw new Error('Token refresh failed');
                    }

                    const data = await response.json();
                    set({
                        token: data.token,
                        user: data.user,
                    });
                } catch (error) {
                    get().logout();
                    throw error;
                }
            },

            hasPermission: (permission: string) => {
                const { user } = get();
                return user?.permissions.includes(permission) || false;
            },

            hasRole: (role: string) => {
                const { user } = get();
                return user?.role === role || false;
            },

            clearError: () => {
                set({ error: null });
            },
        }),
        {
            name: 'auth-storage',
            partialize: (state) => ({
                user: state.user,
                token: state.token,
                isAuthenticated: state.isAuthenticated,
            }),
        }
    )
);
