import { apiClient } from './apiClient';

export interface Factory {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    location: string;
    isActive: boolean;
}

export interface Department {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    factoryId: number;
    factoryName: string;
}

export interface CreateFactoryRequest {
    name: string;
    nameAr: string;
    code: string;
    location: string;
}

export interface UpdateFactoryRequest {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    location: string;
    isActive: boolean;
}

export interface CreateDepartmentRequest {
    name: string;
    nameAr: string;
    code: string;
    factoryId: number;
}

export interface UpdateDepartmentRequest {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    factoryId: number;
}

class FactoryService {
    async getFactories(isActive?: boolean): Promise<Factory[]> {
        const params = new URLSearchParams();
        if (isActive !== undefined) params.append('isActive', isActive.toString());
        const response = await apiClient.get<Factory[] | { data: Factory[] }>(`/api/factories?${params.toString()}`);
        return Array.isArray(response) ? response : response.data || [];
    }

    async getFactory(id: number): Promise<Factory> {
        return await apiClient.get<Factory>(`/api/factories/${id}`);
    }

    async createFactory(data: CreateFactoryRequest): Promise<Factory> {
        return await apiClient.post<Factory>('/api/factories', data);
    }

    async updateFactory(data: UpdateFactoryRequest): Promise<Factory> {
        return await apiClient.put<Factory>(`/api/factories/${data.id}`, data);
    }

    async getDepartments(factoryId?: number): Promise<Department[]> {
        const params = new URLSearchParams();
        if (factoryId) params.append('factoryId', factoryId.toString());
        const response = await apiClient.get<Department[] | { data: Department[] }>(`/api/departments?${params.toString()}`);
        return Array.isArray(response) ? response : response.data || [];
    }

    async getDepartment(id: number): Promise<Department> {
        return await apiClient.get<Department>(`/api/departments/${id}`);
    }

    async createDepartment(data: CreateDepartmentRequest): Promise<Department> {
        return await apiClient.post<Department>('/api/departments', data);
    }

    async updateDepartment(data: UpdateDepartmentRequest): Promise<Department> {
        return await apiClient.put<Department>(`/api/departments/${data.id}`, data);
    }
}

export const factoryService = new FactoryService();
