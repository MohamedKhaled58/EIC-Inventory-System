import { apiClient } from './apiClient';
import type {
    ProjectBOQ,
    CreateProjectBOQRequest,
    UpdateProjectBOQRequest,
    ApproveBOQRequest,
    RejectBOQRequest,
    IssueBOQRequest,
    BOQStatistics,
    PendingBOQItems,
    PaginatedResponse,
    BOQFilter,
} from '../types';

const BASE_URL = '/api/projectboq';

export const projectBOQService = {
    /**
     * Get paginated list of BOQs
     */
    getBOQs: async (
        filter: BOQFilter = {},
        pageNumber: number = 1,
        pageSize: number = 20
    ): Promise<PaginatedResponse<ProjectBOQ>> => {
        const params = new URLSearchParams();
        params.append('pageNumber', pageNumber.toString());
        params.append('pageSize', pageSize.toString());

        if (filter.factoryId) params.append('factoryId', filter.factoryId.toString());
        if (filter.projectId) params.append('projectId', filter.projectId.toString());
        if (filter.warehouseId) params.append('warehouseId', filter.warehouseId.toString());
        if (filter.status) params.append('status', filter.status);
        if (filter.startDate) params.append('startDate', filter.startDate);
        if (filter.endDate) params.append('endDate', filter.endDate);

        return apiClient.get<PaginatedResponse<ProjectBOQ>>(`${BASE_URL}?${params.toString()}`);
    },

    /**
     * Get BOQ by ID
     */
    getBOQById: async (id: number): Promise<ProjectBOQ> => {
        return apiClient.get<ProjectBOQ>(`${BASE_URL}/${id}`);
    },

    /**
     * Get BOQ by number
     */
    getBOQByNumber: async (boqNumber: string): Promise<ProjectBOQ | null> => {
        try {
            return await apiClient.get<ProjectBOQ>(`${BASE_URL}/number/${boqNumber}`);
        } catch {
            return null;
        }
    },

    /**
     * Get pending BOQs
     */
    getPendingBOQs: async (factoryId?: number): Promise<ProjectBOQ[]> => {
        const params = factoryId ? `?factoryId=${factoryId}` : '';
        return apiClient.get<ProjectBOQ[]>(`${BASE_URL}/pending${params}`);
    },

    /**
     * Get BOQs pending approval
     */
    getBOQsForApproval: async (factoryId?: number): Promise<ProjectBOQ[]> => {
        const params = factoryId ? `?factoryId=${factoryId}` : '';
        return apiClient.get<ProjectBOQ[]>(`${BASE_URL}/for-approval${params}`);
    },

    /**
     * Get BOQs ready for issuance
     */
    getBOQsForIssuance: async (warehouseId?: number): Promise<ProjectBOQ[]> => {
        const params = warehouseId ? `?warehouseId=${warehouseId}` : '';
        return apiClient.get<ProjectBOQ[]>(`${BASE_URL}/for-issuance${params}`);
    },

    /**
     * Get partially issued BOQs
     */
    getPartiallyIssuedBOQs: async (projectId?: number): Promise<ProjectBOQ[]> => {
        const params = projectId ? `?projectId=${projectId}` : '';
        return apiClient.get<ProjectBOQ[]>(`${BASE_URL}/partially-issued${params}`);
    },

    /**
     * Get pending items across BOQs
     */
    getPendingItems: async (projectId?: number, factoryId?: number): Promise<PendingBOQItems[]> => {
        const params = new URLSearchParams();
        if (projectId) params.append('projectId', projectId.toString());
        if (factoryId) params.append('factoryId', factoryId.toString());
        const queryString = params.toString();
        return apiClient.get<PendingBOQItems[]>(`${BASE_URL}/pending-items${queryString ? '?' + queryString : ''}`);
    },

    /**
     * Get BOQ statistics
     */
    getStatistics: async (factoryId?: number, startDate?: string, endDate?: string): Promise<BOQStatistics> => {
        const params = new URLSearchParams();
        if (factoryId) params.append('factoryId', factoryId.toString());
        if (startDate) params.append('startDate', startDate);
        if (endDate) params.append('endDate', endDate);
        const queryString = params.toString();
        return apiClient.get<BOQStatistics>(`${BASE_URL}/statistics${queryString ? '?' + queryString : ''}`);
    },

    /**
     * Get available stock for an item in a warehouse
     */
    getAvailableStock: async (itemId: number, warehouseId: number): Promise<number> => {
        return apiClient.get<number>(`${BASE_URL}/available-stock?itemId=${itemId}&warehouseId=${warehouseId}`);
    },

    /**
     * Create a new BOQ
     */
    createBOQ: async (request: CreateProjectBOQRequest): Promise<ProjectBOQ> => {
        return apiClient.post<ProjectBOQ>(BASE_URL, request);
    },

    /**
     * Update an existing BOQ
     */
    updateBOQ: async (request: UpdateProjectBOQRequest): Promise<ProjectBOQ> => {
        return apiClient.put<ProjectBOQ>(`${BASE_URL}/${request.id}`, request);
    },

    /**
     * Delete a BOQ (draft only)
     */
    deleteBOQ: async (id: number): Promise<void> => {
        return apiClient.delete<void>(`${BASE_URL}/${id}`);
    },

    /**
     * Submit BOQ for approval
     */
    submitBOQ: async (id: number): Promise<ProjectBOQ> => {
        return apiClient.post<ProjectBOQ>(`${BASE_URL}/${id}/submit`);
    },

    /**
     * Approve a BOQ
     */
    approveBOQ: async (request: ApproveBOQRequest): Promise<ProjectBOQ> => {
        return apiClient.post<ProjectBOQ>(`${BASE_URL}/${request.id}/approve`, request);
    },

    /**
     * Reject a BOQ
     */
    rejectBOQ: async (request: RejectBOQRequest): Promise<ProjectBOQ> => {
        return apiClient.post<ProjectBOQ>(`${BASE_URL}/${request.id}/reject`, request);
    },

    /**
     * Approve commander reserve for a BOQ
     */
    approveCommanderReserve: async (id: number, approvalNotes?: string, approvalNotesArabic?: string): Promise<ProjectBOQ> => {
        return apiClient.post<ProjectBOQ>(`${BASE_URL}/${id}/approve-commander-reserve`, {
            id,
            approvalNotes,
            approvalNotesArabic,
        });
    },

    /**
     * Issue a BOQ
     */
    issueBOQ: async (request: IssueBOQRequest): Promise<ProjectBOQ> => {
        return apiClient.post<ProjectBOQ>(`${BASE_URL}/${request.id}/issue`, request);
    },

    /**
     * Cancel a BOQ
     */
    cancelBOQ: async (id: number, reason?: string, reasonArabic?: string): Promise<void> => {
        return apiClient.post<void>(`${BASE_URL}/${id}/cancel`, { reason, reasonArabic });
    },
};
