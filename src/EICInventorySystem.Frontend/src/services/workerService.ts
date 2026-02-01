import { apiClient } from './apiClient';
import type {
    Worker,
    CreateWorkerRequest,
    UpdateWorkerRequest,
    PaginatedResponse,
    WorkerFilter,
} from '../types';

const BASE_URL = '/api/worker';

export const workerService = {
    /**
     * Get paginated list of workers
     */
    getWorkers: async (
        filter: WorkerFilter = {},
        pageNumber: number = 1,
        pageSize: number = 20
    ): Promise<PaginatedResponse<Worker>> => {
        const params = new URLSearchParams();
        params.append('pageNumber', pageNumber.toString());
        params.append('pageSize', pageSize.toString());

        if (filter.factoryId) params.append('factoryId', filter.factoryId.toString());
        if (filter.departmentId) params.append('departmentId', filter.departmentId.toString());
        if (filter.isActive !== undefined) params.append('isActive', filter.isActive.toString());
        if (filter.searchTerm) params.append('searchTerm', filter.searchTerm);

        return apiClient.get<PaginatedResponse<Worker>>(`${BASE_URL}?${params.toString()}`);
    },

    /**
     * Get worker by ID
     */
    getWorkerById: async (id: number): Promise<Worker> => {
        return apiClient.get<Worker>(`${BASE_URL}/${id}`);
    },

    /**
     * Get worker by code
     */
    getWorkerByCode: async (code: string): Promise<Worker | null> => {
        try {
            return await apiClient.get<Worker>(`${BASE_URL}/code/${code}`);
        } catch {
            return null;
        }
    },

    /**
     * Search workers for autocomplete (Zero Manual Typing)
     */
    searchWorkers: async (
        searchTerm: string,
        factoryId?: number,
        departmentId?: number,
        maxResults: number = 10
    ): Promise<Worker[]> => {
        const params = new URLSearchParams();
        params.append('searchTerm', searchTerm);
        params.append('maxResults', maxResults.toString());
        if (factoryId) params.append('factoryId', factoryId.toString());
        if (departmentId) params.append('departmentId', departmentId.toString());

        return apiClient.get<Worker[]>(`${BASE_URL}/search?${params.toString()}`);
    },

    /**
     * Create a new worker
     */
    createWorker: async (request: CreateWorkerRequest): Promise<Worker> => {
        return apiClient.post<Worker>(BASE_URL, request);
    },

    /**
     * Update an existing worker
     */
    updateWorker: async (request: UpdateWorkerRequest): Promise<Worker> => {
        return apiClient.put<Worker>(`${BASE_URL}/${request.id}`, request);
    },

    /**
     * Activate a worker
     */
    activateWorker: async (id: number): Promise<void> => {
        return apiClient.post<void>(`${BASE_URL}/${id}/activate`);
    },

    /**
     * Deactivate a worker
     */
    deactivateWorker: async (id: number): Promise<void> => {
        return apiClient.post<void>(`${BASE_URL}/${id}/deactivate`);
    },

    /**
     * Transfer worker to another department
     */
    transferWorker: async (id: number, newDepartmentId: number): Promise<Worker> => {
        return apiClient.post<Worker>(`${BASE_URL}/${id}/transfer`, { newDepartmentId });
    },
};
