import { apiClient } from './apiClient';
import {
    InventoryRecord,
    Warehouse,
    Item,
    InventoryFilter,
    PaginatedResponse,
    ApiResponse,
} from '../types';

class InventoryService {
    async getInventory(
        filter?: InventoryFilter,
        pageNumber: number = 1,
        pageSize: number = 20
    ): Promise<PaginatedResponse<InventoryRecord>> {
        const params = new URLSearchParams({
            pageNumber: pageNumber.toString(),
            pageSize: pageSize.toString(),
        });

        if (filter?.warehouseId) params.append('warehouseId', filter.warehouseId.toString());
        if (filter?.itemId) params.append('itemId', filter.itemId.toString());
        if (filter?.status) params.append('status', filter.status);
        if (filter?.searchQuery) params.append('searchQuery', filter.searchQuery);

        const response = await apiClient.get<ApiResponse<PaginatedResponse<InventoryRecord>>>(
            `/inventory?${params.toString()}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get inventory');
    }

    async getInventoryById(id: number): Promise<InventoryRecord> {
        const response = await apiClient.get<ApiResponse<InventoryRecord>>(
            `/inventory/${id}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get inventory record');
    }

    async getWarehouses(): Promise<Warehouse[]> {
        const response = await apiClient.get<ApiResponse<Warehouse[]>>(
            '/warehouses'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get warehouses');
    }

    async getWarehouseById(id: number): Promise<Warehouse> {
        const response = await apiClient.get<ApiResponse<Warehouse>>(
            `/warehouses/${id}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get warehouse');
    }

    async getItems(): Promise<Item[]> {
        const response = await apiClient.get<ApiResponse<Item[]>>(
            '/items'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get items');
    }

    async getItemById(id: number): Promise<Item> {
        const response = await apiClient.get<ApiResponse<Item>>(
            `/items/${id}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get item');
    }

    async searchItems(query: string, warehouseId?: number): Promise<Item[]> {
        const params = new URLSearchParams({ query: encodeURIComponent(query) });
        if (warehouseId) {
            params.append('warehouseId', warehouseId.toString());
        }

        const response = await apiClient.get<ApiResponse<Item[]>>(
            `/items/search?${params.toString()}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to search items');
    }

    async adjustInventory(
        warehouseId: number,
        itemId: number,
        quantity: number,
        reason: string
    ): Promise<void> {
        const response = await apiClient.post<ApiResponse<void>>(
            '/inventory/adjust',
            {
                warehouseId,
                itemId,
                quantity,
                reason,
            }
        );

        if (!response.success) {
            throw new Error(response.message || 'Failed to adjust inventory');
        }
    }

    async getLowStockItems(threshold?: number): Promise<InventoryRecord[]> {
        const params = threshold ? `?threshold=${threshold}` : '';
        const response = await apiClient.get<ApiResponse<InventoryRecord[]>>(
            `/inventory/low-stock${params}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get low stock items');
    }

    async getCriticalStockItems(): Promise<InventoryRecord[]> {
        const response = await apiClient.get<ApiResponse<InventoryRecord[]>>(
            '/inventory/critical-stock'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get critical stock items');
    }

    async getInventoryByWarehouse(warehouseId: number): Promise<InventoryRecord[]> {
        const response = await apiClient.get<ApiResponse<InventoryRecord[]>>(
            `/inventory/warehouse/${warehouseId}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get warehouse inventory');
    }

    async getInventorySummary(): Promise<{
        totalItems: number;
        totalWarehouses: number;
        lowStockCount: number;
        criticalStockCount: number;
        totalValue: number;
    }> {
        const response = await apiClient.get<ApiResponse<{
            totalItems: number;
            totalWarehouses: number;
            lowStockCount: number;
            criticalStockCount: number;
            totalValue: number;
        }>>(
            '/inventory/summary'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get inventory summary');
    }
}

export const inventoryService = new InventoryService();
