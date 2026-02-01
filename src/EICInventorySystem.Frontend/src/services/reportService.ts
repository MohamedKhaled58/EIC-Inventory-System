import { apiClient } from './apiClient';
import {
    InventoryReport,
    MovementReport,
    ApiResponse,
} from '../types';

class ReportService {
    async getInventoryReport(warehouseId?: number): Promise<InventoryReport> {
        const params = warehouseId ? `?warehouseId=${warehouseId}` : '';
        const response = await apiClient.get<ApiResponse<InventoryReport>>(
            `/reports/inventory${params}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get inventory report');
    }

    async getMovementReport(
        startDate: string,
        endDate: string,
        warehouseId?: number
    ): Promise<MovementReport> {
        const params = new URLSearchParams({
            startDate,
            endDate,
        });

        if (warehouseId) params.append('warehouseId', warehouseId.toString());

        const response = await apiClient.get<ApiResponse<MovementReport>>(
            `/reports/movement?${params.toString()}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get movement report');
    }

    async getProjectReport(projectId: number): Promise<{
        project: {
            id: number;
            name: string;
            nameAr: string;
            status: string;
            progress: number;
            budget: number;
            spentAmount: number;
        };
        allocations: Array<{
            itemId: number;
            itemName: string;
            itemNameAr: string;
            allocatedQuantity: number;
            consumedQuantity: number;
            remainingQuantity: number;
            unit: string;
            unitAr: string;
        }>;
        requisitions: Array<{
            id: number;
            requisitionNumber: string;
            status: string;
            totalItems: number;
            totalQuantity: number;
            requestedDate: string;
        }>;
    }> {
        const response = await apiClient.get<ApiResponse<{
            project: {
                id: number;
                name: string;
                nameAr: string;
                status: string;
                progress: number;
                budget: number;
                spentAmount: number;
            };
            allocations: Array<{
                itemId: number;
                itemName: string;
                itemNameAr: string;
                allocatedQuantity: number;
                consumedQuantity: number;
                remainingQuantity: number;
                unit: string;
                unitAr: string;
            }>;
            requisitions: Array<{
                id: number;
                requisitionNumber: string;
                status: string;
                totalItems: number;
                totalQuantity: number;
                requestedDate: string;
            }>;
        }>>(
            `/reports/project/${projectId}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get project report');
    }

    async getWarehouseReport(warehouseId: number): Promise<{
        warehouse: {
            id: number;
            name: string;
            nameAr: string;
            type: string;
        };
        summary: {
            totalItems: number;
            totalValue: number;
            lowStockItems: number;
            criticalStockItems: number;
            generalStockValue: number;
            reserveStockValue: number;
        };
        topItems: Array<{
            itemId: number;
            itemName: string;
            itemNameAr: string;
            totalQuantity: number;
            value: number;
        }>;
        recentTransactions: Array<{
            id: number;
            type: string;
            itemName: string;
            quantity: number;
            transactionDate: string;
        }>;
    }> {
        const response = await apiClient.get<ApiResponse<{
            warehouse: {
                id: number;
                name: string;
                nameAr: string;
                type: string;
            };
            summary: {
                totalItems: number;
                totalValue: number;
                lowStockItems: number;
                criticalStockItems: number;
                generalStockValue: number;
                reserveStockValue: number;
            };
            topItems: Array<{
                itemId: number;
                itemName: string;
                itemNameAr: string;
                totalQuantity: number;
                value: number;
            }>;
            recentTransactions: Array<{
                id: number;
                type: string;
                itemName: string;
                quantity: number;
                transactionDate: string;
            }>;
        }>>(
            `/reports/warehouse/${warehouseId}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get warehouse report');
    }

    async getCommanderReserveReport(factoryId?: number): Promise<{
        summary: {
            totalReserveValue: number;
            totalReserveItems: number;
            lowReserveCount: number;
            pendingRequests: number;
        };
        warehouses: Array<{
            warehouseId: number;
            warehouseName: string;
            warehouseNameAr: string;
            reserveValue: number;
            reserveItems: number;
            lowReserveItems: number;
        }>;
        recentRequests: Array<{
            id: number;
            itemName: string;
            itemNameAr: string;
            requestedQuantity: number;
            status: string;
            requestedDate: string;
        }>;
    }> {
        const params = factoryId ? `?factoryId=${factoryId}` : '';
        const response = await apiClient.get<ApiResponse<{
            summary: {
                totalReserveValue: number;
                totalReserveItems: number;
                lowReserveCount: number;
                pendingRequests: number;
            };
            warehouses: Array<{
                warehouseId: number;
                warehouseName: string;
                warehouseNameAr: string;
                reserveValue: number;
                reserveItems: number;
                lowReserveItems: number;
            }>;
            recentRequests: Array<{
                id: number;
                itemName: string;
                itemNameAr: string;
                requestedQuantity: number;
                status: string;
                requestedDate: string;
            }>;
        }>>(
            `/reports/commander-reserve${params}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get commander reserve report');
    }

    async getDashboardSummary(): Promise<{
        inventory: {
            totalItems: number;
            totalWarehouses: number;
            lowStockCount: number;
            criticalStockCount: number;
            totalValue: number;
        };
        requisitions: {
            totalRequisitions: number;
            pendingRequisitions: number;
            approvedToday: number;
            rejectedToday: number;
        };
        transfers: {
            totalTransfers: number;
            pendingTransfers: number;
            inTransitTransfers: number;
            completedToday: number;
        };
        projects: {
            totalProjects: number;
            activeProjects: number;
            completedProjects: number;
            averageProgress: number;
        };
        commanderReserve: {
            totalReserveValue: number;
            pendingRequests: number;
            lowReserveCount: number;
        };
    }> {
        const response = await apiClient.get<ApiResponse<{
            inventory: {
                totalItems: number;
                totalWarehouses: number;
                lowStockCount: number;
                criticalStockCount: number;
                totalValue: number;
            };
            requisitions: {
                totalRequisitions: number;
                pendingRequisitions: number;
                approvedToday: number;
                rejectedToday: number;
            };
            transfers: {
                totalTransfers: number;
                pendingTransfers: number;
                inTransitTransfers: number;
                completedToday: number;
            };
            projects: {
                totalProjects: number;
                activeProjects: number;
                completedProjects: number;
                averageProgress: number;
            };
            commanderReserve: {
                totalReserveValue: number;
                pendingRequests: number;
                lowReserveCount: number;
            };
        }>>(
            '/reports/dashboard-summary'
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get dashboard summary');
    }

    async exportReport(
        reportType: 'inventory' | 'movement' | 'project' | 'warehouse' | 'commander-reserve',
        format: 'pdf' | 'excel',
        filters?: Record<string, any>
    ): Promise<Blob> {
        const params = new URLSearchParams({
            reportType,
            format,
        });

        if (filters) {
            Object.entries(filters).forEach(([key, value]) => {
                if (value !== undefined && value !== null) {
                    params.append(key, String(value));
                }
            });
        }

        const response = await apiClient.get(`/reports/export?${params.toString()}`, {
            responseType: 'blob',
        }) as any;

        return response as Blob;
    }
}

export const reportService = new ReportService();
