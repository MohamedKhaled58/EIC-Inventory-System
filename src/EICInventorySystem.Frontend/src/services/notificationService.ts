import { apiClient } from './apiClient';
import {
    Notification,
    PaginatedResponse,
    ApiResponse,
} from '../types';

class NotificationService {
    async getNotifications(
        isRead?: boolean,
        pageNumber: number = 1,
        pageSize: number = 20
    ): Promise<PaginatedResponse<Notification>> {
        const params = new URLSearchParams({
            pageNumber: pageNumber.toString(),
            pageSize: pageSize.toString(),
        });

        if (isRead !== undefined) params.append('isRead', isRead.toString());

        const response = await apiClient.get<ApiResponse<PaginatedResponse<Notification>>>(
            `/notifications?${params.toString()}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get notifications');
    }

    async getNotificationById(id: number): Promise<Notification> {
        const response = await apiClient.get<ApiResponse<Notification>>(
            `/notifications/${id}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get notification');
    }

    async markAsRead(id: number): Promise<Notification> {
        const response = await apiClient.post<ApiResponse<Notification>>(
            `/notifications/${id}/read`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to mark notification as read');
    }

    async markAllAsRead(): Promise<void> {
        const response = await apiClient.post<ApiResponse<void>>(
            '/notifications/read-all'
        );

        if (!response.success) {
            throw new Error(response.message || 'Failed to mark all notifications as read');
        }
    }

    async deleteNotification(id: number): Promise<void> {
        const response = await apiClient.delete<ApiResponse<void>>(
            `/notifications/${id}`
        );

        if (!response.success) {
            throw new Error(response.message || 'Failed to delete notification');
        }
    }

    async getUnreadCount(): Promise<number> {
        const response = await apiClient.get<ApiResponse<{ count: number }>>(
            '/notifications/unread-count'
        );

        if (response.success && response.data) {
            return response.data.count;
        }

        throw new Error(response.message || 'Failed to get unread count');
    }

    async getRecentNotifications(limit: number = 10): Promise<Notification[]> {
        const response = await apiClient.get<ApiResponse<Notification[]>>(
            `/notifications/recent?limit=${limit}`
        );

        if (response.success && response.data) {
            return response.data;
        }

        throw new Error(response.message || 'Failed to get recent notifications');
    }
}

export const notificationService = new NotificationService();
