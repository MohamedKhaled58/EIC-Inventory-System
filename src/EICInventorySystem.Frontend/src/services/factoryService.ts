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

export interface Warehouse {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    type: 'Central' | 'Factory';
    location: string;
    isActive: boolean;
    factoryId?: number;
    factoryName?: string;
}

export interface CreateWarehouseRequest {
    name: string;
    nameAr: string;
    code: string;
    type: 'Central' | 'Factory';
    location: string;
    factoryId?: number;
}

export interface UpdateWarehouseRequest {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    type: 'Central' | 'Factory';
    location: string;
    isActive: boolean;
    factoryId?: number;
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

    async getWarehouses(factoryId?: number, type: 'Central' | 'Factory' | null = null, isActive?: boolean): Promise<Warehouse[]> {
        const params = new URLSearchParams();
        if (factoryId) params.append('factoryId', factoryId.toString());
        if (type) params.append('type', type);
        if (isActive !== undefined) params.append('isActive', isActive.toString());

        const response = await apiClient.get<Warehouse[] | { data: Warehouse[] }>(`/api/warehouses?${params.toString()}`);
        return Array.isArray(response) ? response : response.data || [];
    }

    async createWarehouse(data: CreateWarehouseRequest): Promise<Warehouse> {
        return await apiClient.post<Warehouse>('/api/warehouses', data);
    }

    async updateWarehouse(data: UpdateWarehouseRequest): Promise<Warehouse> {
        return await apiClient.put<Warehouse>(`/api/warehouses/${data.id}`, data);
    }
}

export const factoryService = new FactoryService();
