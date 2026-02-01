import { apiClient } from './apiClient';
import type {
    OperationalCustody,
    CreateCustodyRequest,
    ReturnCustodyRequest,
    ConsumeCustodyRequest,
    TransferCustodyRequest,
    CustodyByWorker,
    CustodyAgingReport,
    CustodyStatistics,
    PaginatedResponse,
    CustodyFilter,
} from '../types';

const BASE_URL = '/api/custody';

export const custodyService = {
    /**
     * Get paginated list of custodies
     */
    getCustodies: async (
        filter: CustodyFilter = {},
        pageNumber: number = 1,
        pageSize: number = 20
    ): Promise<PaginatedResponse<OperationalCustody>> => {
        const params = new URLSearchParams();
        params.append('pageNumber', pageNumber.toString());
        params.append('pageSize', pageSize.toString());

        if (filter.workerId) params.append('workerId', filter.workerId.toString());
        if (filter.itemId) params.append('itemId', filter.itemId.toString());
        if (filter.factoryId) params.append('factoryId', filter.factoryId.toString());
        if (filter.departmentId) params.append('departmentId', filter.departmentId.toString());
        if (filter.warehouseId) params.append('warehouseId', filter.warehouseId.toString());
        if (filter.status) params.append('status', filter.status);
        if (filter.isOverdue !== undefined) params.append('isOverdue', filter.isOverdue.toString());
        if (filter.startDate) params.append('startDate', filter.startDate);
        if (filter.endDate) params.append('endDate', filter.endDate);

        return apiClient.get<PaginatedResponse<OperationalCustody>>(`${BASE_URL}?${params.toString()}`);
    },

    /**
     * Get custody by ID
     */
    getCustodyById: async (id: number): Promise<OperationalCustody> => {
        return apiClient.get<OperationalCustody>(`${BASE_URL}/${id}`);
    },

    /**
     * Get custody by number
     */
    getCustodyByNumber: async (custodyNumber: string): Promise<OperationalCustody | null> => {
        try {
            return await apiClient.get<OperationalCustody>(`${BASE_URL}/number/${custodyNumber}`);
        } catch {
            return null;
        }
    },

    /**
     * Get active custodies for a worker
     */
    getActiveCustodiesByWorker: async (workerId: number): Promise<OperationalCustody[]> => {
        return apiClient.get<OperationalCustody[]>(`${BASE_URL}/worker/${workerId}/active`);
    },

    /**
     * Get overdue custodies
     */
    getOverdueCustodies: async (maxDays: number = 30, factoryId?: number): Promise<OperationalCustody[]> => {
        const params = new URLSearchParams();
        params.append('maxDays', maxDays.toString());
        if (factoryId) params.append('factoryId', factoryId.toString());
        return apiClient.get<OperationalCustody[]>(`${BASE_URL}/overdue?${params.toString()}`);
    },

    /**
     * Get custodies grouped by worker
     */
    getCustodiesByWorker: async (departmentId?: number, factoryId?: number): Promise<CustodyByWorker[]> => {
        const params = new URLSearchParams();
        if (departmentId) params.append('departmentId', departmentId.toString());
        if (factoryId) params.append('factoryId', factoryId.toString());
        const queryString = params.toString();
        return apiClient.get<CustodyByWorker[]>(`${BASE_URL}/by-worker${queryString ? '?' + queryString : ''}`);
    },

    /**
     * Get custody aging report for a worker
     */
    getCustodyAgingReport: async (workerId: number): Promise<CustodyAgingReport> => {
        return apiClient.get<CustodyAgingReport>(`${BASE_URL}/worker/${workerId}/aging`);
    },

    /**
     * Get custody statistics
     */
    getStatistics: async (factoryId?: number, departmentId?: number): Promise<CustodyStatistics> => {
        const params = new URLSearchParams();
        if (factoryId) params.append('factoryId', factoryId.toString());
        if (departmentId) params.append('departmentId', departmentId.toString());
        const queryString = params.toString();
        return apiClient.get<CustodyStatistics>(`${BASE_URL}/statistics${queryString ? '?' + queryString : ''}`);
    },

    /**
     * Validate custody limit for a worker/item
     */
    validateCustodyLimit: async (workerId: number, itemId: number, quantity: number): Promise<boolean> => {
        return apiClient.get<boolean>(`${BASE_URL}/validate-limit?workerId=${workerId}&itemId=${itemId}&quantity=${quantity}`);
    },

    /**
     * Get total custody quantity for a worker/item
     */
    getWorkerTotalCustodyQuantity: async (workerId: number, itemId: number): Promise<number> => {
        return apiClient.get<number>(`${BASE_URL}/worker/${workerId}/item/${itemId}/total`);
    },

    /**
     * Issue custody to a worker
     */
    issueCustody: async (request: CreateCustodyRequest): Promise<OperationalCustody> => {
        return apiClient.post<OperationalCustody>(`${BASE_URL}/issue`, request);
    },

    /**
     * Return custody
     */
    returnCustody: async (request: ReturnCustodyRequest): Promise<OperationalCustody> => {
        return apiClient.post<OperationalCustody>(`${BASE_URL}/${request.id}/return`, request);
    },

    /**
     * Consume custody
     */
    consumeCustody: async (request: ConsumeCustodyRequest): Promise<OperationalCustody> => {
        return apiClient.post<OperationalCustody>(`${BASE_URL}/${request.id}/consume`, request);
    },

    /**
     * Transfer custody to another worker
     */
    transferCustody: async (request: TransferCustodyRequest): Promise<OperationalCustody> => {
        return apiClient.post<OperationalCustody>(`${BASE_URL}/${request.id}/transfer`, request);
    },
};
