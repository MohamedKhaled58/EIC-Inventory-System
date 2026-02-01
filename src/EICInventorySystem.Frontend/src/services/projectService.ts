import { apiClient } from './apiClient';
import {
    Project,
    ProjectAllocation,
    ProjectFilter,
    PaginatedResponse,
    ApiResponse,
} from '../types';

class ProjectService {
    async getProjects(
        filter?: ProjectFilter,
        pageNumber: number = 1,
        pageSize: number = 20
    ): Promise<PaginatedResponse<Project>> {
        const params = new URLSearchParams({
            pageNumber: pageNumber.toString(),
            pageSize: pageSize.toString(),
        });

        if (filter?.status) params.append('status', filter.status);
        if (filter?.priority) params.append('priority', filter.priority);
        if (filter?.factoryId) params.append('factoryId', filter.factoryId.toString());
        if (filter?.departmentId) params.append('departmentId', filter.departmentId.toString());
        if (filter?.managerId) params.append('managerId', filter.managerId.toString());
        if (filter?.startDate) params.append('startDate', filter.startDate);
        if (filter?.endDate) params.append('endDate', filter.endDate);
        if (filter?.searchQuery) params.append('searchQuery', filter.searchQuery);

        const response = await apiClient.get<ApiResponse<PaginatedResponse<Project>>>(
            `/projects?${params.toString()}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get projects');
    }

    async getProjectById(id: number): Promise<Project> {
        const response = await apiClient.get<ApiResponse<Project>>(
            `/projects/${id}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get project');
    }

    async createProject(data: {
        name: string;
        nameAr: string;
        description?: string;
        descriptionAr?: string;
        factoryId: number;
        departmentId?: number;
        managerId: number;
        priority: 'Low' | 'Medium' | 'High' | 'Critical';
        startDate: string;
        plannedEndDate: string;
        budget: number;
    }): Promise<Project> {
        const response = await apiClient.post<ApiResponse<Project>>(
            '/projects',
            data
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to create project');
    }

    async updateProject(
        id: number,
        data: {
            name?: string;
            nameAr?: string;
            description?: string;
            descriptionAr?: string;
            status?: 'Planning' | 'Active' | 'OnHold' | 'Completed' | 'Cancelled';
            priority?: 'Low' | 'Medium' | 'High' | 'Critical';
            plannedEndDate?: string;
            budget?: number;
            progress?: number;
        }
    ): Promise<Project> {
        const response = await apiClient.put<ApiResponse<Project>>(
            `/projects/${id}`,
            data
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to update project');
    }

    async deleteProject(id: number): Promise<void> {
        const response = await apiClient.delete<ApiResponse<void>>(
            `/projects/${id}`
        );

        if (!response.success) {
            throw new Error(response.message || 'Failed to delete project');
        }
    }

    async getProjectAllocations(projectId: number): Promise<ProjectAllocation[]> {
        const response = await apiClient.get<ApiResponse<ProjectAllocation[]>>(
            `/projects/${projectId}/allocations`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get project allocations');
    }

    async allocateItemToProject(
        projectId: number,
        itemId: number,
        quantity: number
    ): Promise<ProjectAllocation> {
        const response = await apiClient.post<ApiResponse<ProjectAllocation>>(
            `/projects/${projectId}/allocations`,
            {
                itemId,
                quantity,
            }
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to allocate item to project');
    }

    async recordConsumption(
        projectId: number,
        itemId: number,
        quantity: number,
        notes?: string
    ): Promise<void> {
        const response = await apiClient.post<ApiResponse<void>>(
            `/projects/${projectId}/consumption`,
            {
                itemId,
                quantity,
                notes,
            }
        );

        if (!response.success) {
            throw new Error(response.message || 'Failed to record consumption');
        }
    }

    async getActiveProjects(): Promise<Project[]> {
        const response = await apiClient.get<ApiResponse<Project[]>>(
            '/projects/active'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get active projects');
    }

    async getProjectsByFactory(factoryId: number): Promise<Project[]> {
        const response = await apiClient.get<ApiResponse<Project[]>>(
            `/projects/factory/${factoryId}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get factory projects');
    }

    async getProjectsByDepartment(departmentId: number): Promise<Project[]> {
        const response = await apiClient.get<ApiResponse<Project[]>>(
            `/projects/department/${departmentId}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get department projects');
    }

    async getProjectsByManager(managerId: number): Promise<Project[]> {
        const response = await apiClient.get<ApiResponse<Project[]>>(
            `/projects/manager/${managerId}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get manager projects');
    }

    async getProjectSummary(): Promise<{
        totalProjects: number;
        activeProjects: number;
        completedProjects: number;
        onHoldProjects: number;
        totalBudget: number;
        spentAmount: number;
        averageProgress: number;
    }> {
        const response = await apiClient.get<ApiResponse<{
            totalProjects: number;
            activeProjects: number;
            completedProjects: number;
            onHoldProjects: number;
            totalBudget: number;
            spentAmount: number;
            averageProgress: number;
        }>>(
            '/projects/summary'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get project summary');
    }
}

export const projectService = new ProjectService();
