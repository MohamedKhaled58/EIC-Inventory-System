// Domain Types for EIC Inventory System Frontend

// ==================== Auth Types ====================

export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    user: User;
    expiresIn: number;
}

export interface User {
    id: number;
    username: string;
    fullName: string;
    fullNameAr: string;
    email: string;
    role: UserRole;
    factoryId?: number;
    departmentId?: number;
    isActive: boolean;
    createdAt: string;
    lastLogin?: string;
}

export enum UserRole {
    ComplexCommander = 'ComplexCommander',
    FactoryCommander = 'FactoryCommander',
    CentralWarehouseKeeper = 'CentralWarehouseKeeper',
    FactoryWarehouseKeeper = 'FactoryWarehouseKeeper',
    DepartmentHead = 'DepartmentHead',
    ProjectManager = 'ProjectManager',
    Officer = 'Officer',
    CivilEngineer = 'CivilEngineer',
    Worker = 'Worker',
    Auditor = 'Auditor'
}

// ==================== Organization Types ====================

export interface Factory {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    location: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface Warehouse {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    type: WarehouseType;
    factoryId?: number;
    location: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

export enum WarehouseType {
    Central = 'Central',
    Factory = 'Factory'
}

export interface Department {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    factoryId: number;
    headUserId?: number;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface Project {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    factoryId: number;
    departmentId?: number;
    managerUserId?: number;
    description?: string;
    startDate: string;
    endDate?: string;
    budget?: number;
    status: ProjectStatus;
    priority: ProjectPriority;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

export enum ProjectStatus {
    Planning = 'Planning',
    Active = 'Active',
    OnHold = 'OnHold',
    Completed = 'Completed',
    Cancelled = 'Cancelled'
}

export enum ProjectPriority {
    Low = 'Low',
    Medium = 'Medium',
    High = 'High',
    Critical = 'Critical'
}

// ==================== Inventory Types ====================

export interface Item {
    id: number;
    code: string;
    name: string;
    nameAr: string;
    description?: string;
    category: string;
    unitOfMeasure: string;
    unitOfMeasureAr: string;
    minimumStock: number;
    reorderPoint: number;
    maximumStock?: number;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface InventoryRecord {
    id: number;
    warehouseId: number;
    itemId: number;
    item: Item;
    warehouse: Warehouse;

    // Quantities
    totalQuantity: number;
    generalQuantity: number;
    commanderReserveQuantity: number;

    // Allocations
    generalAllocated: number;
    reserveAllocated: number;

    // Computed
    availableQuantity: number;

    // Thresholds
    minimumReserveRequired: number;
    reorderPoint: number;

    // Status
    status: InventoryStatus;

    // Audit
    lastUpdated: string;
    lastUpdatedBy: number;
    lastUpdatedByUser?: User;
}

export enum InventoryStatus {
    OK = 'OK',
    Low = 'Low',
    Critical = 'Critical',
    Overstocked = 'Overstocked'
}

// ==================== Transaction Types ====================

export interface Requisition {
    id: number;
    requisitionNumber: string;
    type: RequisitionType;
    fromWarehouseId: number;
    fromWarehouse: Warehouse;
    toDepartmentId?: number;
    toDepartment?: Department;
    toProjectId?: number;
    toProject?: Project;
    requestedByUserId: number;
    requestedByUser: User;
    requestedDate: string;
    requiredDate?: string;
    priority: RequisitionPriority;
    status: RequisitionStatus;
    reason?: string;
    rejectionReason?: string;
    approvedByUserId?: number;
    approvedByUser?: User;
    approvedDate?: string;
    items: RequisitionItem[];
    createdAt: string;
    updatedAt: string;
}

export enum RequisitionType {
    Department = 'Department',
    Project = 'Project'
}

export enum RequisitionPriority {
    Normal = 'Normal',
    Urgent = 'Urgent',
    Emergency = 'Emergency'
}

export enum RequisitionStatus {
    Draft = 'Draft',
    Pending = 'Pending',
    Approved = 'Approved',
    PartiallyApproved = 'PartiallyApproved',
    Rejected = 'Rejected',
    Completed = 'Completed',
    Cancelled = 'Cancelled'
}

export interface RequisitionItem {
    id: number;
    requisitionId: number;
    itemId: number;
    item: Item;
    requestedQuantity: number;
    approvedQuantity: number;
    fromGeneralStock: number;
    fromCommanderReserve: number;
    unitPrice?: number;
    reason?: string;
    status: RequisitionItemStatus;
    commanderApprovalRequired: boolean;
    commanderApprovedByUserId?: number;
    commanderApprovedByUser?: User;
    commanderApprovedDate?: string;
    commanderRejectionReason?: string;
}

export enum RequisitionItemStatus {
    Pending = 'Pending',
    Approved = 'Approved',
    PartiallyApproved = 'PartiallyApproved',
    Rejected = 'Rejected',
    Issued = 'Issued'
}

export interface Transfer {
    id: number;
    transferNumber: string;
    fromWarehouseId: number;
    fromWarehouse: Warehouse;
    toWarehouseId: number;
    toWarehouse: Warehouse;
    requestedByUserId: number;
    requestedByUser: User;
    requestedDate: string;
    requiredDate?: string;
    priority: TransferPriority;
    status: TransferStatus;
    reason?: string;
    rejectionReason?: string;
    approvedByUserId?: number;
    approvedByUser?: User;
    approvedDate?: string;
    shippedByUserId?: number;
    shippedByUser?: User;
    shippedDate?: string;
    receivedByUserId?: number;
    receivedByUser?: User;
    receivedDate?: string;
    items: TransferItem[];
    createdAt: string;
    updatedAt: string;
}

export enum TransferPriority {
    Normal = 'Normal',
    Urgent = 'Urgent',
    Emergency = 'Emergency'
}

export enum TransferStatus {
    Draft = 'Draft',
    PendingApproval = 'PendingApproval',
    Approved = 'Approved',
    Rejected = 'Rejected',
    Shipped = 'Shipped',
    Received = 'Received',
    Cancelled = 'Cancelled'
}

export interface TransferItem {
    id: number;
    transferId: number;
    itemId: number;
    item: Item;
    requestedQuantity: number;
    shippedQuantity: number;
    receivedQuantity: number;
    unitPrice?: number;
    reason?: string;
    status: TransferItemStatus;
}

export enum TransferItemStatus {
    Pending = 'Pending',
    Shipped = 'Shipped',
    Received = 'Received',
    Cancelled = 'Cancelled'
}

export interface Receipt {
    id: number;
    receiptNumber: string;
    warehouseId: number;
    warehouse: Warehouse;
    supplierId: number;
    supplier: Supplier;
    purchaseOrderNumber?: string;
    receivedByUserId: number;
    receivedByUser: User;
    receivedDate: string;
    status: ReceiptStatus;
    notes?: string;
    items: ReceiptItem[];
    createdAt: string;
    updatedAt: string;
}

export enum ReceiptStatus {
    Draft = 'Draft',
    Pending = 'Pending',
    Completed = 'Completed',
    Cancelled = 'Cancelled'
}

export interface ReceiptItem {
    id: number;
    receiptId: number;
    itemId: number;
    item: Item;
    orderedQuantity: number;
    receivedQuantity: number;
    generalQuantity: number;
    commanderReserveQuantity: number;
    unitPrice: number;
    totalPrice: number;
    batchNumber?: string;
    expiryDate?: string;
    notes?: string;
}

export interface Consumption {
    id: number;
    consumptionNumber: string;
    projectId: number;
    project: Project;
    departmentId?: number;
    department?: Department;
    consumedByUserId: number;
    consumedByUser: User;
    consumptionDate: string;
    status: ConsumptionStatus;
    notes?: string;
    approvedByUserId?: number;
    approvedByUser?: User;
    approvedDate?: string;
    rejectionReason?: string;
    items: ConsumptionItem[];
    createdAt: string;
    updatedAt: string;
}

export enum ConsumptionStatus {
    Draft = 'Draft',
    Pending = 'Pending',
    Approved = 'Approved',
    Rejected = 'Rejected',
    Cancelled = 'Cancelled'
}

export interface ConsumptionItem {
    id: number;
    consumptionId: number;
    itemId: number;
    item: Item;
    quantity: number;
    unitPrice?: number;
    reason?: string;
}

// ==================== Supplier Types ====================

export interface Supplier {
    id: number;
    code: string;
    name: string;
    nameAr: string;
    contactPerson?: string;
    phone?: string;
    email?: string;
    address?: string;
    taxNumber?: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

// ==================== Audit Types ====================

export interface AuditLog {
    id: number;
    userId: number;
    user: User;
    action: string;
    entityType: string;
    entityId: number;
    entityName?: string;
    changes?: string;
    ipAddress?: string;
    userAgent?: string;
    timestamp: string;
}

// ==================== Dashboard Types ====================

export interface DashboardStats {
    totalFactories: number;
    totalWarehouses: number;
    totalItems: number;
    totalProjects: number;
    activeProjects: number;
    pendingRequisitions: number;
    pendingTransfers: number;
    lowStockItems: number;
    criticalReserveItems: number;
    totalInventoryValue: number;
    recentActivities: AuditLog[];
}

export interface InventoryMovement {
    id: number;
    transactionType: TransactionType;
    transactionNumber: string;
    itemId: number;
    item: Item;
    warehouseId: number;
    warehouse: Warehouse;
    quantity: number;
    fromGeneralStock: number;
    fromCommanderReserve: number;
    unitPrice?: number;
    transactionDate: string;
    performedByUserId: number;
    performedByUser: User;
    notes?: string;
}

export enum TransactionType {
    Receipt = 'Receipt',
    Transfer = 'Transfer',
    Requisition = 'Requisition',
    Consumption = 'Consumption',
    Adjustment = 'Adjustment',
    Return = 'Return'
}

// ==================== Common Types ====================

export interface PaginatedResponse<T> {
    data: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}

export interface QueryParams {
    pageNumber?: number;
    pageSize?: number;
    searchTerm?: string;
    sortBy?: string;
    sortOrder?: 'asc' | 'desc';
    filters?: Record<string, any>;
}

export interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
    errors?: string[];
}

export interface SelectOption {
    value: string | number;
    label: string;
    labelAr?: string;
}

export interface TableColumn {
    key: string;
    title: string;
    titleAr?: string;
    sortable?: boolean;
    filterable?: boolean;
    width?: string | number;
    align?: 'left' | 'center' | 'right';
}

export interface FormField {
    name: string;
    label: string;
    labelAr?: string;
    type: 'text' | 'number' | 'email' | 'password' | 'date' | 'select' | 'textarea' | 'checkbox' | 'radio';
    required?: boolean;
    placeholder?: string;
    placeholderAr?: string;
    options?: SelectOption[];
    validation?: {
        pattern?: RegExp;
        minLength?: number;
        maxLength?: number;
        min?: number;
        max?: number;
        custom?: (value: any) => string | null;
    };
}

export interface Notification {
    id: string;
    type: 'success' | 'error' | 'warning' | 'info';
    title: string;
    titleAr?: string;
    message: string;
    messageAr?: string;
    timestamp: string;
    read: boolean;
    actionUrl?: string;
}

export interface ChartData {
    labels: string[];
    datasets: {
        label: string;
        labelAr?: string;
        data: number[];
        backgroundColor?: string | string[];
        borderColor?: string | string[];
    }[];
}
