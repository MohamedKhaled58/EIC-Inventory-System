import React, { useEffect, useState } from 'react';
import {
    Search,
    Filter,
    Plus,
    Download,
    Star,
    AlertTriangle,
    CheckCircle,
    XCircle,
    Eye,
    Edit,
    Trash2
} from 'lucide-react';
import { useAuthStore } from '../stores/authStore';

interface InventoryItem {
    id: string;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    category: string;
    categoryAr: string;
    unit: string;
    unitAr: string;
    totalStock: number;
    generalStock: number;
    commanderReserve: number;
    allocated: number;
    available: number;
    reorderPoint: number;
    minimumReserveRequired: number;
    status: 'OK' | 'LOW' | 'CRITICAL';
    warehouseName: string;
    warehouseNameAr: string;
    lastUpdated: string;
}

const Inventory: React.FC = () => {
    const { user } = useAuthStore();
    const [items, setItems] = useState<InventoryItem[]>([]);
    const [filteredItems, setFilteredItems] = useState<InventoryItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedCategory, setSelectedCategory] = useState('all');
    const [selectedWarehouse, setSelectedWarehouse] = useState('all');
    const [selectedStatus, setSelectedStatus] = useState('all');
    const [showReserveOnly, setShowReserveOnly] = useState(false);

    useEffect(() => {
        loadInventoryData();
    }, []);

    useEffect(() => {
        filterItems();
    }, [items, searchTerm, selectedCategory, selectedWarehouse, selectedStatus, showReserveOnly]);

    const loadInventoryData = async () => {
        try {
            setLoading(true);
            // TODO: Replace with actual API call
            // const response = await api.get('/api/inventory');

            // Mock data
            setItems([
                {
                    id: '1',
                    itemCode: 'STL-001',
                    itemName: 'Steel Plate 10mm',
                    itemNameAr: 'لوحة فولاذية 10 مم',
                    category: 'Raw Materials',
                    categoryAr: 'المواد الخام',
                    unit: 'KG',
                    unitAr: 'كجم',
                    totalStock: 1000,
                    generalStock: 800,
                    commanderReserve: 200,
                    allocated: 100,
                    available: 700,
                    reorderPoint: 500,
                    minimumReserveRequired: 150,
                    status: 'OK',
                    warehouseName: 'Central Warehouse 1',
                    warehouseNameAr: 'المخزن المركزي 1',
                    lastUpdated: '2025-01-30T10:00:00',
                },
                {
                    id: '2',
                    itemCode: 'ALM-002',
                    itemName: 'Aluminum Sheet 5mm',
                    itemNameAr: 'ورق ألمنيوم 5 مم',
                    category: 'Raw Materials',
                    categoryAr: 'المواد الخام',
                    unit: 'KG',
                    unitAr: 'كجم',
                    totalStock: 300,
                    generalStock: 240,
                    commanderReserve: 60,
                    allocated: 50,
                    available: 190,
                    reorderPoint: 400,
                    minimumReserveRequired: 50,
                    status: 'LOW',
                    warehouseName: 'Factory 1 Warehouse',
                    warehouseNameAr: 'مخزن المصنع 1',
                    lastUpdated: '2025-01-30T09:30:00',
                },
                {
                    id: '3',
                    itemCode: 'BLT-003',
                    itemName: 'Bolt M10x50',
                    itemNameAr: 'صمولة M10x50',
                    category: 'Fasteners',
                    categoryAr: 'المثبتات',
                    unit: 'PCS',
                    unitAr: 'قطعة',
                    totalStock: 5000,
                    generalStock: 4000,
                    commanderReserve: 1000,
                    allocated: 500,
                    available: 3500,
                    reorderPoint: 2000,
                    minimumReserveRequired: 800,
                    status: 'OK',
                    warehouseName: 'Central Warehouse 2',
                    warehouseNameAr: 'المخزن المركزي 2',
                    lastUpdated: '2025-01-30T11:00:00',
                },
                {
                    id: '4',
                    itemCode: 'ENG-004',
                    itemName: 'Engine Oil 15W40',
                    itemNameAr: 'زيت محرك 15W40',
                    category: 'Consumables',
                    categoryAr: 'المواد الاستهلاكية',
                    unit: 'L',
                    unitAr: 'لتر',
                    totalStock: 100,
                    generalStock: 80,
                    commanderReserve: 20,
                    allocated: 30,
                    available: 50,
                    reorderPoint: 200,
                    minimumReserveRequired: 15,
                    status: 'CRITICAL',
                    warehouseName: 'Factory 2 Warehouse',
                    warehouseNameAr: 'مخزن المصنع 2',
                    lastUpdated: '2025-01-30T08:00:00',
                },
            ]);
        } catch (error) {
            console.error('Error loading inventory data:', error);
        } finally {
            setLoading(false);
        }
    };

    const filterItems = () => {
        let filtered = [...items];

        // Search filter
        if (searchTerm) {
            const term = searchTerm.toLowerCase();
            filtered = filtered.filter(item =>
                item.itemCode.toLowerCase().includes(term) ||
                item.itemName.toLowerCase().includes(term) ||
                item.itemNameAr.includes(term)
            );
        }

        // Category filter
        if (selectedCategory !== 'all') {
            filtered = filtered.filter(item => item.category === selectedCategory);
        }

        // Warehouse filter
        if (selectedWarehouse !== 'all') {
            filtered = filtered.filter(item => item.warehouseName === selectedWarehouse);
        }

        // Status filter
        if (selectedStatus !== 'all') {
            filtered = filtered.filter(item => item.status === selectedStatus);
        }

        // Reserve only filter
        if (showReserveOnly) {
            filtered = filtered.filter(item => item.commanderReserve < item.minimumReserveRequired);
        }

        setFilteredItems(filtered);
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'OK':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                        <CheckCircle size={12} className="mr-1" />
                        جيد
                    </span>
                );
            case 'LOW':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800">
                        <AlertTriangle size={12} className="mr-1" />
                        منخفض
                    </span>
                );
            case 'CRITICAL':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                        <XCircle size={12} className="mr-1" />
                        حرج
                    </span>
                );
            default:
                return null;
        }
    };

    const getReserveStatus = (item: InventoryItem) => {
        if (item.commanderReserve < item.minimumReserveRequired) {
            return (
                <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                    <AlertTriangle size={12} className="mr-1" />
                    يحتاج تعبئة
                </span>
            );
        }
        return (
            <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                <CheckCircle size={12} className="mr-1" />
                كافٍ
            </span>
        );
    };

    const categories = ['all', ...Array.from(new Set(items.map(item => item.category)))];
    const warehouses = ['all', ...Array.from(new Set(items.map(item => item.warehouseName)))];

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
                    <h1 className="text-3xl font-bold text-gray-900">المخزون</h1>
                    <p className="text-gray-600 mt-1">Inventory Management</p>
                </div>
                <div className="flex space-x-3">
                    <button className="flex items-center px-4 py-2 bg-white border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors">
                        <Download size={20} className="mr-2" />
                        تصدير
                    </button>
                    <button className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
                        <Plus size={20} className="mr-2" />
                        إضافة صنف
                    </button>
                </div>
            </div>

            {/* Filters */}
            <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
                    {/* Search */}
                    <div className="lg:col-span-2">
                        <div className="relative">
                            <Search size={20} className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                            <input
                                type="text"
                                placeholder="بحث بالكود أو الاسم..."
                                className="w-full pr-10 pl-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                    </div>

                    {/* Category Filter */}
                    <div>
                        <select
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            value={selectedCategory}
                            onChange={(e) => setSelectedCategory(e.target.value)}
                        >
                            <option value="all">كل الفئات</option>
                            {categories.filter(c => c !== 'all').map(cat => (
                                <option key={cat} value={cat}>{cat}</option>
                            ))}
                        </select>
                    </div>

                    {/* Warehouse Filter */}
                    <div>
                        <select
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            value={selectedWarehouse}
                            onChange={(e) => setSelectedWarehouse(e.target.value)}
                        >
                            <option value="all">كل المخازن</option>
                            {warehouses.filter(w => w !== 'all').map(wh => (
                                <option key={wh} value={wh}>{wh}</option>
                            ))}
                        </select>
                    </div>

                    {/* Status Filter */}
                    <div>
                        <select
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            value={selectedStatus}
                            onChange={(e) => setSelectedStatus(e.target.value)}
                        >
                            <option value="all">كل الحالات</option>
                            <option value="OK">جيد</option>
                            <option value="LOW">منخفض</option>
                            <option value="CRITICAL">حرج</option>
                        </select>
                    </div>
                </div>

                {/* Reserve Filter */}
                <div className="mt-4 flex items-center space-x-2">
                    <input
                        type="checkbox"
                        id="reserveOnly"
                        className="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                        checked={showReserveOnly}
                        onChange={(e) => setShowReserveOnly(e.target.checked)}
                    />
                    <label htmlFor="reserveOnly" className="flex items-center text-sm text-gray-700">
                        <Star size={16} className="mr-1 text-yellow-600" />
                        عرض الأصناف التي تحتاج تعبئة الاحتياطي فقط
                    </label>
                </div>
            </div>

            {/* Results Count */}
            <div className="text-sm text-gray-600">
                عرض {filteredItems.length} من {items.length} صنف
            </div>

            {/* Inventory Table */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الكود
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الصنف
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الفئة
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    المخزن
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الإجمالي
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider bg-yellow-50">
                                    <div className="flex items-center justify-end">
                                        <Star size={14} className="mr-1 text-yellow-600" />
                                        الاحتياطي
                                    </div>
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    متاح
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الحالة
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    الإجراءات
                                </th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {filteredItems.map((item) => (
                                <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                        {item.itemCode}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <div>
                                            <div className="text-sm font-medium text-gray-900">{item.itemNameAr}</div>
                                            <div className="text-sm text-gray-500">{item.itemName}</div>
                                        </div>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                        {item.categoryAr}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                        {item.warehouseNameAr}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                        {item.totalStock.toLocaleString()} {item.unitAr}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap bg-yellow-50">
                                        <div className="text-sm font-medium text-gray-900">
                                            {item.commanderReserve.toLocaleString()} {item.unitAr}
                                        </div>
                                        <div className="mt-1">
                                            {getReserveStatus(item)}
                                        </div>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                        {item.available.toLocaleString()} {item.unitAr}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        {getStatusBadge(item.status)}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                                        <div className="flex items-center space-x-2 space-x-reverse">
                                            <button className="text-blue-600 hover:text-blue-800" title="عرض">
                                                <Eye size={18} />
                                            </button>
                                            <button className="text-green-600 hover:text-green-800" title="تعديل">
                                                <Edit size={18} />
                                            </button>
                                            <button className="text-red-600 hover:text-red-800" title="حذف">
                                                <Trash2 size={18} />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                {filteredItems.length === 0 && (
                    <div className="text-center py-12">
                        <p className="text-gray-500">لا توجد نتائج</p>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Inventory;
