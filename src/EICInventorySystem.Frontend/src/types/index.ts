// User & Authentication Types
export interface User {
    id: number;
    userId?: number;
    username: string;
    name?: string;
    fullName: string;
    fullNameAr: string;
    email: string;
    role: UserRole;
    factoryId?: number;
    factoryName?: string;
    departmentId?: number;
    warehouseId?: number;
    isActive: boolean;
}


export type UserRole =
    | 'ComplexCommander'
    | 'FactoryCommander'
    | 'CentralWarehouseKeeper'
    | 'FactoryWarehouseKeeper'
    | 'DepartmentHead'
    | 'ProjectManager'
    | 'Officer'
    | 'CivilEngineer'
    | 'Worker'
    | 'Auditor';

export interface LoginRequest {
    username: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    user: User;
    expiresIn: number;
}

// Inventory Types
export interface Item {
    id: number;
    code: string;
    name: string;
    nameAr: string;
    description?: string;
    descriptionAr?: string;
    category: string;
    categoryName?: string;
    categoryNameAr?: string;
    unit: string;
    unitAr: string;
    minimumStock: number;
    reorderPoint: number;
    isActive: boolean;
    isCritical?: boolean;
    reservePercentage?: number;
    unitPrice?: number;
    // Inventory properties (populated when warehouseId is specified)
    availableQuantity?: number;
    totalQuantity?: number;
    reservedQuantity?: number;
    commanderReserveQuantity?: number;
}

export interface Warehouse {
    id: number;
    name: string;
    nameAr: string;
    code: string;
    type: 'Central' | 'Factory';
    factoryId?: number;
    location?: string;
    isActive: boolean;
}

export interface InventoryRecord {
    id: number;
    warehouseId: number;
    warehouseName: string;
    warehouseNameAr: string;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    totalQuantity: number;
    generalQuantity: number;
    commanderReserveQuantity: number;
    minimumReserveRequired: number;
    reorderPoint: number;
    generalAllocated: number;
    reserveAllocated: number;
    availableQuantity: number;
    status: 'OK' | 'LOW' | 'CRITICAL';
    lastUpdated: string;
    lastUpdatedBy: string;
}

// Commander's Reserve Types
export interface CommanderReserveRequest {
    id: number;
    requisitionId: number;
    itemId: number;
    itemName: string;
    itemNameAr: string;
    requestedQuantity: number;
    justification: string;
    justificationAr: string;
    status: 'Pending' | 'Approved' | 'Rejected';
    requestedBy: string;
    requestedDate: string;
    approvedBy?: string;
    approvedDate?: string;
    rejectionReason?: string;
}

export interface ReserveReleaseRequest {
    requisitionId: number;
    itemId: number;
    quantity: number;
    justification: string;
}

// Requisition Types
export interface Requisition {
    id: number;
    requisitionNumber: string;
    type: 'Internal' | 'External';
    status: 'Draft' | 'Pending' | 'Approved' | 'Rejected' | 'PartiallyFulfilled' | 'Completed' | 'Cancelled';
    priority: 'Low' | 'Medium' | 'High' | 'Critical';
    requestedBy: string;
    requestedDate: string;
    departmentId?: number;
    departmentName?: string;
    projectId?: number;
    projectName?: string;
    warehouseId: number;
    warehouseName: string;
    items: RequisitionItem[];
    totalItems: number;
    totalQuantity: number;
    approvedBy?: string;
    approvedDate?: string;
    rejectionReason?: string;
    notes?: string;
}

export interface RequisitionItem {
    id: number;
    requisitionId: number;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    requestedQuantity: number;
    approvedQuantity: number;
    issuedQuantity: number;
    unit: string;
    unitAr: string;
    isFromCommanderReserve: boolean;
    status: 'Pending' | 'Approved' | 'Rejected' | 'Issued';
}

// Project Types
export interface Project {
    id: number;
    projectNumber: string;
    name: string;
    nameAr: string;
    description?: string;
    descriptionAr?: string;
    factoryId: number;
    factoryName: string;
    departmentId?: number;
    departmentName?: string;
    managerId: number;
    managerName: string;
    status: 'Planning' | 'Active' | 'OnHold' | 'Completed' | 'Cancelled';
    priority: 'Low' | 'Medium' | 'High' | 'Critical';
    startDate: string;
    plannedEndDate: string;
    actualEndDate?: string;
    budget: number;
    spentAmount: number;
    progress: number;
    allocatedItems: ProjectAllocation[];
}

export interface ProjectAllocation {
    id: number;
    projectId: number;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    allocatedQuantity: number;
    consumedQuantity: number;
    remainingQuantity: number;
    unit: string;
    unitAr: string;
}

// Transfer Types
export interface TransferRequest {
    id: number;
    transferNumber: string;
    type: 'CentralToFactory' | 'FactoryToFactory' | 'WarehouseToWarehouse';
    status: 'Draft' | 'Pending' | 'Approved' | 'Rejected' | 'InTransit' | 'Received' | 'Cancelled';
    priority: 'Low' | 'Medium' | 'High' | 'Critical';
    requestedBy: string;
    requestedDate: string;
    sourceWarehouseId: number;
    sourceWarehouseName: string;
    destinationWarehouseId: number;
    destinationWarehouseName: string;
    items: TransferItem[];
    totalItems: number;
    approvedBy?: string;
    approvedDate?: string;
    shippedDate?: string;
    receivedBy?: string;
    receivedDate?: string;
    notes?: string;
}

export interface TransferItem {
    id: number;
    transferId: number;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    requestedQuantity: number;
    shippedQuantity: number;
    receivedQuantity: number;
    unit: string;
    unitAr: string;
}

// Transaction Types
export interface InventoryTransaction {
    id: number;
    transactionNumber: string;
    type: 'Receipt' | 'Issue' | 'Transfer' | 'Adjustment' | 'Return';
    status: 'Pending' | 'Completed' | 'Cancelled';
    transactionDate: string;
    warehouseId: number;
    warehouseName: string;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameAr: string;
    quantity: number;
    unit: string;
    unitAr: string;
    referenceNumber?: string;
    referenceType?: string;
    isFromCommanderReserve: boolean;
    commanderApprovalId?: number;
    commanderApprovalDate?: string;
    performedBy: string;
    notes?: string;
}

// Audit Types
export interface AuditLog {
    id: number;
    timestamp: string;
    userId: number;
    username: string;
    action: string;
    entityType: string;
    entityId: number;
    entityName: string;
    changes: string;
    ipAddress: string;
    userAgent: string;
}

// Report Types
export interface InventoryReport {
    warehouseId: number;
    warehouseName: string;
    totalItems: number;
    totalValue: number;
    lowStockItems: number;
    criticalStockItems: number;
    items: InventoryRecord[];
}

export interface MovementReport {
    startDate: string;
    endDate: string;
    totalTransactions: number;
    receipts: number;
    issues: number;
    transfers: number;
    adjustments: number;
    transactions: InventoryTransaction[];
}

// Notification Types
export interface Notification {
    id: number;
    userId: number;
    type: 'Info' | 'Warning' | 'Error' | 'Success';
    title: string;
    titleAr: string;
    message: string;
    messageAr: string;
    isRead: boolean;
    createdAt: string;
    actionUrl?: string;
}

// API Response Types
export interface ApiResponse<T> {
    success: boolean;
    data?: T;
    message?: string;
    errors?: string[];
}

export interface PaginatedResponse<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
}

// Filter & Search Types
export interface InventoryFilter {
    warehouseId?: number;
    itemId?: number;
    status?: 'OK' | 'LOW' | 'CRITICAL';
    searchQuery?: string;
}

export interface RequisitionFilter {
    status?: Requisition['status'];
    priority?: Requisition['priority'];
    warehouseId?: number;
    departmentId?: number;
    projectId?: number;
    startDate?: string;
    endDate?: string;
    searchQuery?: string;
}

export interface ProjectFilter {
    status?: Project['status'];
    priority?: Project['priority'];
    factoryId?: number;
    departmentId?: number;
    managerId?: number;
    startDate?: string;
    endDate?: string;
    searchQuery?: string;
}

// Type aliases for backward compatibility
export type TransferStatus = 'Draft' | 'Pending' | 'Approved' | 'Rejected' | 'InTransit' | 'Received' | 'Cancelled';
export type Transfer = TransferRequest;
export type InventoryItem = Item;
export type ReportPeriod = 'today' | 'week' | 'month' | 'quarter' | 'year' | 'custom';
export type RequisitionStatus = Requisition['status'];
export type ProjectStatus = Project['status'];

// ==================== Worker Types ====================

export interface Worker {
    id: number;
    workerCode: string;
    name: string;
    nameArabic: string;
    militaryRank?: string;
    militaryRankArabic?: string;
    nationalId?: string;
    phone?: string;
    factoryId: number;
    factoryName: string;
    factoryNameArabic: string;
    departmentId: number;
    departmentName: string;
    departmentNameArabic: string;
    joinDate: string;
    isActive: boolean;
    activeCustodyCount: number;
}

export interface CreateWorkerRequest {
    workerCode: string;
    name: string;
    nameArabic: string;
    militaryRank?: string;
    militaryRankArabic?: string;
    nationalId?: string;
    phone?: string;
    factoryId: number;
    departmentId: number;
    joinDate?: string;
}

export interface UpdateWorkerRequest {
    id: number;
    name: string;
    nameArabic: string;
    militaryRank?: string;
    militaryRankArabic?: string;
    nationalId?: string;
    phone?: string;
    departmentId: number;
}

// ==================== Project BOQ Types ====================

export type BOQStatus = 'Draft' | 'Pending' | 'Approved' | 'PartiallyIssued' | 'FullyIssued' | 'Completed' | 'Cancelled';
export type BOQPriority = 'Low' | 'Medium' | 'High' | 'Critical';

export interface ProjectBOQ {
    id: number;
    boqNumber: string;
    projectId: number;
    projectCode: string;
    projectName: string;
    projectNameArabic: string;
    factoryId: number;
    factoryName: string;
    factoryNameArabic: string;
    warehouseId: number;
    warehouseName: string;
    warehouseNameArabic: string;
    createdDate: string;
    requiredDate?: string;
    approvedDate?: string;
    issuedDate?: string;
    completedDate?: string;
    status: BOQStatus;
    priority: BOQPriority;
    totalQuantity: number;
    issuedQuantity: number;
    remainingQuantity: number;
    totalItems: number;
    requiresCommanderReserve: boolean;
    commanderReserveQuantity: number;
    commanderApproved: boolean;
    commanderApprovalId?: number;
    commanderApproverName?: string;
    commanderApprovalDate?: string;
    approvedById?: number;
    approvedByName?: string;
    approvalNotes?: string;
    approvalNotesArabic?: string;
    isRemainingBOQ: boolean;
    originalBOQId?: number;
    originalBOQNumber?: string;
    partialIssueReason?: string;
    partialIssueReasonArabic?: string;
    notes?: string;
    notesArabic?: string;
    items: ProjectBOQItem[];
}

export interface ProjectBOQItem {
    id: number;
    boqId: number;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameArabic: string;
    unit: string;
    requestedQuantity: number;
    issuedQuantity: number;
    remainingQuantity: number;
    availableStock: number;
    shortfall: number;
    isFromCommanderReserve: boolean;
    commanderReserveQuantity: number;
    partialIssueReason?: string;
    partialIssueReasonArabic?: string;
    notes?: string;
    notesArabic?: string;
}

export interface CreateProjectBOQRequest {
    projectId: number;
    factoryId: number;
    warehouseId: number;
    requiredDate: string;
    priority: BOQPriority;
    notes?: string;
    notesArabic?: string;
    items: CreateBOQItemRequest[];
}

export interface CreateBOQItemRequest {
    itemId: number;
    requestedQuantity: number;
    useCommanderReserve: boolean;
    commanderReserveQuantity: number;
    notes?: string;
    notesArabic?: string;
}

export interface UpdateProjectBOQRequest {
    id: number;
    requiredDate?: string;
    priority: BOQPriority;
    notes?: string;
    notesArabic?: string;
    items: UpdateBOQItemRequest[];
}

export interface UpdateBOQItemRequest {
    id?: number;
    itemId: number;
    requestedQuantity: number;
    useCommanderReserve: boolean;
    commanderReserveQuantity: number;
    notes?: string;
    notesArabic?: string;
}

export interface ApproveBOQRequest {
    id: number;
    approvalNotes?: string;
    approvalNotesArabic?: string;
}

export interface RejectBOQRequest {
    id: number;
    rejectionReason: string;
    rejectionReasonArabic?: string;
}

export interface IssueBOQRequest {
    id: number;
    allowPartialIssue: boolean;
    partialIssueReason?: string;
    partialIssueReasonArabic?: string;
    items: IssueBOQItemRequest[];
}

export interface IssueBOQItemRequest {
    itemId: number;
    issueQuantity: number;
}

export interface BOQStatistics {
    totalBOQs: number;
    draftBOQs: number;
    pendingBOQs: number;
    approvedBOQs: number;
    partiallyIssuedBOQs: number;
    fullyIssuedBOQs: number;
    cancelledBOQs: number;
    totalRequestedQuantity: number;
    totalIssuedQuantity: number;
    totalPendingQuantity: number;
}

export interface PendingBOQItems {
    projectId: number;
    projectCode: string;
    projectName: string;
    projectNameArabic: string;
    boqId: number;
    boqNumber: string;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameArabic: string;
    unit: string;
    requestedQuantity: number;
    issuedQuantity: number;
    pendingQuantity: number;
    partialIssueReason?: string;
    partialIssueReasonArabic?: string;
    requiredDate?: string;
}

// ==================== Operational Custody Types ====================

export type CustodyStatus = 'Active' | 'PartiallyReturned' | 'FullyReturned' | 'Consumed' | 'Transferred';

export interface OperationalCustody {
    id: number;
    custodyNumber: string;
    workerId: number;
    workerCode: string;
    workerName: string;
    workerNameArabic: string;
    workerMilitaryRank?: string;
    itemId: number;
    itemCode: string;
    itemName: string;
    itemNameArabic: string;
    unit: string;
    warehouseId: number;
    warehouseName: string;
    warehouseNameArabic: string;
    factoryId: number;
    factoryName: string;
    factoryNameArabic: string;
    departmentId: number;
    departmentName: string;
    departmentNameArabic: string;
    quantity: number;
    returnedQuantity: number;
    consumedQuantity: number;
    remainingQuantity: number;
    issuedDate: string;
    returnedDate?: string;
    status: CustodyStatus;
    purpose: string;
    purposeArabic: string;
    notes?: string;
    notesArabic?: string;
    issuedById: number;
    issuedByName: string;
    returnReceivedById?: number;
    returnReceivedByName?: string;
    custodyLimit?: number;
    daysInCustody: number;
    isOverdue: boolean;
}

export interface CreateCustodyRequest {
    workerId: number;
    itemId: number;
    warehouseId: number;
    factoryId: number;
    departmentId: number;
    quantity: number;
    purpose: string;
    purposeArabic: string;
    custodyLimit?: number;
    notes?: string;
    notesArabic?: string;
}

export interface ReturnCustodyRequest {
    id: number;
    returnQuantity: number;
    notes?: string;
    notesArabic?: string;
}

export interface ConsumeCustodyRequest {
    id: number;
    consumeQuantity: number;
    notes?: string;
    notesArabic?: string;
}

export interface TransferCustodyRequest {
    id: number;
    newWorkerId: number;
    newDepartmentId: number;
    notes?: string;
    notesArabic?: string;
}

export interface CustodyByWorker {
    workerId: number;
    workerCode: string;
    workerName: string;
    workerNameArabic: string;
    militaryRank?: string;
    departmentName: string;
    departmentNameArabic: string;
    custodies: OperationalCustody[];
}

export interface CustodyAgingReport {
    workerId: number;
    workerCode: string;
    workerName: string;
    workerNameArabic: string;
    departmentName: string;
    departmentNameArabic: string;
    totalCustodies: number;
    overdueCustodies: number;
    totalQuantity: number;
    averageDaysInCustody: number;
    maxDaysInCustody: number;
    custodies: OperationalCustody[];
}

export interface CustodyStatistics {
    totalActiveCustodies: number;
    totalOverdueCustodies: number;
    totalQuantityInCustody: number;
    totalReturnedQuantity: number;
    totalConsumedQuantity: number;
    totalWorkersWithCustody: number;
    totalItemsInCustody: number;
}

// ==================== Filters ====================

export interface WorkerFilter {
    factoryId?: number;
    departmentId?: number;
    isActive?: boolean;
    searchTerm?: string;
}

export interface BOQFilter {
    factoryId?: number;
    projectId?: number;
    warehouseId?: number;
    status?: BOQStatus;
    startDate?: string;
    endDate?: string;
}

export interface CustodyFilter {
    workerId?: number;
    itemId?: number;
    factoryId?: number;
    departmentId?: number;
    warehouseId?: number;
    status?: CustodyStatus;
    isOverdue?: boolean;
    startDate?: string;
    endDate?: string;
}
