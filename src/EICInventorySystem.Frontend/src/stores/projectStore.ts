import { create } from 'zustand';
import { Project, ProjectStatus } from '../types';

interface ProjectState {
    projects: Project[];
    isLoading: boolean;
    error: string | null;

    fetchProjects: (status?: ProjectStatus) => Promise<void>;
    fetchProject: (id: number) => Promise<Project>;
    createProject: (project: Partial<Project>) => Promise<Project>;
    updateProject: (id: number, project: Partial<Project>) => Promise<void>;
    deleteProject: (id: number) => Promise<void>;
    clearError: () => void;
}

export const useProjectStore = create<ProjectState>((set, get) => ({
    projects: [],
    isLoading: false,
    error: null,

    fetchProjects: async (status?: ProjectStatus) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const url = status
                ? `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/projects?status=${status}`
                : `${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/projects`;

            const response = await fetch(url, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch projects');

            const data = await response.json();
            set({ projects: data, isLoading: false });
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch projects',
                isLoading: false,
            });
        }
    },

    fetchProject: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/projects/${id}`, {
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to fetch project');

            const data = await response.json();
            set({ isLoading: false });
            return data;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to fetch project',
                isLoading: false,
            });
            throw error;
        }
    },

    createProject: async (project: Partial<Project>) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/projects`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify(project),
            });

            if (!response.ok) throw new Error('Failed to create project');

            const data = await response.json();
            set((state) => ({ projects: [...state.projects, data], isLoading: false }));
            return data;
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to create project',
                isLoading: false,
            });
            throw error;
        }
    },

    updateProject: async (id: number, project: Partial<Project>) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/projects/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
                body: JSON.stringify(project),
            });

            if (!response.ok) throw new Error('Failed to update project');

            const data = await response.json();
            set((state) => ({
                projects: state.projects.map((p) => (p.id === id ? data : p)),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to update project',
                isLoading: false,
            });
            throw error;
        }
    },

    deleteProject: async (id: number) => {
        set({ isLoading: true, error: null });
        try {
            const token = localStorage.getItem('auth-storage');
            const auth = token ? JSON.parse(token) : null;

            const response = await fetch(`${import.meta.env.VITE_API_BASE_URL || 'https://localhost:5001/api'}/projects/${id}`, {
                method: 'DELETE',
                headers: {
                    Authorization: `Bearer ${auth?.state?.token}`,
                },
            });

            if (!response.ok) throw new Error('Failed to delete project');

            set((state) => ({
                projects: state.projects.filter((p) => p.id !== id),
                isLoading: false,
            }));
        } catch (error) {
            set({
                error: error instanceof Error ? error.message : 'Failed to delete project',
                isLoading: false,
            });
            throw error;
        }
    },

    clearError: () => {
        set({ error: null });
    },
}));
