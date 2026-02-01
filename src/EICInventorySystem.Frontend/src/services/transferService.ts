import { apiClient } from './apiClient';
import {
  TransferRequest,
  TransferItem,
  PaginatedResponse,
  ApiResponse,
} from '../types';

class TransferService {
  async getTransfers(
    status?: TransferRequest['status'],
    type?: TransferRequest['type'],
    pageNumber: number = 1,
    pageSize: number = 20
  ): Promise<PaginatedResponse<TransferRequest>> {
    const params = new URLSearchParams({
      pageNumber: pageNumber.toString(),
      pageSize: pageSize.toString(),
    });

    if (status) params.append('status', status);
    if (type) params.append('type', type);

    const response = await apiClient.get<ApiResponse<PaginatedResponse<TransferRequest>>>(
      `/transfers?${params.toString()}`
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to get transfers');
  }

  async getTransferById(id: number): Promise<TransferRequest> {
    const response = await apiClient.get<ApiResponse<TransferRequest>>(
      `/transfers/${id}`
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to get transfer');
  }

  async createTransfer(data: {
    type: 'CentralToFactory' | 'FactoryToFactory' | 'WarehouseToWarehouse';
    priority: 'Low' | 'Medium' | 'High' | 'Critical';
    sourceWarehouseId: number;
    destinationWarehouseId: number;
    items: Array<{
      itemId: number;
      requestedQuantity: number;
    }>;
    notes?: string;
  }): Promise<TransferRequest> {
    const response = await apiClient.post<ApiResponse<TransferRequest>>(
      '/transfers',
      data
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to create transfer');
  }

  async updateTransfer(
    id: number,
    data: {
      priority?: 'Low' | 'Medium' | 'High' | 'Critical';
      notes?: string;
    }
  ): Promise<TransferRequest> {
    const response = await apiClient.put<ApiResponse<TransferRequest>>(
      `/transfers/${id}`,
      data
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to update transfer');
  }

  async approveTransfer(id: number, notes?: string): Promise<TransferRequest> {
    const response = await apiClient.post<ApiResponse<TransferRequest>>(
      `/transfers/${id}/approve`,
      { notes }
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to approve transfer');
  }

  async rejectTransfer(id: number, reason: string): Promise<TransferRequest> {
    const response = await apiClient.post<ApiResponse<TransferRequest>>(
      `/transfers/${id}/reject`,
      { reason }
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to reject transfer');
  }

  async cancelTransfer(id: number): Promise<TransferRequest> {
    const response = await apiClient.post<ApiResponse<TransferRequest>>(
      `/transfers/${id}/cancel`
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to cancel transfer');
  }

  async shipTransfer(id: number): Promise<TransferRequest> {
    const response = await apiClient.post<ApiResponse<TransferRequest>>(
      `/transfers/${id}/ship`
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to ship transfer');
  }

  async receiveTransfer(
    id: number,
    items: Array<{
      itemId: number;
      receivedQuantity: number;
    }>
  ): Promise<TransferRequest> {
    const response = await apiClient.post<ApiResponse<TransferRequest>>(
      `/transfers/${id}/receive`,
      { items }
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to receive transfer');
  }

  async getPendingTransfers(): Promise<TransferRequest[]> {
    const response = await apiClient.get<ApiResponse<TransferRequest[]>>(
      '/transfers/pending'
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to get pending transfers');
  }

  async getInTransitTransfers(): Promise<TransferRequest[]> {
    const response = await apiClient.get<ApiResponse<TransferRequest[]>>(
      '/transfers/in-transit'
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to get in-transit transfers');
  }

  async getTransfersByWarehouse(warehouseId: number): Promise<TransferRequest[]> {
    const response = await apiClient.get<ApiResponse<TransferRequest[]>>(
      `/transfers/warehouse/${warehouseId}`
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to get warehouse transfers');
  }

  async getTransferItems(transferId: number): Promise<TransferItem[]> {
    const response = await apiClient.get<ApiResponse<TransferItem[]>>(
      `/transfers/${transferId}/items`
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to get transfer items');
  }

  async getTransferSummary(): Promise<{
    totalTransfers: number;
    pendingTransfers: number;
    inTransitTransfers: number;
    completedTransfers: number;
    cancelledTransfers: number;
  }> {
    const response = await apiClient.get<ApiResponse<{
      totalTransfers: number;
      pendingTransfers: number;
      inTransitTransfers: number;
      completedTransfers: number;
      cancelledTransfers: number;
    }>>(
      '/transfers/summary'
    );

    if (response.success && response.data) {
      return response.data;
    }

    throw new Error(response.message || 'Failed to get transfer summary');
  }
}

export const transferService = new TransferService();
