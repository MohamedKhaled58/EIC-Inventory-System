import React from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { useAuthStore } from '../../stores/authStore';
import {
    LayoutDashboard,
    Package,
    FileText,
    Users,
    Settings,
    LogOut,
    Menu,
    X,
    Warehouse,
    ShoppingCart,
    ClipboardList,
    BarChart3,
    FileSpreadsheet,
    UserCog
} from 'lucide-react';

const MainLayout: React.FC = () => {
    const { user, logout } = useAuthStore();
    const navigate = useNavigate();
    const location = useLocation();
    const [sidebarOpen, setSidebarOpen] = React.useState(true);

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const menuItems = [
        { path: '/dashboard', icon: LayoutDashboard, label: 'Dashboard', labelAr: 'لوحة التحكم' },
        { path: '/inventory', icon: Package, label: 'Inventory', labelAr: 'المخزون' },
        { path: '/requisitions', icon: FileText, label: 'Requisitions', labelAr: 'الطلبات' },
        { path: '/transfers', icon: ShoppingCart, label: 'Custody Transfer', labelAr: 'نقل العهده' },
        { path: '/projects', icon: ClipboardList, label: 'Projects', labelAr: 'المشاريع' },
        { path: '/boq', icon: FileSpreadsheet, label: 'Project BOQ', labelAr: 'قائمة الكميات' },
        { path: '/custody', icon: UserCog, label: 'Custody', labelAr: 'العهد التشغيلية' },
        { path: '/commander-reserve', icon: Warehouse, label: 'Commander Reserve', labelAr: 'احتياطي القائد' },
        { path: '/reports', icon: BarChart3, label: 'Reports', labelAr: 'التقارير' },
        { path: '/users', icon: Users, label: 'Users', labelAr: 'المستخدمين' },
        { path: '/settings', icon: Settings, label: 'Settings', labelAr: 'الإعدادات' },
    ];

    const isActive = (path: string) => location.pathname === path;

    return (
        <div className="min-h-screen bg-gray-100 relative" dir="rtl">
            {/* Sidebar */}
            <aside
                className={`fixed inset-y-0 right-0 z-50 w-64 bg-gradient-to-b from-blue-900 to-blue-800 text-white transition-transform duration-300 ease-in-out ${sidebarOpen ? 'translate-x-0' : 'translate-x-full'
                    } lg:translate-x-0 border-l border-blue-700 shadow-xl`}
            >
                <div className="flex flex-col h-full">
                    {/* Header */}
                    <div className="p-6 border-b border-blue-700">
                        <div className="flex items-center justify-between">
                            <div>
                                <h1 className="text-xl font-bold">EIC Inventory System</h1>
                                <p className="text-xs text-blue-200 mt-1">نظام إدارة مخازن مجمع الصناعات الهندسيه</p>
                            </div>
                            <button
                                onClick={() => setSidebarOpen(false)}
                                className="lg:hidden text-white hover:text-blue-200"
                            >
                                <X size={24} />
                            </button>
                        </div>
                    </div>

                    {/* User Info */}
                    <div className="p-4 border-b border-blue-700 bg-blue-800/50">
                        <div className="flex items-center gap-3">
                            <div className="w-10 h-10 rounded-full bg-blue-600 flex items-center justify-center">
                                <span className="text-lg font-semibold">
                                    {user?.name?.charAt(0) || 'U'}
                                </span>
                            </div>
                            <div className="flex-1 min-w-0 text-right">
                                <p className="text-sm font-medium truncate">{user?.name}</p>
                                <p className="text-xs text-blue-200 truncate">{user?.role}</p>
                            </div>
                        </div>
                    </div>

                    {/* Navigation */}
                    <nav className="flex-1 overflow-y-auto p-4 space-y-2">
                        {menuItems.map((item) => {
                            const Icon = item.icon;
                            return (
                                <button
                                    key={item.path}
                                    onClick={() => navigate(item.path)}
                                    className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${isActive(item.path)
                                        ? 'bg-blue-700 text-white shadow-lg'
                                        : 'text-blue-100 hover:bg-blue-700/50'
                                        }`}
                                >
                                    <Icon size={20} />
                                    <div className="flex-1 text-right">
                                        <p className="text-sm font-medium">{item.labelAr}</p>
                                        <p className="text-xs text-blue-200">{item.label}</p>
                                    </div>
                                </button>
                            );
                        })}
                    </nav>

                    {/* Logout */}
                    <div className="p-4 border-t border-blue-700">
                        <button
                            onClick={handleLogout}
                            className="w-full flex items-center justify-center gap-2 px-4 py-3 bg-red-600 hover:bg-red-700 rounded-lg transition-colors"
                        >
                            <LogOut size={20} />
                            <span className="text-sm font-medium">تسجيل الخروج</span>
                        </button>
                    </div>
                </div>
            </aside>

            {/* Main Content */}
            <div className={`transition-all duration-300 ${sidebarOpen ? 'lg:mr-64' : ''} min-h-screen`}>
                {/* Top Bar */}
                <header className="bg-white shadow-sm border-b border-gray-200">
                    <div className="flex items-center justify-between px-6 py-4">
                        <button
                            onClick={() => setSidebarOpen(!sidebarOpen)}
                            className="lg:hidden text-gray-600 hover:text-gray-900"
                        >
                            <Menu size={24} />
                        </button>
                        <div className="flex-1" />
                        <div className="flex items-center gap-4">
                            <div className="text-left">
                                <p className="text-sm font-medium text-gray-900">{user?.name}</p>
                                <p className="text-xs text-gray-500">{user?.factoryName || 'Complex Commander Level'}</p>
                            </div>
                        </div>
                    </div>
                </header>

                {/* Page Content */}
                <main className="p-6 w-full">
                    <Outlet />
                </main>
            </div>

            {/* Mobile Overlay */}
            {sidebarOpen && (
                <div
                    className="fixed inset-0 bg-black/50 z-40 lg:hidden"
                    onClick={() => setSidebarOpen(false)}
                />
            )}
        </div>
    );
};

export default MainLayout;
