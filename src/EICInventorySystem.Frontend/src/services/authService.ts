import { apiClient } from './apiClient';
import {
    LoginRequest,
    LoginResponse,
    User,
    ApiResponse,
} from '../types';

class AuthService {
    private readonly TOKEN_KEY = 'token';
    private readonly USER_KEY = 'user';

    async login(credentials: LoginRequest): Promise<LoginResponse> {
        const response = await apiClient.post<LoginResponse>(
            '/api/Auth/login',
            credentials
        );

        if (response && response.token) {
            this.setToken(response.token);
            this.setUser(response.user);
            return response;
        }

        throw new Error('Login failed');
    }

    async logout(): Promise<void> {
        try {
            await apiClient.post('/api/Auth/logout');
        } catch (error) {
            // Ignore logout errors
        } finally {
            this.clearAuth();
        }
    }

    async getCurrentUser(): Promise<User> {
        const response = await apiClient.get<User>('/api/Auth/me');

        if (response) {
            this.setUser(response);
            return response;
        }

        throw new Error('Failed to get user');
    }

    async refreshToken(): Promise<string> {
        const response = await apiClient.post<{ token: string }>(
            '/api/Auth/refresh'
        );

        if (response && response.token) {
            this.setToken(response.token);
            return response.token;
        }

        throw new Error('Failed to refresh token');
    }

    getToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }

    setToken(token: string): void {
        localStorage.setItem(this.TOKEN_KEY, token);
    }

    getUser(): User | null {
        const userStr = localStorage.getItem(this.USER_KEY);
        return userStr ? JSON.parse(userStr) : null;
    }

    setUser(user: User): void {
        localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    }

    clearAuth(): void {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem(this.USER_KEY);
    }

    isAuthenticated(): boolean {
        return !!this.getToken();
    }

    hasRole(role: string): boolean {
        const user = this.getUser();
        return user?.role === role;
    }

    hasAnyRole(roles: string[]): boolean {
        const user = this.getUser();
        return user ? roles.includes(user.role) : false;
    }

    hasPermission(permission: string): boolean {
        const user = this.getUser();
        if (!user) return false;

        // Define role-based permissions
        const rolePermissions: Record<string, string[]> = {
            ComplexCommander: [
                'view_all_factories',
                'create_requisition',
                'approve_requisition',
                'approve_reserve',
                'issue_materials',
                'transfer_central_factory',
                'create_project',
                'manage_users',
                'view_reserve',
                'access_reserve',
                'adjust_inventory',
                'delete_transactions',
                'view_audit_trail',
                'view_inventory',
                'view_transfers',
                'view_reports',
                'manage_settings',
                'view_audit_log',
                'view_own_project',
            ],
            FactoryCommander: [
                'view_own_factory',
                'create_requisition',
                'approve_requisition',
                'approve_reserve',
                'issue_materials',
                'create_project',
                'manage_users',
                'view_reserve',
                'access_reserve',
                'adjust_inventory',
                'view_audit_trail',
                'view_inventory',
                'view_transfers',
                'view_reports',
                'view_audit_log',
            ],
            CentralWarehouseKeeper: [
                'view_own_warehouse',
                'create_receipt',
                'approve_transfer',
                'issue_materials',
                'view_reserve',
                'adjust_inventory',
                'view_audit_trail',
                'view_inventory',
                'view_transfers',
                'view_reports',
                'view_audit_log',
            ],
            FactoryWarehouseKeeper: [
                'view_own_warehouse',
                'create_transfer',
                'approve_requisition_general',
                'issue_materials',
                'view_reserve',
                'adjust_inventory',
                'view_audit_trail',
                'view_inventory',
                'view_transfers',
                'view_reports',
                'view_audit_log',
            ],
            DepartmentHead: [
                'view_own_department',
                'create_requisition',
                'view_reserve',
                'view_inventory',
                'view_reports',
            ],
            ProjectManager: [
                'view_own_project',
                'create_requisition',
                'update_project',
                'view_reserve',
                'view_inventory',
                'view_reports',
            ],
            Officer: [
                'view_factory',
                'create_requisition',
                'approve_requisition_general',
                'view_reserve',
                'view_inventory',
                'view_reports',
            ],
            CivilEngineer: [
                'view_project',
                'create_requisition',
                'view_reserve',
                'view_inventory',
                'view_reports',
            ],
            Worker: [
                'view_tasks',
                'create_consumption',
                'view_inventory',
                'view_reports',
            ],
            Auditor: [
                'view_all',
                'view_reserve',
                'export_reports',
                'view_audit_trail',
                'view_audit_log',
                'view_inventory',
                'view_transfers',
                'view_reports',
            ],
        };

        return rolePermissions[user.role]?.includes(permission) || false;
    }
}

export const authService = new AuthService();
