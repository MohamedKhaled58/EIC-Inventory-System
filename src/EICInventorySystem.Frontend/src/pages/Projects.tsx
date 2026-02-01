import React, { useEffect, useState } from 'react';
import {
    Search,
    Filter,
    Plus,
    Download,
    Eye,
    Edit,
    Trash2,
    TrendingUp,
    TrendingDown,
    AlertTriangle,
    CheckCircle,
    Clock,
    DollarSign,
    Calendar,
    Users,
    Package
} from 'lucide-react';
import { useAuthStore } from '../stores/authStore';

interface Project {
    id: string;
    projectCode: string;
    projectName: string;
    projectNameAr: string;
    description: string;
    descriptionAr: string;

    // Status & Priority
    status: 'Planning' | 'Active' | 'OnHold' | 'Completed' | 'Cancelled';
    priority: 'Low' | 'Medium' | 'High' | 'Critical';

    // Dates
    startDate: string;
    endDate: string;
    actualEndDate?: string;

    // Budget
    budget: number;
    spent: number;
    remaining: number;

    // Progress
    progress: number;

    // Factory
    factoryId: string;
    factoryName: string;
    factoryNameAr: string;

    // Manager
    managerId: string;
    managerName: string;
    managerNameAr: string;

    // Team
    teamSize: number;

    // Materials
    totalItems: number;
    allocatedItems: number;
    consumedItems: number;

    // Milestones
    totalMilestones: number;
    completedMilestones: number;

    // Notes
    notes: string;
    notesAr: string;
}

interface ProjectMaterial {
    id: string;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    allocatedQuantity: number;
    consumedQuantity: number;
    remainingQuantity: number;
    unit: string;
    unitAr: string;
    unitCost: number;
    totalCost: number;
}

const Projects: React.FC = () => {
    const { user } = useAuthStore();
    const [projects, setProjects] = useState<Project[]>([]);
    const [filteredProjects, setFilteredProjects] = useState<Project[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedStatus, setSelectedStatus] = useState('all');
    const [selectedPriority, setSelectedPriority] = useState('all');
    const [selectedFactory, setSelectedFactory] = useState('all');
    const [selectedProject, setSelectedProject] = useState<Project | null>(null);
    const [showDetailsModal, setShowDetailsModal] = useState(false);

    useEffect(() => {
        loadProjectsData();
    }, []);

    useEffect(() => {
        filterProjects();
    }, [projects, searchTerm, selectedStatus, selectedPriority, selectedFactory]);

    const loadProjectsData = async () => {
        try {
            setLoading(true);
            // TODO: Replace with actual API call
            // const response = await api.get('/api/projects');

            // Mock data
            setProjects([
                {
                    id: '1',
                    projectCode: 'PRJ-2025-001',
                    projectName: 'Armored Vehicle Production - Batch 1',
                    projectNameAr: 'إنتاج مركبات مدرعة - الدفعة الأولى',
                    description: 'Production of 50 armored vehicles for military use',
                    descriptionAr: 'إنتاج 50 مركبة مدرعة للاستخدام العسكري',
                    status: 'Active',
                    priority: 'High',
                    startDate: '2025-01-01T00:00:00',
                    endDate: '2025-06-30T00:00:00',
                    budget: 50000000,
                    spent: 15000000,
                    remaining: 35000000,
                    progress: 30,
                    factoryId: 'FAC-001',
                    factoryName: 'Factory 1',
                    factoryNameAr: 'المصنع 1',
                    managerId: 'user-001',
                    managerName: 'Ahmed Mohamed',
                    managerNameAr: 'أحمد محمد',
                    teamSize: 25,
                    totalItems: 500,
                    allocatedItems: 450,
                    consumedItems: 150,
                    totalMilestones: 10,
                    completedMilestones: 3,
                    notes: 'Priority project for military requirements',
                    notesAr: 'مشروع ذو أولوية للمتطلبات العسكرية',
                },
                {
                    id: '2',
                    projectCode: 'PRJ-2025-002',
                    projectName: 'Spare Parts Manufacturing',
                    projectNameAr: 'تصنيع قطع الغيار',
                    description: 'Manufacturing spare parts for existing vehicles',
                    descriptionAr: 'تصنيع قطع غيار للمركبات الموجودة',
                    status: 'Active',
                    priority: 'Medium',
                    startDate: '2025-01-15T00:00:00',
                    endDate: '2025-04-30T00:00:00',
                    budget: 10000000,
                    spent: 3000000,
                    remaining: 7000000,
                    progress: 30,
                    factoryId: 'FAC-002',
                    factoryName: 'Factory 2',
                    factoryNameAr: 'المصنع 2',
                    managerId: 'user-002',
                    managerName: 'Sara Ahmed',
                    managerNameAr: 'سارة أحمد',
                    teamSize: 15,
                    totalItems: 200,
                    allocatedItems: 180,
                    consumedItems: 60,
                    totalMilestones: 5,
                    completedMilestones: 1,
                    notes: 'Regular maintenance support',
                    notesAr: 'دعم الصيانة العادية',
                },
                {
                    id: '3',
                    projectCode: 'PRJ-2025-003',
                    projectName: 'Equipment Upgrade',
                    projectNameAr: 'ترقية المعدات',
                    description: 'Upgrading production equipment',
                    descriptionAr: 'ترقية معدات الإنتاج',
                    status: 'Planning',
                    priority: 'Low',
                    startDate: '2025-03-01T00:00:00',
                    endDate: '2025-05-31T00:00:00',
                    budget: 5000000,
                    spent: 0,
                    remaining: 5000000,
                    progress: 0,
                    factoryId: 'FAC-001',
                    factoryName: 'Factory 1',
                    factoryNameAr: 'المصنع 1',
                    managerId: 'user-003',
                    managerName: 'Omar Hassan',
                    managerNameAr: 'عمر حسن',
                    teamSize: 10,
                    totalItems: 50,
                    allocatedItems: 0,
                    consumedItems: 0,
                    totalMilestones: 4,
                    completedMilestones: 0,
                    notes: 'Equipment modernization',
                    notesAr: 'تحديث المعدات',
                },
                {
                    id: '4',
                    projectCode: 'PRJ-2024-015',
                    projectName: 'Emergency Vehicle Repair',
                    projectNameAr: 'إصلاح مركبات طوارئ',
                    description: 'Emergency repair of damaged vehicles',
                    descriptionAr: 'إصلاح طوارئ للمركبات التالفة',
                    status: 'Completed',
                    priority: 'Critical',
                    startDate: '2024-12-01T00:00:00',
                    endDate: '2024-12-31T00:00:00',
                    actualEndDate: '2024-12-28T00:00:00',
                    budget: 2000000,
                    spent: 1800000,
                    remaining: 200000,
                    progress: 100,
                    factoryId: 'FAC-001',
                    factoryName: 'Factory 1',
                    factoryNameAr: 'المصنع 1',
                    managerId: 'user-001',
                    managerName: 'Ahmed Mohamed',
                    managerNameAr: 'أحمد محمد',
                    teamSize: 20,
                    totalItems: 100,
                    allocatedItems: 100,
                    consumedItems: 95,
                    totalMilestones: 3,
                    completedMilestones: 3,
                    notes: 'Completed ahead of schedule',
                    notesAr: 'اكتمل قبل الموعد المحدد',
                },
            ]);
        } catch (error) {
            console.error('Error loading projects data:', error);
        } finally {
            setLoading(false);
        }
    };

    const filterProjects = () => {
        let filtered = [...projects];

        // Search filter
        if (searchTerm) {
            const term = searchTerm.toLowerCase();
            filtered = filtered.filter(prj =>
                prj.projectCode.toLowerCase().includes(term) ||
                prj.projectName.toLowerCase().includes(term) ||
                prj.projectNameAr.includes(term)
            );
        }

        // Status filter
        if (selectedStatus !== 'all') {
            filtered = filtered.filter(prj => prj.status === selectedStatus);
        }

        // Priority filter
        if (selectedPriority !== 'all') {
            filtered = filtered.filter(prj => prj.priority === selectedPriority);
        }

        // Factory filter
        if (selectedFactory !== 'all') {
            filtered = filtered.filter(prj => prj.factoryId === selectedFactory);
        }

        setFilteredProjects(filtered);
    };

    const getStatusBadge = (status: string) => {
        switch (status) {
            case 'Planning':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                        <Clock size={12} className="mr-1" />
                        التخطيط
                    </span>
                );
            case 'Active':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        <TrendingUp size={12} className="mr-1" />
                        نشط
                    </span>
                );
            case 'OnHold':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-yellow-100 text-yellow-800">
                        <AlertTriangle size={12} className="mr-1" />
                        معلق
                    </span>
                );
            case 'Completed':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                        <CheckCircle size={12} className="mr-1" />
                        مكتمل
                    </span>
                );
            case 'Cancelled':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                        <TrendingDown size={12} className="mr-1" />
                        ملغي
                    </span>
                );
            default:
                return null;
        }
    };

    const getPriorityBadge = (priority: string) => {
        switch (priority) {
            case 'Low':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                        منخفض
                    </span>
                );
            case 'Medium':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                        متوسط
                    </span>
                );
            case 'High':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-orange-100 text-orange-800">
                        عالي
                    </span>
                );
            case 'Critical':
                return (
                    <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-red-100 text-red-800">
                        <AlertTriangle size={12} className="mr-1" />
                        حرج
                    </span>
                );
            default:
                return null;
        }
    };

    const getBudgetStatus = (spent: number, budget: number) => {
        const percentage = (spent / budget) * 100;
        if (percentage >= 90) {
            return { color: 'text-red-600', icon: AlertTriangle, text: 'حرج' };
        } else if (percentage >= 70) {
            return { color: 'text-yellow-600', icon: AlertTriangle, text: 'تحذير' };
        }
        return { color: 'text-green-600', icon: CheckCircle, text: 'جيد' };
    };

    const handleViewDetails = (project: Project) => {
        setSelectedProject(project);
        setShowDetailsModal(true);
    };

    const formatCurrency = (amount: number) => {
        return new Intl.NumberFormat('ar-EG', {
            style: 'currency',
            currency: 'EGP',
            minimumFractionDigits: 0,
        }).format(amount);
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
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold text-gray-900">المشاريع</h1>
                    <p className="text-gray-600 mt-1">Projects Management</p>
                </div>
                <div className="flex space-x-3">
                    <button className="flex items-center px-4 py-2 bg-white border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors">
                        <Download size={20} className="mr-2" />
                        تصدير
                    </button>
                    <button className="flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
                        <Plus size={20} className="mr-2" />
                        مشروع جديد
                    </button>
                </div>
            </div>

            {/* Summary Cards */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm font-medium text-gray-600">إجمالي المشاريع</p>
                            <p className="text-3xl font-bold text-gray-900 mt-2">{projects.length}</p>
                        </div>
                        <div className="bg-blue-100 p-3 rounded-full">
                            <Package size={24} className="text-blue-600" />
                        </div>
                    </div>
                </div>

                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm font-medium text-gray-600">المشاريع النشطة</p>
                            <p className="text-3xl font-bold text-blue-600 mt-2">
                                {projects.filter(p => p.status === 'Active').length}
                            </p>
                        </div>
                        <div className="bg-green-100 p-3 rounded-full">
                            <TrendingUp size={24} className="text-green-600" />
                        </div>
                    </div>
                </div>

                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm font-medium text-gray-600">إجمالي الميزانية</p>
                            <p className="text-2xl font-bold text-gray-900 mt-2">
                                {formatCurrency(projects.reduce((sum, p) => sum + p.budget, 0))}
                            </p>
                        </div>
                        <div className="bg-yellow-100 p-3 rounded-full">
                            <DollarSign size={24} className="text-yellow-600" />
                        </div>
                    </div>
                </div>

                <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                    <div className="flex items-center justify-between">
                        <div>
                            <p className="text-sm font-medium text-gray-600">المصروف</p>
                            <p className="text-2xl font-bold text-gray-900 mt-2">
                                {formatCurrency(projects.reduce((sum, p) => sum + p.spent, 0))}
                            </p>
                        </div>
                        <div className="bg-purple-100 p-3 rounded-full">
                            <TrendingDown size={24} className="text-purple-600" />
                        </div>
                    </div>
                </div>
            </div>

            {/* Filters */}
            <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                    {/* Search */}
                    <div className="lg:col-span-2">
                        <div className="relative">
                            <Search size={20} className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400" />
                            <input
                                type="text"
                                placeholder="بحث برمز المشروع أو الاسم..."
                                className="w-full pr-10 pl-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                    </div>

                    {/* Status Filter */}
                    <div>
                        <select
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            value={selectedStatus}
                            onChange={(e) => setSelectedStatus(e.target.value)}
                        >
                            <option value="all">كل الحالات</option>
                            <option value="Planning">التخطيط</option>
                            <option value="Active">نشط</option>
                            <option value="OnHold">معلق</option>
                            <option value="Completed">مكتمل</option>
                            <option value="Cancelled">ملغي</option>
                        </select>
                    </div>

                    {/* Priority Filter */}
                    <div>
                        <select
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            value={selectedPriority}
                            onChange={(e) => setSelectedPriority(e.target.value)}
                        >
                            <option value="all">كل الأولويات</option>
                            <option value="Low">منخفض</option>
                            <option value="Medium">متوسط</option>
                            <option value="High">عالي</option>
                            <option value="Critical">حرج</option>
                        </select>
                    </div>
                </div>

                {/* Factory Filter */}
                <div className="mt-4">
                    <select
                        className="w-full md:w-64 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                        value={selectedFactory}
                        onChange={(e) => setSelectedFactory(e.target.value)}
                    >
                        <option value="all">كل المصانع</option>
                        <option value="FAC-001">المصنع 1</option>
                        <option value="FAC-002">المصنع 2</option>
                    </select>
                </div>
            </div>

            {/* Results Count */}
            <div className="text-sm text-gray-600">
                عرض {filteredProjects.length} من {projects.length} مشروع
            </div>

            {/* Projects Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {filteredProjects.map((project) => {
                    const budgetStatus = getBudgetStatus(project.spent, project.budget);
                    const BudgetIcon = budgetStatus.icon;

                    return (
                        <div key={project.id} className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
                            {/* Header */}
                            <div className="p-6 border-b border-gray-200">
                                <div className="flex items-start justify-between mb-3">
                                    <div>
                                        <h3 className="text-lg font-semibold text-gray-900">{project.projectNameAr}</h3>
                                        <p className="text-sm text-gray-500">{project.projectCode}</p>
                                    </div>
                                    {getPriorityBadge(project.priority)}
                                </div>
                                <p className="text-sm text-gray-600">{project.descriptionAr}</p>
                            </div>

                            {/* Body */}
                            <div className="p-6 space-y-4">
                                {/* Status */}
                                <div className="flex items-center justify-between">
                                    <span className="text-sm text-gray-600">الحالة</span>
                                    {getStatusBadge(project.status)}
                                </div>

                                {/* Progress */}
                                <div>
                                    <div className="flex items-center justify-between mb-2">
                                        <span className="text-sm text-gray-600">التقدم</span>
                                        <span className="text-sm font-medium text-gray-900">{project.progress}%</span>
                                    </div>
                                    <div className="w-full bg-gray-200 rounded-full h-2">
                                        <div
                                            className="bg-blue-600 h-2 rounded-full transition-all"
                                            style={{ width: `${project.progress}%` }}
                                        ></div>
                                    </div>
                                </div>

                                {/* Budget */}
                                <div className="bg-gray-50 p-3 rounded-lg">
                                    <div className="flex items-center justify-between mb-2">
                                        <span className="text-sm text-gray-600">الميزانية</span>
                                        <span className={`text-sm font-medium ${budgetStatus.color} flex items-center`}>
                                            <BudgetIcon size={14} className="mr-1" />
                                            {budgetStatus.text}
                                        </span>
                                    </div>
                                    <div className="space-y-1">
                                        <div className="flex justify-between text-sm">
                                            <span className="text-gray-600">المصروف</span>
                                            <span className="font-medium">{formatCurrency(project.spent)}</span>
                                        </div>
                                        <div className="flex justify-between text-sm">
                                            <span className="text-gray-600">المتبقي</span>
                                            <span className="font-medium">{formatCurrency(project.remaining)}</span>
                                        </div>
                                    </div>
                                </div>

                                {/* Dates */}
                                <div className="flex items-center text-sm text-gray-600">
                                    <Calendar size={16} className="mr-2" />
                                    <span>
                                        {new Date(project.startDate).toLocaleDateString('ar-EG')} - {new Date(project.endDate).toLocaleDateString('ar-EG')}
                                    </span>
                                </div>

                                {/* Manager */}
                                <div className="flex items-center text-sm text-gray-600">
                                    <Users size={16} className="mr-2" />
                                    <span>{project.managerNameAr}</span>
                                </div>
                            </div>

                            {/* Footer */}
                            <div className="p-4 bg-gray-50 border-t border-gray-200 flex justify-end space-x-2 space-x-reverse">
                                <button
                                    className="flex items-center px-3 py-1.5 text-sm text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                                    onClick={() => handleViewDetails(project)}
                                >
                                    <Eye size={16} className="mr-1" />
                                    عرض
                                </button>
                            </div>
                        </div>
                    );
                })}
            </div>

            {filteredProjects.length === 0 && (
                <div className="text-center py-12">
                    <Package size={48} className="mx-auto text-gray-400 mb-4" />
                    <p className="text-gray-500">لا توجد مشاريع</p>
                </div>
            )}

            {/* Details Modal */}
            {showDetailsModal && selectedProject && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
                    <div className="bg-white rounded-lg shadow-xl max-w-4xl w-full mx-4 max-h-[90vh] overflow-y-auto">
                        <div className="p-6 border-b border-gray-200">
                            <div className="flex items-center justify-between">
                                <h2 className="text-2xl font-bold text-gray-900">
                                    {selectedProject.projectNameAr}
                                </h2>
                                <button
                                    onClick={() => setShowDetailsModal(false)}
                                    className="text-gray-400 hover:text-gray-600"
                                >
                                    <Trash2 size={24} />
                                </button>
                            </div>
                            <p className="text-sm text-gray-500 mt-1">{selectedProject.projectCode}</p>
                        </div>

                        <div className="p-6 space-y-6">
                            {/* Header Info */}
                            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        الحالة
                                    </label>
                                    {getStatusBadge(selectedProject.status)}
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        الأولوية
                                    </label>
                                    {getPriorityBadge(selectedProject.priority)}
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        المصنع
                                    </label>
                                    <p className="text-sm text-gray-900">{selectedProject.factoryNameAr}</p>
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        مدير المشروع
                                    </label>
                                    <p className="text-sm text-gray-900">{selectedProject.managerNameAr}</p>
                                </div>
                            </div>

                            {/* Description */}
                            <div>
                                <h3 className="text-lg font-semibold text-gray-900 mb-2">الوصف</h3>
                                <p className="text-sm text-gray-700">{selectedProject.descriptionAr}</p>
                            </div>

                            {/* Progress */}
                            <div>
                                <div className="flex items-center justify-between mb-2">
                                    <h3 className="text-lg font-semibold text-gray-900">التقدم</h3>
                                    <span className="text-sm font-medium text-gray-900">{selectedProject.progress}%</span>
                                </div>
                                <div className="w-full bg-gray-200 rounded-full h-3">
                                    <div
                                        className="bg-blue-600 h-3 rounded-full transition-all"
                                        style={{ width: `${selectedProject.progress}%` }}
                                    ></div>
                                </div>
                                <div className="mt-2 text-sm text-gray-600">
                                    {selectedProject.completedMilestones} من {selectedProject.totalMilestones} معالم مكتملة
                                </div>
                            </div>

                            {/* Budget */}
                            <div className="bg-blue-50 p-4 rounded-lg">
                                <h3 className="text-lg font-semibold text-gray-900 mb-3">الميزانية</h3>
                                <div className="grid grid-cols-3 gap-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-1">
                                            إجمالي الميزانية
                                        </label>
                                        <p className="text-lg font-bold text-gray-900">
                                            {formatCurrency(selectedProject.budget)}
                                        </p>
                                    </div>
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-1">
                                            المصروف
                                        </label>
                                        <p className="text-lg font-bold text-blue-600">
                                            {formatCurrency(selectedProject.spent)}
                                        </p>
                                    </div>
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-1">
                                            المتبقي
                                        </label>
                                        <p className="text-lg font-bold text-green-600">
                                            {formatCurrency(selectedProject.remaining)}
                                        </p>
                                    </div>
                                </div>
                            </div>

                            {/* Dates */}
                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        تاريخ البدء
                                    </label>
                                    <p className="text-sm text-gray-900">
                                        {new Date(selectedProject.startDate).toLocaleDateString('ar-EG')}
                                    </p>
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-1">
                                        تاريخ الانتهاء المخطط
                                    </label>
                                    <p className="text-sm text-gray-900">
                                        {new Date(selectedProject.endDate).toLocaleDateString('ar-EG')}
                                    </p>
                                </div>
                                {selectedProject.actualEndDate && (
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-1">
                                            تاريخ الانتهاء الفعلي
                                        </label>
                                        <p className="text-sm text-gray-900">
                                            {new Date(selectedProject.actualEndDate).toLocaleDateString('ar-EG')}
                                        </p>
                                    </div>
                                )}
                            </div>

                            {/* Materials */}
                            <div>
                                <h3 className="text-lg font-semibold text-gray-900 mb-3">المواد</h3>
                                <div className="grid grid-cols-3 gap-4">
                                    <div className="bg-gray-50 p-3 rounded-lg text-center">
                                        <p className="text-2xl font-bold text-gray-900">{selectedProject.totalItems}</p>
                                        <p className="text-sm text-gray-600">إجمالي الأصناف</p>
                                    </div>
                                    <div className="bg-blue-50 p-3 rounded-lg text-center">
                                        <p className="text-2xl font-bold text-blue-600">{selectedProject.allocatedItems}</p>
                                        <p className="text-sm text-gray-600">المخصصة</p>
                                    </div>
                                    <div className="bg-green-50 p-3 rounded-lg text-center">
                                        <p className="text-2xl font-bold text-green-600">{selectedProject.consumedItems}</p>
                                        <p className="text-sm text-gray-600">المستهلكة</p>
                                    </div>
                                </div>
                            </div>

                            {/* Notes */}
                            <div>
                                <h3 className="text-lg font-semibold text-gray-900 mb-2">ملاحظات</h3>
                                <p className="text-sm text-gray-700">{selectedProject.notesAr}</p>
                            </div>
                        </div>

                        {/* Actions */}
                        <div className="p-6 border-t border-gray-200 flex justify-end space-x-3 space-x-reverse">
                            <button
                                onClick={() => setShowDetailsModal(false)}
                                className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
                            >
                                إغلاق
                            </button>
                            <button className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
                                <Edit size={18} className="inline mr-2" />
                                تعديل
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Projects;
