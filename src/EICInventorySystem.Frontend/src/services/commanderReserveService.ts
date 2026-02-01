import { apiClient } from './apiClient';
import {
    CommanderReserveRequest,
    ReserveReleaseRequest,
    InventoryRecord,
    PaginatedResponse,
    ApiResponse,
} from '../types';

class CommanderReserveService {
    async getReserveRequests(
        status?: 'Pending' | 'Approved' | 'Rejected',
        pageNumber: number = 1,
        pageSize: number = 20
    ): Promise<PaginatedResponse<CommanderReserveRequest>> {
        const params = new URLSearchParams({
            pageNumber: pageNumber.toString(),
            pageSize: pageSize.toString(),
        });

        if (status) params.append('status', status);

        const response = await apiClient.get<ApiResponse<PaginatedResponse<CommanderReserveRequest>>>(
            `/commander-reserve/requests?${params.toString()}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get reserve requests');
    }

    async getReserveRequestById(id: number): Promise<CommanderReserveRequest> {
        const response = await apiClient.get<ApiResponse<CommanderReserveRequest>>(
            `/commander-reserve/requests/${id}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get reserve request');
    }

    async approveReserveRequest(
        id: number,
        notes?: string
    ): Promise<CommanderReserveRequest> {
        const response = await apiClient.post<ApiResponse<CommanderReserveRequest>>(
            `/commander-reserve/requests/${id}/approve`,
            { notes }
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to approve reserve request');
    }

    async rejectReserveRequest(
        id: number,
        reason: string
    ): Promise<CommanderReserveRequest> {
        const response = await apiClient.post<ApiResponse<CommanderReserveRequest>>(
            `/commander-reserve/requests/${id}/reject`,
            { reason }
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to reject reserve request');
    }

    async releaseReserve(request: ReserveReleaseRequest): Promise<void> {
        const response = await apiClient.post<ApiResponse<void>>(
            '/commander-reserve/release',
            request
        );

        if (!response.success) {
            throw new Error(response.message || 'Failed to release reserve');
        }
    }

    async getReserveInventory(warehouseId?: number): Promise<InventoryRecord[]> {
        const params = warehouseId ? `?warehouseId=${warehouseId}` : '';
        const response = await apiClient.get<ApiResponse<InventoryRecord[]>>(
            `/commander-reserve/inventory${params}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get reserve inventory');
    }

    async getLowReserveItems(threshold?: number): Promise<InventoryRecord[]> {
        const params = threshold ? `?threshold=${threshold}` : '';
        const response = await apiClient.get<ApiResponse<InventoryRecord[]>>(
            `/commander-reserve/low${params}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get low reserve items');
    }

    async getPendingReserveRequests(): Promise<CommanderReserveRequest[]> {
        const response = await apiClient.get<ApiResponse<CommanderReserveRequest[]>>(
            '/commander-reserve/requests/pending'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get pending reserve requests');
    }

    async getReserveSummary(): Promise<{
        totalReserveValue: number;
        totalReserveItems: number;
        lowReserveCount: number;
        pendingRequests: number;
        approvedThisMonth: number;
        rejectedThisMonth: number;
    }> {
        const response = await apiClient.get<ApiResponse<{
            totalReserveValue: number;
            totalReserveItems: number;
            lowReserveCount: number;
            pendingRequests: number;
            approvedThisMonth: number;
            rejectedThisMonth: number;
        }>>(
            '/commander-reserve/summary'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get reserve summary');
    }

    async getReserveByWarehouse(warehouseId: number): Promise<InventoryRecord[]> {
        const response = await apiClient.get<ApiResponse<InventoryRecord[]>>(
            `/commander-reserve/warehouse/${warehouseId}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get warehouse reserve');
    }

    async adjustReserve(
        warehouseId: number,
        itemId: number,
        quantity: number,
        reason: string
    ): Promise<void> {
        const response = await apiClient.post<ApiResponse<void>>(
            '/commander-reserve/adjust',
            {
                warehouseId,
                itemId,
                quantity,
                reason,
            }
        );

        if (!response.success) {
            throw new Error(response.message || 'Failed to adjust reserve');
        }
    }

    async getReserveHistory(
        itemId?: number,
        startDate?: string,
        endDate?: string
    ): Promise<CommanderReserveRequest[]> {
        const params = new URLSearchParams();
        if (itemId) params.append('itemId', itemId.toString());
        if (startDate) params.append('startDate', startDate);
        if (endDate) params.append('endDate', endDate);

        const response = await apiClient.get<ApiResponse<CommanderReserveRequest[]>>(
            `/commander-reserve/history?${params.toString()}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get reserve history');
    }
}

export const commanderReserveService = new CommanderReserveService();
