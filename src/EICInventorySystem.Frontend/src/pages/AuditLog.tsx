import React, { useEffect, useState } from 'react';
import {
    FileText,
    Search,
    Filter,
    Download,
    Eye,
    ChevronDown,
    ChevronUp,
    Calendar,
    User,
    Shield,
    AlertTriangle,
    CheckCircle,
    XCircle,
    Clock,
    RefreshCw,
    ArrowLeft,
    ArrowRight
} from 'lucide-react';
import { useAuthStore } from '../stores/authStore';

interface AuditLogEntry {
    id: string;
    timestamp: string;
    userId: string;
    userName: string;
    userRole: string;
    action: string;
    entityType: string;
    entityId: string;
    entityName: string;
    oldValue?: string;
    newValue?: string;
    ipAddress: string;
    userAgent: string;
    success: boolean;
    errorMessage?: string;
    severity: 'info' | 'warning' | 'error' | 'critical';
}

interface FilterState {
    dateRange: {
        start: string;
        end: string;
    };
    userId?: string;
    action?: string;
    entityType?: string;
    severity?: string;
    success?: boolean;
}

const AuditLog: React.FC = () => {
    const { user } = useAuthStore();
    const [loading, setLoading] = useState(true);
    const [logs, setLogs] = useState<AuditLogEntry[]>([]);
    const [filteredLogs, setFilteredLogs] = useState<AuditLogEntry[]>([]);
    const [showFilters, setShowFilters] = useState(false);
    const [selectedLog, setSelectedLog] = useState<AuditLogEntry | null>(null);
    const [showDetails, setShowDetails] = useState(false);
    const [filters, setFilters] = useState<FilterState>({
        dateRange: {
            start: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
            end: new Date().toISOString().split('T')[0],
        },
    });
    const [searchTerm, setSearchTerm] = useState('');
    const [sortField, setSortField] = useState<keyof AuditLogEntry>('timestamp');
    const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc');
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(20);

    useEffect(() => {
        loadAuditLogs();
    }, []);

    useEffect(() => {
        applyFilters();
    }, [logs, filters, searchTerm]);

    const loadAuditLogs = async () => {
        try {
            setLoading(true);
            // TODO: Replace with actual API call
            // const response = await api.get('/api/audit-logs', { params: filters });

            // Mock data
            const mockLogs: AuditLogEntry[] = [
                {
                    id: '1',
                    timestamp: new Date().toISOString(),
                    userId: '1',
                    userName: 'أحمد محمد',
                    userRole: 'FactoryCommander',
                    action: 'CreateRequisition',
                    entityType: 'Requisition',
                    entityId: 'REQ-2025-001',
                    entityName: 'طلب صرف فولاذ',
                    newValue: '{"quantity": 100, "itemId": "ITM-001"}',
                    ipAddress: '192.168.1.100',
                    userAgent: 'Mozilla/5.0...',
                    success: true,
                    severity: 'info',
                },
                {
                    id: '2',
                    timestamp: new Date(Date.now() - 3600000).toISOString(),
                    userId: '2',
                    userName: 'محمد علي',
                    userRole: 'WarehouseKeeper',
                    action: 'ApproveRequisition',
                    entityType: 'Requisition',
                    entityId: 'REQ-2025-001',
                    entityName: 'طلب صرف فولاذ',
                    oldValue: '{"status": "Pending"}',
                    newValue: '{"status": "Approved"}',
                    ipAddress: '192.168.1.101',
                    userAgent: 'Mozilla/5.0...',
                    success: true,
                    severity: 'info',
                },
                {
                    id: '3',
                    timestamp: new Date(Date.now() - 7200000).toISOString(),
                    userId: '3',
                    userName: 'خالد أحمد',
                    userRole: 'DepartmentHead',
                    action: 'AccessCommanderReserve',
                    entityType: 'Inventory',
                    entityId: 'INV-001',
                    entityName: 'مخزون الفولاذ',
                    errorMessage: 'Unauthorized access attempt',
                    ipAddress: '192.168.1.102',
                    userAgent: 'Mozilla/5.0...',
                    success: false,
                    severity: 'critical',
                },
            ];

            setLogs(mockLogs);
        } catch (error) {
            console.error('Error loading audit logs:', error);
        } finally {
            setLoading(false);
        }
    };

    const applyFilters = () => {
        let filtered = [...logs];

        // Date range filter
        if (filters.dateRange.start && filters.dateRange.end) {
            filtered = filtered.filter(log => {
                const logDate = new Date(log.timestamp);
                const startDate = new Date(filters.dateRange.start);
                const endDate = new Date(filters.dateRange.end);
                endDate.setHours(23, 59, 59, 999);
                return logDate >= startDate && logDate <= endDate;
            });
        }

        // User filter
        if (filters.userId) {
            filtered = filtered.filter(log => log.userId === filters.userId);
        }

        // Action filter
        if (filters.action) {
            filtered = filtered.filter(log => log.action === filters.action);
        }

        // Entity type filter
        if (filters.entityType) {
            filtered = filtered.filter(log => log.entityType === filters.entityType);
        }

        // Severity filter
        if (filters.severity) {
            filtered = filtered.filter(log => log.severity === filters.severity);
        }

        // Success filter
        if (filters.success !== undefined) {
            filtered = filtered.filter(log => log.success === filters.success);
        }

        // Search term
        if (searchTerm) {
            const term = searchTerm.toLowerCase();
            filtered = filtered.filter(log =>
                log.userName.toLowerCase().includes(term) ||
                log.action.toLowerCase().includes(term) ||
                log.entityName.toLowerCase().includes(term) ||
                log.entityId.toLowerCase().includes(term)
            );
        }

        // Sort
        filtered.sort((a, b) => {
            const aValue = a[sortField] ?? '';
            const bValue = b[sortField] ?? '';

            if (aValue < bValue) return sortDirection === 'asc' ? -1 : 1;
            if (aValue > bValue) return sortDirection === 'asc' ? 1 : -1;
            return 0;
        });

        setFilteredLogs(filtered);
        setCurrentPage(1);
    };

    const handleSort = (field: keyof AuditLogEntry) => {
        if (sortField === field) {
            setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
        } else {
            setSortField(field);
            setSortDirection('asc');
        }
    };

    const handleExport = async () => {
        try {
            // TODO: Replace with actual API call
            // const response = await api.get('/api/audit-logs/export', { params: filters });
            // const blob = new Blob([response.data], { type: 'text/csv' });
            // const url = window.URL.createObjectURL(blob);
            // const a = document.createElement('a');
            // a.href = url;
            // a.download = `audit-logs-${new Date().toISOString()}.csv`;
            // a.click();

            console.log('Exporting audit logs...');
        } catch (error) {
            console.error('Error exporting audit logs:', error);
        }
    };

    const getSeverityIcon = (severity: string) => {
        switch (severity) {
            case 'critical':
                return <AlertTriangle size={16} className="text-red-600" />;
            case 'error':
                return <XCircle size={16} className="text-red-500" />;
            case 'warning':
                return <AlertTriangle size={16} className="text-yellow-600" />;
            case 'info':
            default:
                return <CheckCircle size={16} className="text-blue-600" />;
        }
    };

    const getSeverityBadge = (severity: string) => {
        const styles = {
            critical: 'bg-red-100 text-red-800',
            error: 'bg-red-50 text-red-700',
            warning: 'bg-yellow-100 text-yellow-800',
            info: 'bg-blue-100 text-blue-800',
        };
        return styles[severity as keyof typeof styles] || styles.info;
    };

    const getActionLabel = (action: string) => {
        const labels: Record<string, string> = {
            CreateRequisition: 'إنشاء طلب صرف',
            ApproveRequisition: 'موافقة على طلب صرف',
            RejectRequisition: 'رفض طلب صرف',
            CreateTransfer: 'إنشاء طلب تحويل',
            ApproveTransfer: 'موافقة على تحويل',
            IssueMaterial: 'صرف مادة',
            ReceiveMaterial: 'استلام مادة',
            AdjustInventory: 'تعديل المخزون',
            AccessCommanderReserve: 'محاولة الوصول لاحتياطي القائد',
            ReleaseCommanderReserve: 'إطلاق احتياطي القائد',
            CreateUser: 'إنشاء مستخدم',
            UpdateUser: 'تحديث مستخدم',
            DeleteUser: 'حذف مستخدم',
            Login: 'تسجيل دخول',
            Logout: 'تسجيل خروج',
        };
        return labels[action] || action;
    };

    const totalPages = Math.ceil(filteredLogs.length / itemsPerPage);
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const currentLogs = filteredLogs.slice(startIndex, endIndex);

    if (loading) {
        return (
            <div className="flex items-center justify-center h-64">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold text-gray-900">سجل التدقيق</h1>
                    <p className="text-gray-600 mt-1">Audit Log</p>
                </div>
                <div className="flex items-center space-x-3 space-x-reverse">
                    <button
                        className="flex items-center px-4 py-2 bg-white border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
                        onClick={() => setShowFilters(!showFilters)}
                    >
                        <Filter size={20} className="mr-2" />
                        الفلاتر
                        {showFilters ? <ChevronUp size={20} className="mr-2" /> : <ChevronDown size={20} className="mr-2" />}
                    </button>
                    <button
                        className="flex items-center px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
                        onClick={handleExport}
                    >
                        <Download size={20} className="mr-2" />
                        تصدير
                    </button>
                    <button
                        className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                        onClick={loadAuditLogs}
                    >
                        <RefreshCw size={20} className="mr-2" />
                        تحديث
                    </button>
                </div>
            </div>

            {/* Filters */}
            {showFilters && (
                <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">الفلاتر</h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                من تاريخ
                            </label>
                            <input
                                type="date"
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                value={filters.dateRange.start}
                                onChange={(e) => setFilters({ ...filters, dateRange: { ...filters.dateRange, start: e.target.value } })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                إلى تاريخ
                            </label>
                            <input
                                type="date"
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                value={filters.dateRange.end}
                                onChange={(e) => setFilters({ ...filters, dateRange: { ...filters.dateRange, end: e.target.value } })}
                            />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                الإجراء
                            </label>
                            <select
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                value={filters.action || ''}
                                onChange={(e) => setFilters({ ...filters, action: e.target.value || undefined })}
                            >
                                <option value="">الكل</option>
                                <option value="CreateRequisition">إنشاء طلب صرف</option>
                                <option value="ApproveRequisition">موافقة على طلب صرف</option>
                                <option value="CreateTransfer">إنشاء طلب تحويل</option>
                                <option value="IssueMaterial">صرف مادة</option>
                                <option value="ReceiveMaterial">استلام مادة</option>
                                <option value="AccessCommanderReserve">الوصول لاحتياطي القائد</option>
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                نوع الكيان
                            </label>
                            <select
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                value={filters.entityType || ''}
                                onChange={(e) => setFilters({ ...filters, entityType: e.target.value || undefined })}
                            >
                                <option value="">الكل</option>
                                <option value="Requisition">طلب صرف</option>
                                <option value="Transfer">طلب تحويل</option>
                                <option value="Inventory">مخزون</option>
                                <option value="User">مستخدم</option>
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                الخطورة
                            </label>
                            <select
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                value={filters.severity || ''}
                                onChange={(e) => setFilters({ ...filters, severity: e.target.value || undefined })}
                            >
                                <option value="">الكل</option>
                                <option value="info">معلومات</option>
                                <option value="warning">تحذير</option>
                                <option value="error">خطأ</option>
                                <option value="critical">حرج</option>
                            </select>
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                الحالة
                            </label>
                            <select
                                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                value={filters.success === undefined ? '' : filters.success.toString()}
                                onChange={(e) => setFilters({ ...filters, success: e.target.value === '' ? undefined : e.target.value === 'true' })}
                            >
                                <option value="">الكل</option>
                                <option value="true">نجح</option>
                                <option value="false">فشل</option>
                            </select>
                        </div>
                    </div>
                </div>
            )}

            {/* Search */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
                <div className="relative">
                    <Search className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
                    <input
                        type="text"
                        placeholder="بحث في سجل التدقيق..."
                        className="w-full pr-10 pl-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                </div>
            </div>

            {/* Audit Log Table */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                        <thead className="bg-gray-50">
                            <tr>
                                <th
                                    className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                                    onClick={() => handleSort('timestamp')}
                                >
                                    <div className="flex items-center justify-end">
                                        <Clock size={16} className="mr-2" />
                                        التاريخ والوقت
                                        {sortField === 'timestamp' && (
                                            sortDirection === 'asc' ? <ChevronUp size={16} className="mr-1" /> : <ChevronDown size={16} className="mr-1" />
                                        )}
                                    </div>
                                </th>
                                <th
                                    className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                                    onClick={() => handleSort('userName')}
                                >
                                    <div className="flex items-center justify-end">
                                        <User size={16} className="mr-2" />
                                        المستخدم
                                        {sortField === 'userName' && (
                                            sortDirection === 'asc' ? <ChevronUp size={16} className="mr-1" /> : <ChevronDown size={16} className="mr-1" />
                                        )}
                                    </div>
                                </th>
                                <th
                                    className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider cursor-pointer hover:bg-gray-100"
                                    onClick={() => handleSort('action')}
                                >
                                    <div className="flex items-center justify-end">
                                        <Shield size={16} className="mr-2" />
                                        الإجراء
                                        {sortField === 'action' && (
                                            sortDirection === 'asc' ? <ChevronUp size={16} className="mr-1" /> : <ChevronDown size={16} className="mr-1" />
                                        )}
                                    </div>
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الكيان
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الحالة
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الخطورة
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الإجراءات
                                </th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {currentLogs.map((log) => (
                                <tr key={log.id} className="hover:bg-gray-50">
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                        {new Date(log.timestamp).toLocaleString('ar-EG')}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <div className="text-sm font-medium text-gray-900">{log.userName}</div>
                                        <div className="text-sm text-gray-500">{log.userRole}</div>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                        {getActionLabel(log.action)}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <div className="text-sm font-medium text-gray-900">{log.entityName}</div>
                                        <div className="text-sm text-gray-500">{log.entityId}</div>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        {log.success ? (
                                            <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                                                <CheckCircle size={12} className="mr-1" />
                                                نجح
                                            </span>
                                        ) : (
                                            <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                                                <XCircle size={12} className="mr-1" />
                                                فشل
                                            </span>
                                        )}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getSeverityBadge(log.severity)}`}>
                                            {getSeverityIcon(log.severity)}
                                            <span className="mr-1">
                                                {log.severity === 'critical' && 'حرج'}
                                                {log.severity === 'error' && 'خطأ'}
                                                {log.severity === 'warning' && 'تحذير'}
                                                {log.severity === 'info' && 'معلومات'}
                                            </span>
                                        </span>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                                        <button
                                            className="text-blue-600 hover:text-blue-900 flex items-center"
                                            onClick={() => {
                                                setSelectedLog(log);
                                                setShowDetails(true);
                                            }}
                                        >
                                            <Eye size={16} className="mr-1" />
                                            التفاصيل
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                {/* Pagination */}
                <div className="bg-white px-4 py-3 border-t border-gray-200 sm:px-6">
                    <div className="flex items-center justify-between">
                        <div className="flex-1 flex justify-between sm:hidden">
                            <button
                                onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
                                disabled={currentPage === 1}
                                className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                <ArrowRight size={16} className="mr-2" />
                                السابق
                            </button>
                            <button
                                onClick={() => setCurrentPage(Math.min(totalPages, currentPage + 1))}
                                disabled={currentPage === totalPages}
                                className="mr-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                التالي
                                <ArrowLeft size={16} className="ml-2" />
                            </button>
                        </div>
                        <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
                            <div>
                                <p className="text-sm text-gray-700">
                                    عرض <span className="font-medium">{startIndex + 1}</span> إلى{' '}
                                    <span className="font-medium">{Math.min(endIndex, filteredLogs.length)}</span> من{' '}
                                    <span className="font-medium">{filteredLogs.length}</span> سجل
                                </p>
                            </div>
                            <div>
                                <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px space-x-reverse" aria-label="Pagination">
                                    <button
                                        onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
                                        disabled={currentPage === 1}
                                        className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                                    >
                                        <span className="sr-only">Previous</span>
                                        <ArrowRight size={16} />
                                    </button>
                                    {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                                        let pageNum;
                                        if (totalPages <= 5) {
                                            pageNum = i + 1;
                                        } else if (currentPage <= 3) {
                                            pageNum = i + 1;
                                        } else if (currentPage >= totalPages - 2) {
                                            pageNum = totalPages - 4 + i;
                                        } else {
                                            pageNum = currentPage - 2 + i;
                                        }
                                        return (
                                            <button
                                                key={pageNum}
                                                onClick={() => setCurrentPage(pageNum)}
                                                className={`relative inline-flex items-center px-4 py-2 border text-sm font-medium ${currentPage === pageNum
                                                    ? 'z-10 bg-blue-50 border-blue-500 text-blue-600'
                                                    : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                                                    }`}
                                            >
                                                {pageNum}
                                            </button>
                                        );
                                    })}
                                    <button
                                        onClick={() => setCurrentPage(Math.min(totalPages, currentPage + 1))}
                                        disabled={currentPage === totalPages}
                                        className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                                    >
                                        <span className="sr-only">Next</span>
                                        <ArrowLeft size={16} />
                                    </button>
                                </nav>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            {/* Details Modal */}
            {showDetails && selectedLog && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
                    <div className="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white">
                        <div className="mt-3">
                            <div className="flex items-center justify-between mb-4">
                                <h3 className="text-lg font-semibold text-gray-900">تفاصيل سجل التدقيق</h3>
                                <button
                                    className="text-gray-400 hover:text-gray-600"
                                    onClick={() => setShowDetails(false)}
                                >
                                    <XCircle size={24} />
                                </button>
                            </div>

                            <div className="space-y-4">
                                <div className="grid grid-cols-2 gap-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700">التاريخ والوقت</label>
                                        <p className="mt-1 text-sm text-gray-900">{new Date(selectedLog.timestamp).toLocaleString('ar-EG')}</p>
                                    </div>
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700">المعرف</label>
                                        <p className="mt-1 text-sm text-gray-900">{selectedLog.id}</p>
                                    </div>
                                </div>

                                <div className="border-t pt-4">
                                    <h4 className="text-sm font-medium text-gray-900 mb-2">معلومات المستخدم</h4>
                                    <div className="grid grid-cols-2 gap-4">
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">الاسم</label>
                                            <p className="mt-1 text-sm text-gray-900">{selectedLog.userName}</p>
                                        </div>
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">الدور</label>
                                            <p className="mt-1 text-sm text-gray-900">{selectedLog.userRole}</p>
                                        </div>
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">معرف المستخدم</label>
                                            <p className="mt-1 text-sm text-gray-900">{selectedLog.userId}</p>
                                        </div>
                                    </div>
                                </div>

                                <div className="border-t pt-4">
                                    <h4 className="text-sm font-medium text-gray-900 mb-2">معلومات الإجراء</h4>
                                    <div className="grid grid-cols-2 gap-4">
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">الإجراء</label>
                                            <p className="mt-1 text-sm text-gray-900">{getActionLabel(selectedLog.action)}</p>
                                        </div>
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">نوع الكيان</label>
                                            <p className="mt-1 text-sm text-gray-900">{selectedLog.entityType}</p>
                                        </div>
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">معرف الكيان</label>
                                            <p className="mt-1 text-sm text-gray-900">{selectedLog.entityId}</p>
                                        </div>
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">اسم الكيان</label>
                                            <p className="mt-1 text-sm text-gray-900">{selectedLog.entityName}</p>
                                        </div>
                                    </div>
                                </div>

                                {selectedLog.oldValue && (
                                    <div className="border-t pt-4">
                                        <h4 className="text-sm font-medium text-gray-900 mb-2">القيمة القديمة</h4>
                                        <pre className="mt-1 text-sm text-gray-900 bg-gray-50 p-3 rounded-lg overflow-x-auto">
                                            {JSON.stringify(JSON.parse(selectedLog.oldValue), null, 2)}
                                        </pre>
                                    </div>
                                )}

                                {selectedLog.newValue && (
                                    <div className="border-t pt-4">
                                        <h4 className="text-sm font-medium text-gray-900 mb-2">القيمة الجديدة</h4>
                                        <pre className="mt-1 text-sm text-gray-900 bg-gray-50 p-3 rounded-lg overflow-x-auto">
                                            {JSON.stringify(JSON.parse(selectedLog.newValue), null, 2)}
                                        </pre>
                                    </div>
                                )}

                                <div className="border-t pt-4">
                                    <h4 className="text-sm font-medium text-gray-900 mb-2">معلومات النظام</h4>
                                    <div className="grid grid-cols-1 gap-4">
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">عنوان IP</label>
                                            <p className="mt-1 text-sm text-gray-900">{selectedLog.ipAddress}</p>
                                        </div>
                                        <div>
                                            <label className="block text-sm font-medium text-gray-700">User Agent</label>
                                            <p className="mt-1 text-sm text-gray-900 break-all">{selectedLog.userAgent}</p>
                                        </div>
                                    </div>
                                </div>

                                {selectedLog.errorMessage && (
                                    <div className="border-t pt-4">
                                        <h4 className="text-sm font-medium text-gray-900 mb-2">رسالة الخطأ</h4>
                                        <p className="mt-1 text-sm text-red-600">{selectedLog.errorMessage}</p>
                                    </div>
                                )}
                            </div>

                            <div className="mt-6 flex justify-end">
                                <button
                                    className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                                    onClick={() => setShowDetails(false)}
                                >
                                    إغلاق
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default AuditLog;
