import { apiClient } from './apiClient';
import {
    Requisition,
    RequisitionItem,
    RequisitionFilter,
    PaginatedResponse,
} from '../types';

const BASE_URL = '/api/requisitions';

class RequisitionService {
    async getRequisitions(
        filter?: RequisitionFilter,
        pageNumber: number = 1,
        pageSize: number = 20
    ): Promise<PaginatedResponse<Requisition>> {
        const params = new URLSearchParams({
            pageNumber: pageNumber.toString(),
            pageSize: pageSize.toString(),
        });

        if (filter?.status) params.append('status', filter.status);
        if (filter?.priority) params.append('priority', filter.priority);
        if (filter?.warehouseId) params.append('warehouseId', filter.warehouseId.toString());
        if (filter?.departmentId) params.append('departmentId', filter.departmentId.toString());
        if (filter?.projectId) params.append('projectId', filter.projectId.toString());
        if (filter?.startDate) params.append('startDate', filter.startDate);
        if (filter?.endDate) params.append('endDate', filter.endDate);
        if (filter?.searchQuery) params.append('searchQuery', filter.searchQuery);

        return await apiClient.get<PaginatedResponse<Requisition>>(
            `${BASE_URL}?${params.toString()}`
        );
    }

    async getRequisitionById(id: number): Promise<Requisition> {
        return await apiClient.get<Requisition>(
            `${BASE_URL}/${id}`
        );
    }

    async createRequisition(data: {
        type: 'Internal' | 'External';
        priority: 'Low' | 'Medium' | 'High' | 'Critical';
        warehouseId: number;
        departmentId?: number;
        projectId?: number;
        items: Array<{
            itemId: number;
            requestedQuantity: number;
        }>;
        notes?: string;
    }): Promise<Requisition> {
        return await apiClient.post<Requisition>(
            BASE_URL,
            data
        );
    }

    async updateRequisition(
        id: number,
        data: {
            priority?: 'Low' | 'Medium' | 'High' | 'Critical';
            notes?: string;
        }
    ): Promise<Requisition> {
        return await apiClient.put<Requisition>(
            `${BASE_URL}/${id}`,
            data
        );
    }

    async approveRequisition(
        id: number,
        data: {
            items: Array<{
                itemId: number;
                approvedQuantity: number;
            }>;
            notes?: string;
        }
    ): Promise<Requisition> {
        return await apiClient.post<Requisition>(
            `${BASE_URL}/${id}/approve`,
            data
        );
    }

    async rejectRequisition(
        id: number,
        reason: string
    ): Promise<Requisition> {
        return await apiClient.post<Requisition>(
            `${BASE_URL}/${id}/reject`,
            { reason }
        );
    }

    async cancelRequisition(id: number): Promise<Requisition> {
        return await apiClient.post<Requisition>(
            `${BASE_URL}/${id}/cancel`
        );
    }

    async issueRequisitionItems(id: number): Promise<Requisition> {
        return await apiClient.post<Requisition>(
            `${BASE_URL}/${id}/issue`
        );
    }

    async getPendingRequisitions(): Promise<Requisition[]> {
        return await apiClient.get<Requisition[]>(
            `${BASE_URL}/pending`
        );
    }

    async getRequisitionsByDepartment(departmentId: number): Promise<Requisition[]> {
        // Fallback to main endpoint since proper endpoint might be missing
        // or ensure backend supports it. For now, matching original path.
        return await apiClient.get<Requisition[]>(
            `${BASE_URL}/department/${departmentId}`
        );
    }

    async getRequisitionsByProject(projectId: number): Promise<Requisition[]> {
        return await apiClient.get<Requisition[]>(
            `${BASE_URL}/project/${projectId}`
        );
    }

    async getRequisitionItems(requisitionId: number): Promise<RequisitionItem[]> {
        return await apiClient.get<RequisitionItem[]>(
            `${BASE_URL}/${requisitionId}/items`
        );
    }
}

export const requisitionService = new RequisitionService();
