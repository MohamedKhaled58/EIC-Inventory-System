import React, { useEffect, useState } from 'react';
import {
    Package,
    ShoppingCart,
    ClipboardList,
    AlertTriangle,
    TrendingUp,
    TrendingDown,
    ArrowRight,
    Star
} from 'lucide-react';
import { useAuthStore } from '../stores/authStore';

interface DashboardStats {
    totalItems: number;
    totalStock: number;
    pendingRequisitions: number;
    pendingTransfers: number;
    lowStockItems: number;
    criticalReserveItems: number;
    monthlyConsumption: number;
    monthlyReceipts: number;
}

interface RecentActivity {
    id: string;
    type: 'requisition' | 'transfer' | 'receipt' | 'consumption';
    description: string;
    descriptionAr: string;
    timestamp: string;
    status: 'pending' | 'approved' | 'completed' | 'rejected';
}

const Dashboard: React.FC = () => {
    const { user } = useAuthStore();
    const [stats, setStats] = useState<DashboardStats>({
        totalItems: 0,
        totalStock: 0,
        pendingRequisitions: 0,
        pendingTransfers: 0,
        lowStockItems: 0,
        criticalReserveItems: 0,
        monthlyConsumption: 0,
        monthlyReceipts: 0,
    });
    const [recentActivities, setRecentActivities] = useState<RecentActivity[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadDashboardData();
    }, []);

    const loadDashboardData = async () => {
        try {
            setLoading(true);
            // TODO: Replace with actual API calls
            // const statsResponse = await api.get('/api/dashboard/stats');
            // const activitiesResponse = await api.get('/api/dashboard/activities');

            // Mock data for now
            setStats({
                totalItems: 1250,
                totalStock: 45678,
                pendingRequisitions: 23,
                pendingTransfers: 8,
                lowStockItems: 15,
                criticalReserveItems: 5,
                monthlyConsumption: 12345,
                monthlyReceipts: 15678,
            });

            setRecentActivities([
                {
                    id: '1',
                    type: 'requisition',
                    description: 'Requisition #REQ-2025-001',
                    descriptionAr: 'طلب #REQ-2025-001',
                    timestamp: '2025-01-30T14:30:00',
                    status: 'pending',
                },
                {
                    id: '2',
                    type: 'transfer',
                    description: 'Transfer from Central WH to Factory 1',
                    descriptionAr: 'نقل من المخزن المركزي إلى المصنع 1',
                    timestamp: '2025-01-30T12:15:00',
                    status: 'approved',
                },
                {
                    id: '3',
                    type: 'receipt',
                    description: 'Receipt from Supplier ABC',
                    descriptionAr: 'استلام من المورد ABC',
                    timestamp: '2025-01-30T10:00:00',
                    status: 'completed',
                },
                {
                    id: '4',
                    type: 'consumption',
                    description: 'Consumption for Project P-001',
                    descriptionAr: 'استهلاك للمشروع P-001',
                    timestamp: '2025-01-30T08:45:00',
                    status: 'completed',
                },
            ]);
        } catch (error) {
            console.error('Error loading dashboard data:', error);
        } finally {
            setLoading(false);
        }
    };

    const getStatusColor = (status: string) => {
        switch (status) {
            case 'pending':
                return 'bg-yellow-100 text-yellow-800';
            case 'approved':
                return 'bg-blue-100 text-blue-800';
            case 'completed':
                return 'bg-green-100 text-green-800';
            case 'rejected':
                return 'bg-red-100 text-red-800';
            default:
                return 'bg-gray-100 text-gray-800';
        }
    };

    const getStatusText = (status: string) => {
        switch (status) {
            case 'pending':
                return 'قيد الانتظار';
            case 'approved':
                return 'موافق عليه';
            case 'completed':
            case 'consumption':
                return 'مكتمل';
            case 'rejected':
                return 'مرفوض';
            default:
                return status;
        }
    };

    const getTypeIcon = (type: string) => {
        switch (type) {
            case 'requisition':
                return <ClipboardList size={20} className="text-blue-600" />;
            case 'transfer':
                return <ShoppingCart size={20} className="text-green-600" />;
            case 'receipt':
                return <Package size={20} className="text-purple-600" />;
            case 'consumption':
                return <TrendingDown size={20} className="text-orange-600" />;
            default:
                return null;
        }
    };

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
            <div>
                <h1 className="text-3xl font-bold text-gray-900">لوحة التحكم</h1>
                <p className="text-gray-600 mt-1">Dashboard</p>
            </div>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                {/* Total Items */}
                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm font-medium text-gray-600">إجمالي الأصناف</p>
                            <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalItems.toLocaleString()}</p>
                            <p className="text-xs text-gray-500 mt-1">Total Items</p>
                        </div>
                        <div className="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
                            <Package size={24} className="text-blue-600" />
                        </div>
                    </div>
                </div>

                {/* Total Stock */}
                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm font-medium text-gray-600">إجمالي المخزون</p>
                            <p className="text-2xl font-bold text-gray-900 mt-1">{stats.totalStock.toLocaleString()}</p>
                            <p className="text-xs text-gray-500 mt-1">Total Stock</p>
                        </div>
                        <div className="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
                            <TrendingUp size={24} className="text-green-600" />
                        </div>
                    </div>
                </div>

                {/* Pending Requisitions */}
                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm font-medium text-gray-600">طلبات معلقة</p>
                            <p className="text-2xl font-bold text-gray-900 mt-1">{stats.pendingRequisitions}</p>
                            <p className="text-xs text-gray-500 mt-1">Pending Requisitions</p>
                        </div>
                        <div className="w-12 h-12 bg-yellow-100 rounded-lg flex items-center justify-center">
                            <ClipboardList size={24} className="text-yellow-600" />
                        </div>
                    </div>
                </div>

                {/* Pending Transfers */}
                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm font-medium text-gray-600">نقل معلق</p>
                            <p className="text-2xl font-bold text-gray-900 mt-1">{stats.pendingTransfers}</p>
                            <p className="text-xs text-gray-500 mt-1">Pending Transfers</p>
                        </div>
                        <div className="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
                            <ShoppingCart size={24} className="text-purple-600" />
                        </div>
                    </div>
                </div>
            </div>

            {/* Alerts Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Low Stock Alert */}
                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between mb-4">
                        <div className="flex items-center gap-2">
                            <AlertTriangle size={20} className="text-orange-600" />
                            <h3 className="text-lg font-semibold text-gray-900">تنبيهات المخزون المنخفض</h3>
                        </div>
                        <span className="text-sm text-gray-500">Low Stock Alerts</span>
                    </div>
                    <div className="text-center py-8">
                        <p className="text-4xl font-bold text-orange-600">{stats.lowStockItems}</p>
                        <p className="text-sm text-gray-600 mt-2">أصناف تحتاج إعادة طلب</p>
                        <p className="text-xs text-gray-500">Items need reorder</p>
                    </div>
                </div>

                {/* Commander's Reserve Alert */}
                <div className="bg-gradient-to-br from-yellow-50 to-amber-50 rounded-lg shadow-sm p-6 border-2 border-yellow-400">
                    <div className="flex items-center justify-between mb-4">
                        <div className="flex items-center gap-2">
                            <Star size={20} className="text-yellow-600" />
                            <h3 className="text-lg font-semibold text-gray-900">احتياطي القائد</h3>
                        </div>
                        <span className="text-sm text-gray-500">Commander's Reserve</span>
                    </div>
                    <div className="text-center py-8">
                        <p className="text-4xl font-bold text-yellow-600">{stats.criticalReserveItems}</p>
                        <p className="text-sm text-gray-600 mt-2">أصناف تحتاج تعبئة الاحتياطي</p>
                        <p className="text-xs text-gray-500">Items need reserve refill</p>
                    </div>
                </div>
            </div>

            {/* Monthly Stats */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Monthly Consumption */}
                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between mb-4">
                        <div className="flex items-center gap-2">
                            <TrendingDown size={20} className="text-red-600" />
                            <h3 className="text-lg font-semibold text-gray-900">الاستهلاك الشهري</h3>
                        </div>
                        <span className="text-sm text-gray-500">Monthly Consumption</span>
                    </div>
                    <div className="text-center py-8">
                        <p className="text-4xl font-bold text-red-600">{stats.monthlyConsumption.toLocaleString()}</p>
                        <p className="text-sm text-gray-600 mt-2">وحدة مستهلكة هذا الشهر</p>
                        <p className="text-xs text-gray-500">Units consumed this month</p>
                    </div>
                </div>

                {/* Monthly Receipts */}
                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between mb-4">
                        <div className="flex items-center gap-2">
                            <TrendingUp size={20} className="text-green-600" />
                            <h3 className="text-lg font-semibold text-gray-900">الاستلام الشهري</h3>
                        </div>
                        <span className="text-sm text-gray-500">Monthly Receipts</span>
                    </div>
                    <div className="text-center py-8">
                        <p className="text-4xl font-bold text-green-600">{stats.monthlyReceipts.toLocaleString()}</p>
                        <p className="text-sm text-gray-600 mt-2">وحدة مستلمة هذا الشهر</p>
                        <p className="text-xs text-gray-500">Units received this month</p>
                    </div>
                </div>
            </div>

            {/* Recent Activities */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200">
                <div className="p-6 border-b border-gray-200">
                    <h3 className="text-lg font-semibold text-gray-900">النشاط الأخير</h3>
                    <p className="text-sm text-gray-500">Recent Activity</p>
                </div>
                <div className="divide-y divide-gray-200">
                    {recentActivities.map((activity) => (
                        <div key={activity.id} className="p-4 hover:bg-gray-50 transition-colors">
                            <div className="flex items-center gap-4">
                                <div className="w-10 h-10 bg-gray-100 rounded-lg flex items-center justify-center">
                                    {getTypeIcon(activity.type)}
                                </div>
                                <div className="flex-1 min-w-0">
                                    <p className="text-sm font-medium text-gray-900">{activity.descriptionAr}</p>
                                    <p className="text-xs text-gray-500">{activity.description}</p>
                                </div>
                                <div className="text-right">
                                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(activity.status)}`}>
                                        {getStatusText(activity.status)}
                                    </span>
                                    <p className="text-xs text-gray-400 mt-1">
                                        {new Date(activity.timestamp).toLocaleString('ar-EG')}
                                    </p>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
                <div className="p-4 border-t border-gray-200">
                    <button className="w-full text-center text-sm text-blue-600 hover:text-blue-800 font-medium">
                        عرض كل النشاطات
                        <ArrowRight size={16} className="inline-block mr-1" />
                    </button>
                </div>
            </div>
        </div>
    );
};

export default Dashboard;
