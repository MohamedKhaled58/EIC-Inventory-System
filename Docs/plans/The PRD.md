`# Product Requirements Document (PRD)
## Enginerring Industrial Complex Inventory Command System
### مجمع الصناعات الهندسية - نظام إدارة المخازن

---

## Document Control

| Item | Details |
|------|---------|
| **Document Version** | 1.0 |
| **Last Updated** | January 30, 2025 |
| **Author** | Mohamed Khaled |
| **Status** | Draft → Ready for Development |
| **Classification** | Internal Use Only |

---

## 1. Executive Summary

### 1.1 Project Overview
This system is a **mission-critical inventory management platform** for the Egyptian Armed Forces Engineering Industries Complex (مجمع الصناعات الهندسية للقوات المسلحة) - a defense manufacturing facility producing armored vehicles and military equipment.

### 1.2 Problem Statement
The complex currently faces:
- ❌ **Data Inconsistencies**: Mismatches between database, backend, and frontend
- ❌ **Duplicate Information**: Redundant data causing confusion
- ❌ **Poor Traceability**: Cannot track materials from supplier to final product
- ❌ **Authorization Gaps**: Users accessing data outside their authority
- ❌ **Manual Processes**: Excel-based tracking causing errors
- ❌ **No Commander's Reserve Tracking**: Critical military requirement missing

### 1.3 Goals & Objectives

**Primary Goals:**
1. ✅ **Complete Material Traceability**: Track every item from supplier → central warehouse → factory warehouse → project
2. ✅ **Hierarchical Access Control**: Enforce military chain of command
3. ✅ **Commander's Reserve Management**: Separate tracking and authorization for emergency stock
4. ✅ **Real-time Inventory Visibility**: Accurate stock levels across all locations
5. ✅ **Automated Workflows**: Digital requisition, approval, and transfer processes

**Success Metrics:**
- 100% transaction traceability
- <1 second inventory query response time
- Zero unauthorized data access
- 95% reduction in manual paperwork
- Real-time stock accuracy ≥99%

---

## 2. System Overview

### 2.1 Organizational Structure
```
المجمع (Complex)
│
├── المخازن المركزية (Central Warehouses) [Multiple]
│   ├── Managed by: Chief Warehouse Keeper
│   └── Supplies: All factories
│
├── المصانع (Factories) [Multiple]
│   │
│   ├── Factory Command Structure
│   │   ├── قائد المصنع (Factory Commander) - Top Authority
│   │   ├── ضباط (Officers) - Middle Management
│   │   ├── مهندسين مدنيين (Civil Engineers) - Technical Staff
│   │   └── عمالة (Workers) - Production Workers
│   │
│   ├── مخزن المصنع (Factory Warehouse)
│   │   ├── Managed by: Factory Warehouse Keeper
│   │   └── Contains: General Stock + Commander's Reserve ⭐
│   │
│   ├── الأقسام (Departments/Sections) [Multiple]
│   │   ├── Headed by: Department Head
│   │   └── Request materials from factory warehouse
│   │
│   └── المشاريع (Projects) [Multiple]
│       ├── Managed by: Project Manager
│       ├── Allocated Items: From factory warehouse
│       └── Tracks: Budget, Timeline, Consumption
│
└── الموردين (Suppliers) [Multiple]
    └── Supply materials to central warehouses
```

### 2.2 Material Flow Chain
```
┌─────────────┐
│  Suppliers  │
│  (External) │
└──────┬──────┘
       │ Purchase Order
       ↓
┌─────────────────────┐
│ Central Warehouses  │
│  (Complex Level)    │
└──────┬──────────────┘
       │ Transfer Request
       ↓
┌─────────────────────┐
│ Factory Warehouses  │
│  (Factory Level)    │
│                     │
│ ┌─────────────────┐ │
│ │ General Stock   │ │
│ ├─────────────────┤ │
│ │ Commander's     │ │
│ │ Reserve ⭐      │ │
│ └─────────────────┘ │
└──────┬──────────────┘
       │ Requisition
       ↓
┌─────────────────────┐
│ Projects/Departments│
└──────┬──────────────┘
       │ Consumption
       ↓
┌─────────────────────┐
│  Final Products     │
└─────────────────────┘
```

---

## 3. Critical Business Concept: Commander's Reserve

### 3.1 Definition
**احتياطي قائد المصنع (Commander's Reserve)** is a portion of inventory in EVERY warehouse (central and factory) that is:
- Reserved for emergency use
- Requires special authorization
- Separate from general stock
- Protected from normal requisitions

### 3.2 Business Rules

#### **Allocation Rules:**
```
When items arrive at warehouse:
├── General Stock (المخزون العام): 70-80% of received quantity
└── Commander's Reserve (احتياطي القائد): 20-30% of received quantity

Example:
Received: 1,000 KG Steel Plate
├── General: 800 KG (available for normal requisitions)
└── Reserve: 200 KG (commander approval required)
```

#### **Authorization Matrix:**

| Action | General Stock | Commander's Reserve |
|--------|---------------|---------------------|
| **View** | All authorized users | All authorized users |
| **Request** | Department Heads, Project Managers | Same |
| **Approve** | Warehouse Keeper, Officers | **ONLY Factory/Complex Commander** |
| **Release** | Warehouse Keeper | **ONLY Factory/Complex Commander** |

#### **Use Cases for Commander's Reserve:**
1. 🚨 **Emergency Production**: Urgent military orders
2. 🎯 **Critical Projects**: High-priority defense projects
3. ⚡ **Supply Chain Disruption**: When normal suppliers delayed
4. 🛡️ **Strategic Stockpiling**: Preparation for extended operations
5. 🔧 **Unexpected Failures**: Equipment breakdown requiring immediate parts

#### **Workflow Example:**
```
Scenario: Department needs 50 KG steel, but general stock only has 30 KG

Step 1: Department Head creates requisition
├── Requested: 50 KG
├── Available in General: 30 KG
└── System flags: "Requires 20 KG from Commander's Reserve"

Step 2: Warehouse Keeper reviews
├── Approves: 30 KG from general stock
└── Routes to Factory Commander: 20 KG reserve request

Step 3: Factory Commander reviews
├── Checks justification
├── Verifies project priority
├── Decision:
    ├── Approve → 20 KG released from reserve
    └── Deny → Department must wait for resupply

Step 4: If approved
├── Transaction created with special flag
├── Audit trail logs commander approval
├── Reserve quantity decremented
└── Items issued to department
```

### 3.3 Technical Requirements

#### **Database Schema:**
```sql
-- Every inventory record must track:
CREATE TABLE InventoryRecords (
    Id INT PRIMARY KEY,
    WarehouseId INT NOT NULL,
    ItemId INT NOT NULL,
    
    -- Critical Reserve Tracking
    TotalQuantity DECIMAL(18,3) NOT NULL,
    GeneralQuantity DECIMAL(18,3) NOT NULL,
    CommanderReserveQuantity DECIMAL(18,3) NOT NULL,
    
    -- Thresholds
    MinimumReserveRequired DECIMAL(18,3) NOT NULL,
    ReorderPoint DECIMAL(18,3) NOT NULL,
    
    -- Allocation
    GeneralAllocated DECIMAL(18,3) DEFAULT 0,
    ReserveAllocated DECIMAL(18,3) DEFAULT 0,
    
    -- Computed: Available for normal use
    AvailableQuantity AS (GeneralQuantity - GeneralAllocated),
    
    -- Audit
    LastUpdated TIMESTAMP NOT NULL,
    LastUpdatedBy INT NOT NULL,
    
    CONSTRAINT CHK_Quantities CHECK (
        TotalQuantity = GeneralQuantity + CommanderReserveQuantity
    )
);

-- Transactions must flag reserve usage
CREATE TABLE InventoryTransactions (
    Id INT PRIMARY KEY,
    -- ... other fields ...
    
    IsFromCommanderReserve BOOLEAN DEFAULT FALSE,
    CommanderApprovalId INT NULL,
    CommanderApprovalDate TIMESTAMP NULL,
    
    CONSTRAINT CHK_ReserveApproval CHECK (
        (IsFromCommanderReserve = FALSE) OR 
        (IsFromCommanderReserve = TRUE AND CommanderApprovalId IS NOT NULL)
    )
);
```

#### **UI Requirements:**
```typescript
// Inventory Grid Display
interface InventoryGridRow {
  itemCode: string;
  itemName: string;
  totalStock: number;
  generalStock: number;           // White background
  commanderReserve: number;        // Gold background ⭐
  allocated: number;
  available: number;
  reorderPoint: number;
  status: 'OK' | 'LOW' | 'CRITICAL';
}

// Visual Indicators
- Commander's Reserve column: Gold/Yellow background (#8b6914)
- Reserve badge: ⭐ icon
- Low reserve alert: 🔶 when reserve < MinimumReserveRequired
- Approval required badge: 🔒 on requisitions needing commander approval
```

#### **Authorization Policy:**
```csharp
// ASP.NET Core Policy
services.AddAuthorization(options =>
{
    options.AddPolicy("AccessCommanderReserve", policy =>
        policy.RequireAssertion(context =>
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            return role == "FactoryCommander" || role == "ComplexCommander";
        }));
});

// Controller
[HttpPost("reserve/release")]
[Authorize(Policy = "AccessCommanderReserve")]
public async Task ReleaseCommanderReserve(
    [FromBody] ReserveReleaseRequest request)
{
    // Only commanders reach here
}
```

---

## 4. User Roles & Permissions

### 4.1 Role Definitions

#### **4.1.1 Complex Commander (قائد المجمع)**
- **Scope**: Entire complex
- **Authority Level**: Supreme
- **Responsibilities**:
  - Strategic oversight of all operations
  - Access to all factories and warehouses
  - Final authority on cross-factory transfers
  - Can release any commander's reserve
  - User management across complex

**Permissions:**
```
✅ View: ALL data across entire complex
✅ Create: Requisitions, transfers, projects (anywhere)
✅ Approve: ALL requisitions, transfers, reserve releases
✅ Manage: Users, factories, central warehouses
✅ Reports: Complex-wide analytics
```

#### **4.1.2 Factory Commander (قائد المصنع)**
- **Scope**: Single factory
- **Authority Level**: High (within factory)
- **Responsibilities**:
  - Factory operations oversight
  - Commander's reserve management for factory
  - Project approval and prioritization
  - Officer and personnel management
  - Factory budget control

**Permissions:**
```
✅ View: All factory data (warehouse, projects, departments)
✅ Create: Requisitions, projects, transfers to/from central
✅ Approve: Factory requisitions, reserve releases (factory only)
✅ Manage: Factory users, departments, projects
✅ Reports: Factory-level analytics
❌ Access: Other factories' data
```

#### **4.1.3 Central Warehouse Keeper (أمين مخزن مركزي)**
- **Scope**: Assigned central warehouse
- **Authority Level**: Medium (warehouse operations)
- **Responsibilities**:
  - Receive materials from suppliers
  - Manage central warehouse inventory
  - Approve transfer requests to factories
  - Stock level monitoring
  - Physical inventory counts

**Permissions:**
```
✅ View: Own warehouse inventory, incoming POs
✅ Create: Receipt transactions, stock adjustments
✅ Approve: Transfer requests from factories
✅ Issue: Materials to factories
❌ Create: Requisitions, projects
❌ Access: Commander's reserve (view only)
❌ Manage: Users
```

#### **4.1.4 Factory Warehouse Keeper (أمين مخزن المصنع)**
- **Scope**: Factory warehouse
- **Authority Level**: Medium (warehouse operations)
- **Responsibilities**:
  - Manage factory warehouse inventory
  - Receive transfers from central warehouses
  - Approve department/project requisitions (general stock only)
  - Issue materials to departments
  - Stock level monitoring

**Permissions:**
```
✅ View: Factory warehouse inventory
✅ Create: Transfer requests to central, receipt transactions
✅ Approve: Requisitions from general stock only
✅ Issue: General stock materials
⚠️ View only: Commander's reserve (cannot approve/issue)
❌ Manage: Users, projects
```

#### **4.1.5 Department Head (رئيس قسم)**
- **Scope**: Single department
- **Authority Level**: Low-Medium (department operations)
- **Responsibilities**:
  - Department production oversight
  - Material requisition creation
  - Department personnel management
  - Production reporting

**Permissions:**
```
✅ View: Department data, factory warehouse inventory
✅ Create: Requisitions for department needs
✅ Approve: Department-level tasks (non-inventory)
❌ Approve: Inventory requisitions
❌ Issue: Materials
❌ Access: Other departments' data
```

#### **4.1.6 Project Manager (مدير مشروع)**
- **Scope**: Assigned project(s)
- **Authority Level**: Medium (project-specific)
- **Responsibilities**:
  - Project execution
  - Material planning and requisition
  - Budget tracking
  - Progress reporting
  - Project team coordination

**Permissions:**
```
✅ View: Project data, allocated materials, consumption
✅ Create: Requisitions for project needs
✅ Update: Project status, consumption records
✅ Report: Project progress, material usage
❌ Approve: Requisitions
❌ Access: Other projects' data
```

#### **4.1.7 Officer (ضابط)**
- **Scope**: Factory level
- **Authority Level**: Medium (supervisory)
- **Responsibilities**:
  - Operations supervision
  - Department coordination
  - Requisition review
  - Quality control oversight

**Permissions:**
```
✅ View: Factory operations data
✅ Create: Requisitions
✅ Approve: Department-level requisitions (general stock)
❌ Approve: Commander's reserve requests
❌ Manage: Users, factories
```

#### **4.1.8 Civil Engineer (مهندس مدني)**
- **Scope**: Assigned projects/departments
- **Authority Level**: Low-Medium (technical)
- **Responsibilities**:
  - Technical specifications
  - Material requirements planning
  - Quality verification
  - Technical documentation

**Permissions:**
```
✅ View: Project technical data, material specs
✅ Create: Technical requisitions, specifications
✅ Update: Technical documentation
❌ Approve: Requisitions
❌ Financial: Budget data
```

#### **4.1.9 Worker (عامل)**
- **Scope**: Very limited
- **Authority Level**: Minimal (read-only)
- **Responsibilities**:
  - Production tasks
  - Material consumption recording (supervised)
  - Basic data entry

**Permissions:**
```
✅ View: Assigned tasks, limited inventory data
✅ Create: Consumption records (with approval)
❌ Create: Requisitions
❌ View: Financial data, sensitive information
```

#### **4.1.10 Auditor (مراقب/مراجع)**
- **Scope**: Entire complex (read-only)
- **Authority Level**: View-only (oversight)
- **Responsibilities**:
  - Compliance monitoring
  - Audit trail review
  - Report generation
  - Anomaly detection

**Permissions:**
```
✅ View: ALL data (read-only across complex)
✅ Export: Reports, audit trails
✅ Search: Full transaction history
❌ Create: Any transactions
❌ Approve: Anything
❌ Modify: Any data
```

### 4.2 Permission Matrix

| Action | Complex Cmd | Factory Cmd | Central WH | Factory WH | Dept Head | PM | Officer | Engineer | Worker | Auditor |
|--------|-------------|-------------|------------|------------|-----------|----|---------| ---------|--------|---------|
| **View All Factories** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| **View Own Factory** | ✅ | ✅ | ❌ | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ | ✅ |
| **Create Requisition** | ✅ | ✅ | ❌ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| **Approve Requisition (General)** | ✅ | ✅ | ❌ | ✅ | ❌ | ❌ | ✅ | ❌ | ❌ | ❌ |
| **Approve Requisition (Reserve)** | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Issue Materials** | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Transfer Central→Factory** | ✅ | ⚠️ | ✅ | ⚠️ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Create Project** | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Manage Users** | ✅ | ⚠️ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **View Commander's Reserve** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ✅ |
| **Access Commander's Reserve** | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Adjust Inventory** | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **Delete Transactions** | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| **View Audit Trail** | ✅ | ✅ | ⚠️ | ⚠️ | ❌ | ❌ | ⚠️ | ❌ | ❌ | ✅ |
| **Export Reports** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ | ✅ |

**Legend:**
- ✅ = Full permission
- ⚠️ = Limited permission (own scope only)
- ❌ = No permission

---

## 5. Data Model

### 5.1 Entity Relationship Diagram
```
┌─────────────┐
│   Complex   │
└──────┬──────┘
       │
       ├──────┬──────────────┬────────────┐
       │      │              │            │
       ↓      ↓              ↓            ↓
┌──────────┐ ┌──────────┐ ┌─────────┐ ┌──────────┐
│ Factory  │ │ Central  │ │ Supplier│ │  User    │
│          │ │Warehouse │ │         │ │(Complex) │
└────┬─────┘ └────┬─────┘ └─────────┘ └──────────┘
     │            │
     ├────────────┼──────────┬─────────────┐
     │            │          │             │
     ↓            ↓          ↓             ↓
┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│ Factory  │ │  Item    │ │Department│ │ Project  │
│Warehouse │ │          │ │          │ │          │
└────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘
     │            │          │         │
     │            │          │         │
     └────────────┴──────────┴─────────┘
                  │
                  ↓
         ┌─────────────────┐
         │Inventory Record │
         │  ┌────────────┐ │
         │  │  General   │ │
         │  │   Stock    │ │
         │  ├────────────┤ │
         │  │Commander's │ │
         │  │  Reserve⭐ │ │
         │  └────────────┘ │
         └────────┬────────┘
                  │
                  ↓
         ┌─────────────────┐
         │   Transaction   │
         │   (Movements)   │
         └─────────────────┘
                  │
           ┌──────┴──────┐
           ↓             ↓
    ┌─────────────┐ ┌─────────────┐
    │ Requisition │ │  Transfer   │
    └─────────────┘ └─────────────┘
```

### 5.2 Core Entities

#### **5.2.1 Complex (المجمع)**
```typescript
interface Complex {
  id: number;
  code: string;              // "MEC-001"
  nameAr: string;            // "مجمع الصناعات الهندسية"
  nameEn: string;            // "Engineering Industries Complex"
  location: string;
  commanderName: string;
  establishedDate: Date;
  
  // Relationships
  factories: Factory[];
  centralWarehouses: CentralWarehouse[];
  suppliers: Supplier[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}
```

**Validation Rules:**
- `code`: Unique, format: `MEC-XXX`
- `nameAr` & `nameEn`: Required, max 200 chars
- One complex per system (singleton)

#### **5.2.2 Factory (المصنع)**
```typescript
interface Factory {
  id: number;
  complexId: number;
  code: string;              // "FAC-ARM-01"
  nameAr: string;            // "مصنع التدريع"
  nameEn: string;            // "Armoring Factory"
  specializationType: FactoryType;
  location: string;
  commanderName: string;
  commanderRank: string;
  isActive: boolean;
  
  // Relationships
  complex: Complex;
  warehouse: FactoryWarehouse;
  departments: Department[];
  projects: Project[];
  personnel: User[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

enum FactoryType {
  ARMORING = "ARMORING",           // تدريع
  WEAPONS = "WEAPONS",             // أسلحة
  ELECTRONICS = "ELECTRONICS",     // إلكترونيات
  ASSEMBLY = "ASSEMBLY",           // تجميع
  MAINTENANCE = "MAINTENANCE"      // صيانة
}
```

**Validation Rules:**
- `code`: Unique, format: `FAC-{TYPE}-{NUMBER}`
- `complexId`: Must exist in Complex table
- `commanderName`: Required
- Must have exactly one warehouse

#### **5.2.3 Warehouse (Abstract Base)**
```typescript
abstract class Warehouse {
  id: number;
  code: string;
  nameAr: string;
  nameEn: string;
  location: string;
  warehouseKeeperId: number;
  type: WarehouseType;
  capacity: number;          // Max storage capacity
  isActive: boolean;
  
  // Relationships
  warehouseKeeper: User;
  inventoryRecords: InventoryRecord[];
  transactions: InventoryTransaction[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

enum WarehouseType {
  CENTRAL = "CENTRAL",
  FACTORY = "FACTORY"
}

// Concrete implementations
class CentralWarehouse extends Warehouse {
  complexId: number;
  complex: Complex;
  supplierContracts: SupplierContract[];
}

class FactoryWarehouse extends Warehouse {
  factoryId: number;
  factory: Factory;
}
```

**Validation Rules:**
- `code`: Unique, format: `WH-{TYPE}-{NUMBER}`
- `warehouseKeeperId`: Must be a user with WarehouseKeeper role
- `capacity`: Positive number, unit-specific
- Each factory must have exactly one warehouse

#### **5.2.4 Item (الصنف/المادة)**
```typescript
interface Item {
  id: number;
  code: string;              // "MTL-ST-001"
  nameAr: string;            // "صاج حديدي"
  nameEn: string;            // "Steel Plate"
  category: ItemCategory;
  subCategory: string;
  natoStockNumber?: string;  // Military standard identifier
  
  // Physical properties
  unitOfMeasure: UnitOfMeasure;
  standardCost: number;
  averageCost: number;       // Moving average
  weight?: number;
  dimensions?: string;       // JSON: {length, width, height}
  
  // Technical specs
  specifications: string;    // JSON technical data
  manufacturer?: string;
  partNumber?: string;
  
  // Storage requirements
  isHazardous: boolean;
  requiresSpecialStorage: boolean;
  storageConditions?: string;
  shelfLife?: number;        // Days
  
  // Stock control
  minimumStockLevel: number;
  maximumStockLevel: number;
  reorderPoint: number;
  reorderQuantity: number;
  leadTimeDays: number;
  
  // Commander's Reserve defaults
  defaultReservePercentage: number;  // e.g., 20%
  minimumReserveQuantity: number;
  
  // Status
  isActive: boolean;
  isDiscontinued: boolean;
  
  // Relationships
  inventoryRecords: InventoryRecord[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

enum ItemCategory {
  RAW_MATERIAL = "RAW_MATERIAL",       // مواد خام
  COMPONENT = "COMPONENT",             // مكونات
  CONSUMABLE = "CONSUMABLE",           // مواد استهلاكية
  TOOL = "TOOL",                       // أدوات
  CHEMICAL = "CHEMICAL",               // مواد كيميائية
  ELECTRONIC = "ELECTRONIC",           // إلكترونيات
  MECHANICAL = "MECHANICAL"            // ميكانيكية
}

enum UnitOfMeasure {
  KG = "KG",           // كيلوجرام
  TON = "TON",         // طن
  METER = "METER",     // متر
  LITER = "LITER",     // لتر
  PIECE = "PIECE",     // قطعة
  BOX = "BOX",         // صندوق
  ROLL = "ROLL",       // لفة
  SHEET = "SHEET"      // ورقة
}
```

**Validation Rules:**
- `code`: Unique, format: `{CATEGORY}-{SUBCATEGORY}-{NUMBER}`
- `nameAr` & `nameEn`: Required, max 200 chars
- `standardCost`: ≥ 0
- `minimumStockLevel` < `reorderPoint` < `maximumStockLevel`
- `defaultReservePercentage`: 0-100
- If `isHazardous`, `storageConditions` required

#### **5.2.5 InventoryRecord (سجل المخزون)**
```typescript
interface InventoryRecord {
  id: number;
  warehouseId: number;
  itemId: number;
  
  // ⭐ CRITICAL: Commander's Reserve Tracking
  totalQuantity: number;               // Sum of general + reserve
  generalQuantity: number;             // Available for normal use
  commanderReserveQuantity: number;    // Emergency/Strategic stock
  
  // Allocation tracking
  generalAllocated: number;            // Promised from general
  reserveAllocated: number;            // Promised from reserve
  
  // Computed fields
  generalAvailable: number;            // = generalQuantity - generalAllocated
  reserveAvailable: number;            // = commanderReserveQuantity - reserveAllocated
  
  // Thresholds
  minimumReserveRequired: number;      // Alert if reserve < this
  reorderPoint: number;                // From Item defaults
  
  // Physical location
  batchNumber?: string;
  lotNumber?: string;
  serialNumber?: string;
  expiryDate?: Date;
  storageLocation: string;             // "A-12-3" (Aisle-Rack-Shelf)
  
  // Costing
  averageCost: number;                 // Moving average
  lastCost: number;
  totalValue: number;                  // totalQuantity × averageCost
  
  // Status
  status: InventoryStatus;
  lastPhysicalCount?: Date;
  lastCountBy?: number;
  
  // Relationships
  warehouse: Warehouse;
  item: Item;
  transactions: InventoryTransaction[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

enum InventoryStatus {
  OK = "OK",                    // Normal
  LOW = "LOW",                  // Below reorder point
  CRITICAL = "CRITICAL",        // Below minimum
  RESERVE_LOW = "RESERVE_LOW",  // Reserve below threshold
  EXPIRED = "EXPIRED",          // Past expiry date
  QUARANTINE = "QUARANTINE"     // Quality hold
}
```

**Validation Rules:**
- `totalQuantity` = `generalQuantity` + `commanderReserveQuantity` (enforced by DB constraint)
- `generalAllocated` ≤ `generalQuantity`
- `reserveAllocated` ≤ `commanderReserveQuantity`
- `warehouseId` + `itemId` = Unique (one record per item per warehouse)
- If `expiryDate` exists, `status` auto-updates to EXPIRED when date passes
- `minimumReserveRequired` ≤ `commanderReserveQuantity` (else alert)

#### **5.2.6 InventoryTransaction (حركة المخزون)**
```typescript
interface InventoryTransaction {
  id: number;
  transactionNumber: string;    // Auto-generated: TXN-20250130-0001
  transactionType: TransactionType;
  transactionDate: Date;
  
  // Item & Location
  itemId: number;
  warehouseId: number;
  quantity: number;
  
  // Source/Destination tracking
  sourceWarehouseId?: number;        // For transfers
  destinationWarehouseId?: number;   // For transfers
  projectId?: number;                // For project consumption
  departmentId?: number;             // For department issues
  supplierId?: number;               // For receipts
  
  // ⭐ Commander's Reserve Tracking
  isFromCommanderReserve: boolean;
  commanderApprovalId?: number;      // Required if isFromCommanderReserve
  commanderApprovalDate?: Date;
  
  // Financial
  unitCost: number;
  totalCost: number;                 // quantity × unitCost
  
  // Reference documents
  referenceType?: ReferenceType;     // PO, Requisition, Transfer Order
  referenceNumber?: string;
  
  // Physical details
  batchNumber?: string;
  lotNumber?: string;
  expiryDate?: Date;
  
  // Additional info
  notes?: string;
  attachments?: string;              // JSON array of file URLs
  
  // Status
  status: TransactionStatus;
  isReversed: boolean;
  reversedBy?: number;
  reversedDate?: Date;
  reversingTransactionId?: number;   // Points to reversing TXN
  
  // Relationships
  item: Item;
  warehouse: Warehouse;
  sourceWarehouse?: Warehouse;
  destinationWarehouse?: Warehouse;
  project?: Project;
  department?: Department;
  supplier?: Supplier;
  commanderApproval?: User;
  createdByUser: User;
  
  // Audit
  createdAt: Date;
  createdBy: number;
}

enum TransactionType {
  RECEIPT = "RECEIPT",               // استلام (from supplier)
  ISSUE = "ISSUE",                   // صرف (to project/department)
  TRANSFER_OUT = "TRANSFER_OUT",     // تحويل صادر
  TRANSFER_IN = "TRANSFER_IN",       // تحويل وارد
  ADJUSTMENT_PLUS = "ADJUSTMENT_PLUS",   // تسوية بالزيادة
  ADJUSTMENT_MINUS = "ADJUSTMENT_MINUS", // تسوية بالنقص
  RETURN = "RETURN",                 // مرتجع
  CONSUMPTION = "CONSUMPTION",       // استهلاك
  PHYSICAL_COUNT = "PHYSICAL_COUNT"  // جرد
}

enum ReferenceType {
  PURCHASE_ORDER = "PO",
  REQUISITION = "REQ",
  TRANSFER_ORDER = "TO",
  ADJUSTMENT = "ADJ",
  PHYSICAL_COUNT = "PC"
}

enum TransactionStatus {
  DRAFT = "DRAFT",
  PENDING = "PENDING",
  COMPLETED = "COMPLETED",
  CANCELLED = "CANCELLED",
  REVERSED = "REVERSED"
}
```

**Validation Rules:**
- `transactionNumber`: Unique, auto-generated
- `quantity`: > 0
- `unitCost`: ≥ 0
- `totalCost` = `quantity` × `unitCost`
- If `isFromCommanderReserve` = true, `commanderApprovalId` MUST be set
- `commanderApprovalId` must be user with FactoryCommander or ComplexCommander role
- If `transactionType` = TRANSFER_*, both `sourceWarehouseId` and `destinationWarehouseId` required
- If `isReversed` = true, `reversedBy` and `reversedDate` required
- Cannot reverse a transaction that's already reversed

**Business Logic:**
```typescript
// After transaction creation, update InventoryRecord:
if (transactionType === 'RECEIPT' || transactionType === 'TRANSFER_IN') {
  // Increase stock (split into general + reserve based on item defaults)
  const reservePortion = quantity * item.defaultReservePercentage / 100;
  const generalPortion = quantity - reservePortion;
  
  inventoryRecord.generalQuantity += generalPortion;
  inventoryRecord.commanderReserveQuantity += reservePortion;
  inventoryRecord.totalQuantity += quantity;
}

if (transactionType === 'ISSUE' || transactionType === 'TRANSFER_OUT' || transactionType === 'CONSUMPTION') {
  // Decrease stock
  if (isFromCommanderReserve) {
    inventoryRecord.commanderReserveQuantity -= quantity;
  } else {
    inventoryRecord.generalQuantity -= quantity;
  }
  inventoryRecord.totalQuantity -= quantity;
}

if (transactionType === 'ADJUSTMENT_PLUS' || transactionType === 'ADJUSTMENT_MINUS') {
  // Adjust based on physical count
  // ... complex reconciliation logic
}
```

#### **5.2.7 Requisition (طلب)**
```typescript
interface Requisition {
  id: number;
  requestNumber: string;         // Auto: REQ-20250130-0001
  requisitionType: RequisitionType;
  
  // Requester info
  requestedById: number;
  requestedDate: Date;
  requiredDate: Date;            // When needed
  priority: PriorityLevel;
  
  // Context
  projectId?: number;            // If for a project
  departmentId?: number;         // If for a department
  sourceWarehouseId: number;     // Where to get materials from
  
  // Purpose
  purpose: string;               // Detailed justification
  workOrderNumber?: string;
  
  // Approval workflow
  status: RequisitionStatus;
  requiresCommanderApproval: boolean;  // ⭐ Auto-set if any item from reserve
  
  // Approval chain (can be multiple approvers)
  approvals: RequisitionApproval[];
  
  // Fulfillment
  fulfilledDate?: Date;
  fulfilledBy?: number;
  partiallyFulfilled: boolean;
  
  // Relationships
  requestedBy: User;
  project?: Project;
  department?: Department;
  sourceWarehouse: Warehouse;
  items: RequisitionItem[];
  transactions: InventoryTransaction[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

interface RequisitionItem {
  id: number;
  requisitionId: number;
  itemId: number;
  
  // Quantities
  requestedQuantity: number;
  approvedQuantity: number;      // May be less than requested
  issuedQuantity: number;        // Actual fulfilled
  remainingQuantity: number;     // approved - issued
  
  // ⭐ Commander's Reserve flag
  requestFromReserve: boolean;   // User explicitly requests from reserve
  approvedFromReserve: boolean;  // Commander approves reserve use
  
  // Purpose for this specific item
  itemPurpose?: string;
  
  // Status
  itemStatus: RequisitionItemStatus;
  
  // Relationships
  requisition: Requisition;
  item: Item;
  
  // Audit
  createdAt: Date;
  updatedAt: Date;
}

interface RequisitionApproval {
  id: number;
  requisitionId: number;
  approverId: number;
  approvalLevel: ApprovalLevel;
  
  decision: ApprovalDecision;
  decisionDate: Date;
  comments?: string;
  
  // Relationships
  requisition: Requisition;
  approver: User;
}

enum RequisitionType {
  INTERNAL = "INTERNAL",         // داخلي (within factory)
  TRANSFER = "TRANSFER",         // تحويل (from central warehouse)
  PROJECT = "PROJECT",           // مشروع
  MAINTENANCE = "MAINTENANCE",   // صيانة
  EMERGENCY = "EMERGENCY"        // طوارئ
}

enum RequisitionStatus {
  DRAFT = "DRAFT",               // مسودة
  SUBMITTED = "SUBMITTED",       // مقدم
  PENDING_WAREHOUSE = "PENDING_WAREHOUSE",
  PENDING_COMMANDER = "PENDING_COMMANDER",  // ⭐ Requires commander
  APPROVED = "APPROVED",
  PARTIALLY_FULFILLED = "PARTIALLY_FULFILLED",
  FULFILLED = "FULFILLED",
  REJECTED = "REJECTED",
  CANCELLED = "CANCELLED"
}

enum RequisitionItemStatus {
  PENDING = "PENDING",
  APPROVED = "APPROVED",
  PARTIALLY_ISSUED = "PARTIALLY_ISSUED",
  FULLY_ISSUED = "FULLY_ISSUED",
  REJECTED = "REJECTED",
  OUT_OF_STOCK = "OUT_OF_STOCK"
}

enum PriorityLevel {
  LOW = "LOW",
  NORMAL = "NORMAL",
  HIGH = "HIGH",
  EMERGENCY = "EMERGENCY"
}

enum ApprovalLevel {
  WAREHOUSE_KEEPER = "WAREHOUSE_KEEPER",
  OFFICER = "OFFICER",
  FACTORY_COMMANDER = "FACTORY_COMMANDER",
  COMPLEX_COMMANDER = "COMPLEX_COMMANDER"
}

enum ApprovalDecision {
  PENDING = "PENDING",
  APPROVED = "APPROVED",
  REJECTED = "REJECTED",
  APPROVED_PARTIAL = "APPROVED_PARTIAL"
}
```

**Validation Rules:**
- `requestNumber`: Unique, auto-generated
- `requiredDate` ≥ `requestedDate`
- `sourceWarehouseId`: Must exist and be accessible to requester
- If any `RequisitionItem.requestFromReserve` = true:
  - `requiresCommanderApproval` = true
  - Must have approval from Factory/Complex Commander
- `approvedQuantity` ≤ `requestedQuantity`
- `issuedQuantity` ≤ `approvedQuantity`
- Cannot fulfill if `status` != APPROVED

**Approval Workflow Logic:**
```typescript
// Auto-routing based on requirements
function determineApprovalRoute(requisition: Requisition): ApprovalLevel[] {
  const route: ApprovalLevel[] = [];
  
  // Check if any item requests commander's reserve
  const hasReserveRequest = requisition.items.some(i => i.requestFromReserve);
  
  if (hasReserveRequest) {
    // ⭐ MUST go to commander
    route.push(ApprovalLevel.FACTORY_COMMANDER);
  } else {
    // Normal approval path
    if (requisition.requisitionType === 'TRANSFER') {
      route.push(ApprovalLevel.WAREHOUSE_KEEPER); // Central WH keeper
    } else {
      route.push(ApprovalLevel.WAREHOUSE_KEEPER); // Factory WH keeper
    }
  }
  
  // High priority or large value might need officer review
  if (requisition.priority === 'HIGH' || requisition.priority === 'EMERGENCY') {
    route.push(ApprovalLevel.OFFICER);
  }
  
  return route;
}
```

#### **5.2.8 Project (المشروع)**
```typescript
interface Project {
  id: number;
  projectNumber: string;         // PRJ-2025-001
  nameAr: string;                // "مشروع تصنيع مدرعة نمر"
  nameEn: string;                // "Nimr Armored Vehicle Production"
  factoryId: number;
  
  // Timeline
  startDate: Date;
  plannedEndDate: Date;
  actualEndDate?: Date;
  status: ProjectStatus;
  
  // Financial
  budget: number;
  allocatedCost: number;         // Cost of reserved materials
  consumedCost: number;          // Cost of used materials
  remainingBudget: number;       // budget - consumedCost
  
  // Production targets
  targetQuantity: number;        // How many units to produce
  producedQuantity: number;      // How many completed
  inProgressQuantity: number;
  
  // Management
  projectManagerId: number;
  priority: PriorityLevel;
  clientReference?: string;      // Military unit reference
  
  // Description
  description: string;
  technicalSpecs?: string;       // JSON
  deliveryLocation?: string;
  
  // Relationships
  factory: Factory;
  projectManager: User;
  itemAllocations: ProjectItemAllocation[];
  requisitions: Requisition[];
  transactions: InventoryTransaction[];
  team: ProjectTeamMember[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

interface ProjectItemAllocation {
  id: number;
  projectId: number;
  itemId: number;
  
  // Bill of Materials (BOM) planning
  plannedQuantity: number;       // What was estimated
  allocatedQuantity: number;     // What's reserved
  consumedQuantity: number;      // What's used
  remainingQuantity: number;     // allocated - consumed
  
  // Costing
  estimatedCost: number;         // plannedQuantity × estimatedUnitCost
  allocatedCost: number;         // allocatedQuantity × actualUnitCost
  consumedCost: number;          // consumedQuantity × actualUnitCost
  
  // Status
  allocationStatus: AllocationStatus;
  
  // Relationships
  project: Project;
  item: Item;
  
  // Audit
  createdAt: Date;
  updatedAt: Date;
}

interface ProjectTeamMember {
  id: number;
  projectId: number;
  userId: number;
  role: ProjectRole;
  assignedDate: Date;
  isActive: boolean;
  
  project: Project;
  user: User;
}

enum ProjectStatus {
  PLANNING = "PLANNING",         // تخطيط
  APPROVED = "APPROVED",         // موافق عليه
  IN_PROGRESS = "IN_PROGRESS",   // قيد التنفيذ
  ON_HOLD = "ON_HOLD",           // متوقف مؤقتاً
  COMPLETED = "COMPLETED",       // مكتمل
  CANCELLED = "CANCELLED",       // ملغي
  DELIVERED = "DELIVERED"        // تم التسليم
}

enum AllocationStatus {
  PLANNED = "PLANNED",           // مخطط
  ALLOCATED = "ALLOCATED",       // محجوز
  PARTIALLY_CONSUMED = "PARTIALLY_CONSUMED",
  FULLY_CONSUMED = "FULLY_CONSUMED",
  EXCESS = "EXCESS"              // Allocated more than consumed
}

enum ProjectRole {
  MANAGER = "MANAGER",
  ENGINEER = "ENGINEER",
  SUPERVISOR = "SUPERVISOR",
  WORKER = "WORKER"
}
```

**Validation Rules:**
- `projectNumber`: Unique, auto-generated
- `startDate` < `plannedEndDate`
- `budget` > 0
- `allocatedCost` ≤ `budget`
- `consumedCost` ≤ `allocatedCost`
- `producedQuantity` ≤ `targetQuantity`
- `projectManagerId`: Must be user with ProjectManager role
- `factoryId`: Must exist

**Business Logic:**
```typescript
// When allocating materials to project:
function allocateMaterialToProject(
  projectId: number,
  itemId: number,
  quantity: number
) {
  // 1. Check if sufficient inventory available
  const inventory = getInventoryRecord(warehouseId, itemId);
  if (inventory.generalAvailable < quantity) {
    throw new Error("Insufficient inventory");
  }
  
  // 2. Create/update allocation
  const allocation = getOrCreateAllocation(projectId, itemId);
  allocation.allocatedQuantity += quantity;
  allocation.allocatedCost = allocation.allocatedQuantity * item.averageCost;
  
  // 3. Mark inventory as allocated (reserved)
  inventory.generalAllocated += quantity;
  
  // 4. Update project totals
  project.allocatedCost += (quantity * item.averageCost);
}

// When consuming materials:
function consumeMaterial(
  projectId: number,
  itemId: number,
  quantity: number
) {
  const allocation = getAllocation(projectId, itemId);
  
  // Validate
  if (quantity > allocation.remainingQuantity) {
    throw new Error("Cannot consume more than allocated");
  }
  
  // Update allocation
  allocation.consumedQuantity += quantity;
  allocation.remainingQuantity -= quantity;
  allocation.consumedCost += (quantity * item.averageCost);
  
  // Create consumption transaction
  createTransaction({
    type: 'CONSUMPTION',
    itemId,
    quantity,
    projectId,
    // ... other fields
  });
  
  // Update project cost
  project.consumedCost += (quantity * item.averageCost);
  
  // Release allocation (make available for others)
  inventory.generalAllocated -= quantity;
  inventory.generalQuantity -= quantity;  // Actually remove from stock
}
```

#### **5.2.9 Department (القسم)**
```typescript
interface Department {
  id: number;
  factoryId: number;
  code: string;                  // DEPT-ARM-WELDING
  nameAr: string;                // "قسم اللحام"
  nameEn: string;                // "Welding Department"
  departmentType: DepartmentType;
  
  // Management
  headId: number;                // Department head
  isActive: boolean;
  
  // Location
  buildingNumber?: string;
  floorNumber?: string;
  
  // Relationships
  factory: Factory;
  head: User;
  personnel: User[];
  requisitions: Requisition[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

enum DepartmentType {
  PRODUCTION = "PRODUCTION",     // إنتاج
  ASSEMBLY = "ASSEMBLY",         // تجميع
  WELDING = "WELDING",           // لحام
  MACHINING = "MACHINING",       // تشغيل
  PAINTING = "PAINTING",         // دهان
  QC = "QC",                     // مراقبة جودة
  MAINTENANCE = "MAINTENANCE",   // صيانة
  ADMIN = "ADMIN"                // إداري
}
```

#### **5.2.10 User (المستخدم)**
```typescript
interface User {
  id: number;
  
  // Military ID
  militaryId: string;            // Unique military identifier
  nationalId: string;            // National ID number
  
  // Personal info
  fullNameAr: string;            // "محمد أحمد علي"
  fullNameEn: string;            // "Mohamed Ahmed Ali"
  rank: MilitaryRank;            // Military rank
  
  // Contact
  email: string;
  phoneNumber: string;
  alternatePhone?: string;
  
  // Assignment
  factoryId?: number;            // Assigned factory (null for complex-level)
  departmentId?: number;         // Assigned department
  
  // Role & Permissions
  role: UserRole;
  permissions: Permission[];
  isActive: boolean;
  
  // Authentication
  passwordHash: string;
  mustChangePassword: boolean;
  lastLogin?: Date;
  failedLoginAttempts: number;
  isLocked: boolean;
  
  // Relationships
  factory?: Factory;
  department?: Department;
  managedWarehouses: Warehouse[];
  managedProjects: Project[];
  requisitions: Requisition[];
  approvals: RequisitionApproval[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

enum MilitaryRank {
  // Officers
  GENERAL = "GENERAL",           // فريق
  LIEUTENANT_GENERAL = "LT_GENERAL",  // لواء
  MAJOR_GENERAL = "MAJ_GENERAL", // عميد
  BRIGADIER = "BRIGADIER",       // عقيد
  COLONEL = "COLONEL",           // مقدم
  MAJOR = "MAJOR",               // رائد
  CAPTAIN = "CAPTAIN",           // نقيب
  FIRST_LIEUTENANT = "1LT",      // ملازم أول
  LIEUTENANT = "LT",             // ملازم
  
  // NCOs & Soldiers
  SERGEANT_MAJOR = "SGT_MAJ",    // رقيب أول
  SERGEANT = "SERGEANT",         // رقيب
  CORPORAL = "CORPORAL",         // عريف
  PRIVATE = "PRIVATE",           // جندي
  
  // Civilian
  CIVILIAN = "CIVILIAN"          // مدني
}

enum UserRole {
  COMPLEX_COMMANDER = "COMPLEX_COMMANDER",
  FACTORY_COMMANDER = "FACTORY_COMMANDER",
  CENTRAL_WAREHOUSE_KEEPER = "CENTRAL_WAREHOUSE_KEEPER",
  FACTORY_WAREHOUSE_KEEPER = "FACTORY_WAREHOUSE_KEEPER",
  DEPARTMENT_HEAD = "DEPARTMENT_HEAD",
  PROJECT_MANAGER = "PROJECT_MANAGER",
  OFFICER = "OFFICER",
  CIVIL_ENGINEER = "CIVIL_ENGINEER",
  WORKER = "WORKER",
  AUDITOR = "AUDITOR"
}

enum Permission {
  // Inventory
  VIEW_ALL_INVENTORY = "VIEW_ALL_INVENTORY",
  VIEW_FACTORY_INVENTORY = "VIEW_FACTORY_INVENTORY",
  VIEW_WAREHOUSE_INVENTORY = "VIEW_WAREHOUSE_INVENTORY",
  CREATE_REQUISITION = "CREATE_REQUISITION",
  APPROVE_REQUISITION_GENERAL = "APPROVE_REQUISITION_GENERAL",
  APPROVE_REQUISITION_RESERVE = "APPROVE_REQUISITION_RESERVE",
  ISSUE_MATERIALS = "ISSUE_MATERIALS",
  RECEIVE_MATERIALS = "RECEIVE_MATERIALS",
  ADJUST_INVENTORY = "ADJUST_INVENTORY",
  
  // Transfers
  CREATE_TRANSFER = "CREATE_TRANSFER",
  APPROVE_TRANSFER = "APPROVE_TRANSFER",
  
  // Projects
  VIEW_ALL_PROJECTS = "VIEW_ALL_PROJECTS",
  VIEW_FACTORY_PROJECTS = "VIEW_FACTORY_PROJECTS",
  VIEW_OWN_PROJECTS = "VIEW_OWN_PROJECTS",
  CREATE_PROJECT = "CREATE_PROJECT",
  MANAGE_PROJECT = "MANAGE_PROJECT",
  
  // Users
  VIEW_ALL_USERS = "VIEW_ALL_USERS",
  MANAGE_USERS = "MANAGE_USERS",
  
  // Reports
  VIEW_REPORTS = "VIEW_REPORTS",
  EXPORT_REPORTS = "EXPORT_REPORTS",
  
  // Audit
  VIEW_AUDIT_TRAIL = "VIEW_AUDIT_TRAIL",
  
  // ⭐ Commander's Reserve
  VIEW_COMMANDER_RESERVE = "VIEW_COMMANDER_RESERVE",
  ACCESS_COMMANDER_RESERVE = "ACCESS_COMMANDER_RESERVE"
}
```

**Validation Rules:**
- `militaryId`: Unique
- `email`: Unique, valid format
- `role`: Must match one of UserRole enum
- If `role` = FACTORY_WAREHOUSE_KEEPER, `factoryId` required
- If `role` = DEPARTMENT_HEAD, `departmentId` required
- `permissions`: Auto-assigned based on role (can be customized)
- `failedLoginAttempts`: Lock after 5 attempts

#### **5.2.11 Supplier (المورد)**
```typescript
interface Supplier {
  id: number;
  code: string;                  // SUP-001
  nameAr: string;
  nameEn: string;
  supplierType: SupplierType;
  
  // Contact
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
  
  // Business info
  taxId?: string;
  bankAccount?: string;
  paymentTerms: PaymentTerms;
  creditLimit?: number;
  
  // Performance
  rating: number;                // 1-5
  isApproved: boolean;
  isActive: boolean;
  
  // Relationships
  purchaseOrders: PurchaseOrder[];
  contracts: SupplierContract[];
  
  // Audit
  createdAt: Date;
  createdBy: number;
  updatedAt: Date;
  updatedBy: number;
}

enum SupplierType {
  LOCAL = "LOCAL",
  INTERNATIONAL = "INTERNATIONAL",
  GOVERNMENT = "GOVERNMENT"
}

enum PaymentTerms {
  NET_30 = "NET_30",
  NET_60 = "NET_60",
  NET_90 = "NET_90",
  CASH_ON_DELIVERY = "COD",
  ADVANCE_PAYMENT = "ADVANCE"
}
```

---

### 5.3 Database Constraints & Indexes
```sql
-- Critical constraints
ALTER TABLE InventoryRecords
ADD CONSTRAINT CHK_TotalQuantity 
CHECK (TotalQuantity = GeneralQuantity + CommanderReserveQuantity);

ALTER TABLE InventoryRecords
ADD CONSTRAINT CHK_AllocatedQuantity 
CHECK (GeneralAllocated <= GeneralQuantity 
   AND ReserveAllocated <= CommanderReserveQuantity);

ALTER TABLE InventoryTransactions
ADD CONSTRAINT CHK_CommanderReserveApproval
CHECK (
  (IsFromCommanderReserve = FALSE) OR 
  (IsFromCommanderReserve = TRUE AND CommanderApprovalId IS NOT NULL)
);

-- Performance indexes
CREATE INDEX IDX_Inventory_Warehouse_Item 
ON InventoryRecords(WarehouseId, ItemId);

CREATE INDEX IDX_Transaction_Date 
ON InventoryTransactions(TransactionDate DESC);

CREATE INDEX IDX_Transaction_Warehouse_Item_Date
ON InventoryTransactions(WarehouseId, ItemId, TransactionDate);

CREATE INDEX IDX_Requisition_Status_Date
ON Requisitions(Status, RequestedDate DESC);

CREATE INDEX IDX_Project_Factory_Status
ON Projects(FactoryId, Status);

-- Unique constraints
CREATE UNIQUE INDEX UQ_Inventory_Warehouse_Item
ON InventoryRecords(WarehouseId, ItemId);

CREATE UNIQUE INDEX UQ_User_MilitaryId
ON Users(MilitaryId);

CREATE UNIQUE INDEX UQ_Item_Code
ON Items(Code);
```

---

## 6. Business Workflows

### 6.1 Material Receipt Workflow (from Supplier)
```
┌─────────────────────┐
│ 1. Purchase Order   │
│    Created          │
└──────────┬──────────┘
           │
           ↓
┌─────────────────────┐
│ 2. Supplier         │
│    Delivers         │
└──────────┬──────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 3. Central Warehouse Keeper         │
│    - Inspects quality               │
│    - Counts quantity                │
│    - Checks against PO              │
└──────────┬──────────────────────────┘
           │
           ↓ (If Accept)
┌─────────────────────────────────────┐
│ 4. Create Receipt Transaction       │
│    - Split into General + Reserve   │
│    - Update inventory record        │
│    - Generate receipt document      │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 5. Update Financial Records         │
│    - Record cost                    │
│    - Update averageCost             │
└─────────────────────────────────────┘
```

**Detailed Steps:**

**Step 1: Create Purchase Order**
- Purchasing officer creates PO based on reorder points
- Specifies items, quantities, unit costs
- Sends to supplier

**Step 2: Delivery**
- Supplier delivers materials
- Includes delivery note and invoice

**Step 3: Inspection & Counting**
```typescript
interface ReceiptInspection {
  poNumber: string;
  deliveryNote: string;
  inspector: User;
  inspectionDate: Date;
  items: {
    itemId: number;
    orderedQty: number;
    deliveredQty: number;
    acceptedQty: number;
    rejectedQty: number;
    rejectionReason?: string;
  }[];
  overallDecision: 'ACCEPT' | 'REJECT' | 'PARTIAL';
}
```

**Step 4: Create Receipt Transaction**
```typescript
async function processReceipt(inspection: ReceiptInspection) {
  for (const item of inspection.items) {
    if (item.acceptedQty > 0) {
      // Calculate split
      const itemDetails = await getItem(item.itemId);
      const reservePortion = item.acceptedQty * (itemDetails.defaultReservePercentage / 100);
      const generalPortion = item.acceptedQty - reservePortion;
      
      // Create transaction
      await createTransaction({
        type: 'RECEIPT',
        itemId: item.itemId,
        warehouseId: centralWarehouseId,
        quantity: item.acceptedQty,
        isFromCommanderReserve: false,
        referenceType: 'PO',
        referenceNumber: inspection.poNumber,
        unitCost: item.unitCost
      });
      
      // Update inventory
      const inventory = await getOrCreateInventoryRecord(centralWarehouseId, item.itemId);
      inventory.generalQuantity += generalPortion;
      inventory.commanderReserveQuantity += reservePortion;
      inventory.totalQuantity += item.acceptedQty;
      
      // Update moving average cost
      inventory.averageCost = calculateMovingAverage(
        inventory.averageCost,
        inventory.totalQuantity - item.acceptedQty,
        item.unitCost,
        item.acceptedQty
      );
      
      await saveInventory(inventory);
    }
  }
}
```

### 6.2 Transfer Workflow (Central → Factory Warehouse)
```
┌─────────────────────────────────────┐
│ 1. Factory Warehouse Keeper         │
│    Creates Transfer Request         │
│    - Selects items & quantities     │
│    - Provides justification         │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 2. Central Warehouse Keeper Reviews │
│    - Checks availability            │
│    - Verifies justification         │
└──────────┬──────────────────────────┘
           │
           ↓ (If Approve)
┌─────────────────────────────────────┐
│ 3. Create Transfer Transactions     │
│    - Transfer-Out (Central)         │
│    - Transfer-In (Factory)          │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 4. Update Both Inventories          │
│    - Decrease Central (general)     │
│    - Increase Factory (general+res) │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 5. Generate Transfer Document       │
│    - Print for physical handover   │
└─────────────────────────────────────┘
```

**Implementation:**
```typescript
async function createTransferRequest(
  fromWarehouseId: number,
  toWarehouseId: number,
  items: TransferItem[],
  requestedBy: User
) {
  // Validate user authorization
  if (requestedBy.role !== 'FACTORY_WAREHOUSE_KEEPER') {
    throw new Error('Unauthorized');
  }
  
  // Create transfer order
  const transferOrder = await createEntity('TransferOrder', {
    fromWarehouseId,
    toWarehouseId,
    status: 'PENDING',
    requestedBy: requestedBy.id,
    requestDate: new Date(),
    items
  });
  
  return transferOrder;
}

async function approveTransfer(
  transferOrderId: number,
  approver: User
) {
  const transfer = await getTransferOrder(transferOrderId);
  
  // Validate approver
  const fromWarehouse = await getWarehouse(transfer.fromWarehouseId);
  if (approver.id !== fromWarehouse.warehouseKeeperId) {
    throw new Error('Only source warehouse keeper can approve');
  }
  
  // Check availability
  for (const item of transfer.items) {
    const inventory = await getInventoryRecord(transfer.fromWarehouseId, item.itemId);
    if (inventory.generalAvailable < item.quantity) {
      throw new Error(`Insufficient stock for item ${item.itemId}`);
    }
  }
  
  // Create transactions
  for (const item of transfer.items) {
    // Transfer-Out from Central
    await createTransaction({
      type: 'TRANSFER_OUT',
      itemId: item.itemId,
      warehouseId: transfer.fromWarehouseId,
      destinationWarehouseId: transfer.toWarehouseId,
      quantity: item.quantity,
      referenceType: 'TO',
      referenceNumber: transfer.transferNumber
    });
    
    // Transfer-In to Factory
    await createTransaction({
      type: 'TRANSFER_IN',
      itemId: item.itemId,
      warehouseId: transfer.toWarehouseId,
      sourceWarehouseId: transfer.fromWarehouseId,
      quantity: item.quantity,
      referenceType: 'TO',
      referenceNumber: transfer.transferNumber
    });
    
    // Update inventories
    await updateInventoryAfterTransfer(
      transfer.fromWarehouseId,
      transfer.toWarehouseId,
      item.itemId,
      item.quantity
    );
  }
  
  // Update transfer status
  transfer.status = 'COMPLETED';
  transfer.approvedBy = approver.id;
  transfer.approvedDate = new Date();
  await saveTransferOrder(transfer);
}
```

### 6.3 Requisition Workflow (Normal Stock)
```
┌─────────────────────────────────────┐
│ 1. Department Head/PM               │
│    Creates Requisition              │
│    - Selects items from general     │
│    - Provides purpose               │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 2. System Auto-Routes               │
│    → Warehouse Keeper               │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 3. Warehouse Keeper Reviews         │
│    - Checks availability            │
│    - May approve partial quantity   │
└──────────┬──────────────────────────┘
           │
           ↓ (If Approve)
┌─────────────────────────────────────┐
│ 4. Warehouse Keeper Issues Material │
│    - Creates Issue transaction      │
│    - Updates inventory              │
│    - Prints delivery note           │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 5. Requester Acknowledges Receipt   │
│    - Signs delivery note            │
└─────────────────────────────────────┘
```

### 6.4 Requisition Workflow (Commander's Reserve) ⭐
```
┌─────────────────────────────────────┐
│ 1. Department Head/PM               │
│    Creates Requisition              │
│    - Checks "Use Commander Reserve" │
│    - Provides STRONG justification  │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 2. System Auto-Routes               │
│    → Factory Commander              │
│    (Bypasses Warehouse Keeper)      │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 3. Factory Commander Reviews        │
│    - Evaluates justification        │
│    - Checks project priority        │
│    - Verifies emergency need        │
└──────────┬──────────────────────────┘
           │
           ↓ (If Approve)
┌─────────────────────────────────────┐
│ 4. Warehouse Keeper Executes        │
│    - Creates Issue transaction      │
│    - Flags: isFromCommanderReserve  │
│    - References commander approval  │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 5. Special Audit Trail Created      │
│    - Commander approval recorded    │
│    - Extra logging for oversight    │
└─────────────────────────────────────┘
```

**Implementation:**
```typescript
async function createRequisition(
  requestedBy: User,
  items: RequisitionItemRequest[],
  purpose: string,
  projectId?: number
) {
  // Check if any item requests commander's reserve
  const hasReserveRequest = items.some(i => i.useCommanderReserve);
  
  const requisition = await createEntity('Requisition', {
    requestedById: requestedBy.id,
    requestedDate: new Date(),
    status: hasReserveRequest ? 'PENDING_COMMANDER' : 'SUBMITTED',
    requiresCommanderApproval: hasReserveRequest,
    purpose,
    projectId,
    items: items.map(i => ({
      itemId: i.itemId,
      requestedQuantity: i.quantity,
      requestFromReserve: i.useCommanderReserve,
      itemPurpose: i.purpose
    }))
  });
  
  // Auto-route
  if (hasReserveRequest) {
    await notifyFactoryCommander(requisition);
  } else {
    await notifyWarehouseKeeper(requisition);
  }
  
  return requisition;
}

async function approveRequisition(
  requisitionId: number,
  approver: User,
  approvedItems: {itemId: number, approvedQty: number}[]
) {
  const req = await getRequisition(requisitionId);
  
  // Validate approver authority
  if (req.requiresCommanderApproval) {
    if (approver.role !== 'FACTORY_COMMANDER' && 
        approver.role !== 'COMPLEX_COMMANDER') {
      throw new Error('Only commanders can approve reserve requests');
    }
  }
  
  // Create approval record
  await createApproval({
    requisitionId: req.id,
    approverId: approver.id,
    decision: 'APPROVED',
    decisionDate: new Date()
  });
  
  // Update requisition items
  for (const item of approvedItems) {
    const reqItem = req.items.find(i => i.itemId === item.itemId);
    reqItem.approvedQuantity = item.approvedQty;
    reqItem.approvedFromReserve = reqItem.requestFromReserve;
  }
  
  req.status = 'APPROVED';
  await saveRequisition(req);
  
  // Notify warehouse keeper to fulfill
  await notifyWarehouseKeeperToFulfill(req);
}

async function fulfillRequisition(
  requisitionId: number,
  warehouseKeeper: User
) {
  const req = await getRequisition(requisitionId);
  
  if (req.status !== 'APPROVED') {
    throw new Error('Cannot fulfill unapproved requisition');
  }
  
  for (const item of req.items.filter(i => i.approvedQuantity > 0)) {
    // Create issue transaction
    const transaction = await createTransaction({
      type: 'ISSUE',
      itemId: item.itemId,
      warehouseId: req.sourceWarehouseId,
      quantity: item.approvedQuantity,
      projectId: req.projectId,
      departmentId: req.departmentId,
      isFromCommanderReserve: item.approvedFromReserve,
      commanderApprovalId: item.approvedFromReserve ? req.approvals[0].approverId : null,
      referenceType: 'REQ',
      referenceNumber: req.requestNumber,
      createdBy: warehouseKeeper.id
    });
    
    // Update inventory
    const inventory = await getInventoryRecord(req.sourceWarehouseId, item.itemId);
    if (item.approvedFromReserve) {
      inventory.commanderReserveQuantity -= item.approvedQuantity;
    } else {
      inventory.generalQuantity -= item.approvedQuantity;
    }
    inventory.totalQuantity -= item.approvedQuantity;
    await saveInventory(inventory);
    
    // Update requisition item
    item.issuedQuantity = item.approvedQuantity;
    item.itemStatus = 'FULLY_ISSUED';
  }
  
  req.status = 'FULFILLED';
  req.fulfilledDate = new Date();
  req.fulfilledBy = warehouseKeeper.id;
  await saveRequisition(req);
}
```

### 6.5 Project Material Consumption Workflow
```
┌─────────────────────────────────────┐
│ 1. Project Manager Allocates        │
│    - Creates BOM (Bill of Materials)│
│    - Reserves materials from WH     │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 2. Inventory Marked as Allocated    │
│    - Still in warehouse             │
│    - Not available for others       │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 3. Production Begins                │
│    - Workers withdraw materials     │
│    - Supervised by engineer         │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 4. Consumption Recorded             │
│    - Creates Consumption transaction│
│    - Updates project allocation     │
│    - Removes from inventory         │
└──────────┬──────────────────────────┘
           │
           ↓
┌─────────────────────────────────────┐
│ 5. Remaining Materials Handled      │
│    - Excess returned to WH          │
│    - OR kept allocated if ongoing   │
└─────────────────────────────────────┘
```

---

## 7. Technical Requirements

### 7.1 Technology Stack

**Backend:**
```
- Framework: ASP.NET Core 8 Web API
- Language: C# 12
- Database: PostgreSQL 16
- ORM: Entity Framework Core 8
- Authentication: ASP.NET Identity + JWT
- Validation: FluentValidation
- Mapping: AutoMapper
- Logging: Serilog
- API Docs: Swagger/OpenAPI
```

**Frontend:**
```
- Framework: Next.js 14 (App Router)
- Language: TypeScript 5
- UI Library: Shadcn/ui + Radix UI
- Styling: Tailwind CSS
- Data Grid: ag-Grid Enterprise
- State: TanStack Query + Zustand
- Forms: React Hook Form + Zod
- Charts: Recharts
- Icons: Lucide React
```

**DevOps:**
```
- Version Control: Git + GitHub
- CI/CD: GitHub Actions
- Containerization: Docker
- Hosting: Azure / On-Premise
- Monitoring: Application Insights / Grafana
```

### 7.2 Architecture

**Backend: Clean Architecture**
```
API Layer
├── Controllers (HTTP endpoints)
├── DTOs (Request/Response models)
├── Filters (Exception handling, logging)
└── Middleware (Auth, CORS, etc.)

Application Layer
├── Services (Business logic)
├── Interfaces (Contracts)
├── Validators (FluentValidation)
├── Mappings (AutoMapper profiles)
└── Commands/Queries (CQRS pattern - optional)

Domain Layer
├── Entities (Domain models)
├── ValueObjects
├── Enums
├── Exceptions
└── Interfaces (Repository contracts)

Infrastructure Layer
├── Data
│   ├── ApplicationDbContext
│   ├── Configurations (Fluent API)
│   └── Migrations
├── Repositories (EF implementations)
├── Services (External services)
└── Identity (Authentication)
```

**Frontend: Feature-Based Structure**
```
src/
├── app/                  # Next.js App Router
│   ├── (auth)/
│   │   ├── login/
│   │   └── register/
│   ├── (dashboard)/
│   │   ├── layout.tsx
│   │   ├── inventory/
│   │   ├── requisitions/
│   │   ├── projects/
│   │   ├── factories/
│   │   └── reports/
│   └── api/             # API routes (if needed)
│
├── components/
│   ├── ui/              # Shadcn components
│   ├── forms/           # Form components
│   ├── tables/          # Data tables
│   ├── layout/          # Layout components
│   └── shared/          # Reusable components
│
├── lib/
│   ├── api/             # API client (axios)
│   ├── hooks/           # Custom hooks
│   ├── utils/           # Utilities
│   └── validations/     # Zod schemas
│
├── store/               # Zustand stores
│   ├── authStore.ts
│   ├── uiStore.ts
│   └── inventoryStore.ts
│
└── types/               # TypeScript types
    ├── api.ts           # API DTOs
    ├── entities.ts      # Domain models
    └── enums.ts
```

### 7.3 API Design

**RESTful Endpoints:**
```typescript
// Authentication
POST   /api/auth/login
POST   /api/auth/logout
POST   /api/auth/refresh
GET    /api/auth/me

// Inventory
GET    /api/inventory/warehouses/{id}
GET    /api/inventory/warehouses/{id}/items
GET    /api/inventory/items/{id}
GET    /api/inventory/low-stock
GET    /api/inventory/reserve/{warehouseId}  // ⭐ Commander's reserve

// Transactions
GET    /api/transactions
POST   /api/transactions/receipt
POST   /api/transactions/issue
POST   /api/transactions/transfer
POST   /api/transactions/adjust
GET    /api/transactions/{id}

// Requisitions
GET    /api/requisitions
POST   /api/requisitions
GET    /api/requisitions/{id}
PUT    /api/requisitions/{id}
POST   /api/requisitions/{id}/approve
POST   /api/requisitions/{id}/reject
POST   /api/requisitions/{id}/fulfill

// ⭐ Commander's Reserve Special
POST   /api/reserve/release          // Requires commander auth
GET    /api/reserve/pending-approvals
GET    /api/reserve/audit-trail

// Projects
GET    /api/projects
POST   /api/projects
GET    /api/projects/{id}
PUT    /api/projects/{id}
GET    /api/projects/{id}/allocations
POST   /api/projects/{id}/allocate
POST   /api/projects/{id}/consume

// Factories
GET    /api/factories
GET    /api/factories/{id}
GET    /api/factories/{id}/warehouses
GET    /api/factories/{id}/projects

// Departments
GET    /api/departments
GET    /api/departments/{id}

// Users
GET    /api/users
POST   /api/users
GET    /api/users/{id}
PUT    /api/users/{id}

// Reports
GET    /api/reports/inventory-valuation
GET    /api/reports/movement-summary
GET    /api/reports/project-costs
GET    /api/reports/commander-reserve-usage  // ⭐
POST   /api/reports/export
```

**DTO Examples:**
```typescript
// Request DTOs
interface CreateRequisitionRequest {
  projectId?: number;
  departmentId?: number;
  sourceWarehouseId: number;
  priority: PriorityLevel;
  purpose: string;
  requiredDate: string;
  items: {
    itemId: number;
    requestedQuantity: number;
    useCommanderReserve: boolean;  // ⭐
    itemPurpose?: string;
  }[];
}

interface ApproveRequisitionRequest {
  requisitionId: number;
  comments?: string;
  items: {
    itemId: number;
    approvedQuantity: number;
  }[];
}

interface CreateTransferRequest {
  fromWarehouseId: number;
  toWarehouseId: number;
  items: {
    itemId: number;
    quantity: number;
  }[];
  notes?: string;
}

// Response DTOs
interface InventoryRecordResponse {
  id: number;
  warehouse: WarehouseSummary;
  item: ItemSummary;
  totalQuantity: number;
  generalQuantity: number;
  commanderReserveQuantity: number;  // ⭐
  generalAllocated: number;
  reserveAllocated: number;
  generalAvailable: number;
  reserveAvailable: number;
  status: InventoryStatus;
  averageCost: number;
  totalValue: number;
  lastUpdated: string;
}

interface RequisitionResponse {
  id: number;
  requestNumber: string;
  requestedBy: UserSummary;
  requestedDate: string;
  status: RequisitionStatus;
  requiresCommanderApproval: boolean;  // ⭐
  items: RequisitionItemResponse[];
  approvals: ApprovalResponse[];
}
```

### 7.4 Security Requirements

**Authentication:**
- JWT tokens with refresh mechanism
- Token expiry: Access (15 min), Refresh (7 days)
- Military ID + Password
- Account lockout after 5 failed attempts

**Authorization:**
- Role-based access control (RBAC)
- Permission-based granular control
- Row-level security (users only see their scope)
- ⭐ Special policy for Commander's Reserve access

**Data Protection:**
- HTTPS only (TLS 1.3)
- Sensitive data encrypted at rest
- SQL injection prevention (parameterized queries)
- XSS protection (input sanitization)
- CSRF protection
- Rate limiting on API endpoints

**Audit Trail:**
- All transactions logged with user + timestamp
- Immutable audit log (no deletions)
- Track all data modifications
- ⭐ Extra logging for Commander's Reserve access

**Implementation:**
```csharp
// Authorization Policy
services.AddAuthorization(options =>
{
    // ⭐ Commander's Reserve Policy
    options.AddPolicy("AccessCommanderReserve", policy =>
        policy.RequireAssertion(context =>
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            return role == UserRole.FactoryCommander.ToString() ||
                   role == UserRole.ComplexCommander.ToString();
        }));
    
    // Factory-level data access
    options.AddPolicy("FactoryAccess", policy =>
        policy.Requirements.Add(new FactoryAccessRequirement()));
});

// Row-level security example
public class FactoryAccessRequirement : IAuthorizationRequirement { }

public class FactoryAccessHandler : AuthorizationHandler
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        FactoryAccessRequirement requirement)
    {
        var userFactoryId = context.User.FindFirst("FactoryId")?.Value;
        var requestedFactoryId = context.Resource as string;
        
        if (userFactoryId == requestedFactoryId || 
            context.User.IsInRole(UserRole.ComplexCommander.ToString()))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

### 7.5 Performance Requirements

**Response Time:**
- API response < 200ms (95th percentile)
- Page load < 2 seconds
- Data grid rendering < 500ms for 1000 rows

**Scalability:**
- Support 500 concurrent users
- Handle 10,000 transactions/day
- Database: 10 million+ transactions

**Optimization Strategies:**
```typescript
// Backend
- Database indexing on frequently queried columns
- Caching frequently accessed data (Redis)
- Pagination for large datasets
- Async processing for heavy operations
- Connection pooling

// Frontend
- React Server Components for initial render
- Client-side caching (TanStack Query)
- Virtual scrolling for large tables (ag-Grid)
- Code splitting and lazy loading
- Image optimization
- Debounced search inputs
```

### 7.6 Data Integrity

**Constraints:**
```sql
-- Prevent negative quantities
ALTER TABLE InventoryRecords
ADD CONSTRAINT CHK_PositiveQuantities 
CHECK (TotalQuantity >= 0 
   AND GeneralQuantity >= 0 
   AND CommanderReserveQuantity >= 0);

-- Ensure proper allocation
ALTER TABLE InventoryRecords
ADD CONSTRAINT CHK_ValidAllocation
CHECK (GeneralAllocated >= 0 
   AND ReserveAllocated >= 0
   AND GeneralAllocated <= GeneralQuantity
   AND ReserveAllocated <= CommanderReserveQuantity);

-- Transaction quantity validation
ALTER TABLE InventoryTransactions
ADD CONSTRAINT CHK_PositiveTransactionQty
CHECK (Quantity > 0);

-- Commander approval required for reserve
ALTER TABLE InventoryTransactions
ADD CONSTRAINT CHK_ReserveApproval
CHECK (
  IsFromCommanderReserve = FALSE OR
  (IsFromCommanderReserve = TRUE AND CommanderApprovalId IS NOT NULL)
);
```

**Concurrency Control:**
```csharp
// Optimistic concurrency with row versioning
public class InventoryRecord
{
    // ... properties ...
    
    [Timestamp]
    public byte[] RowVersion { get; set; }
}

// In service
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException)
{
    // Handle concurrent modification
    // Option 1: Retry
    // Option 2: Notify user
    throw new ConcurrencyException("Inventory was modified by another user");
}
```

### 7.7 Testing Requirements

**Unit Tests:**
```csharp
// Business logic tests
[Fact]
public void CalculateAvailableQuantity_WithAllocations_ReturnsCorrectValue()
{
    // Arrange
    var inventory = new InventoryRecord
    {
        GeneralQuantity = 1000,
        GeneralAllocated = 300
    };
    
    // Act
    var available = inventory.GeneralAvailable;
    
    // Assert
    Assert.Equal(700, available);
}

[Fact]
public void ReleaseCommanderReserve_WithoutApproval_ThrowsException()
{
    // Arrange
    var transaction = new InventoryTransaction
    {
        IsFromCommanderReserve = true,
        CommanderApprovalId = null
    };
    
    // Act & Assert
    Assert.Throws(() => 
        _service.CreateTransaction(transaction));
}
```

**Integration Tests:**
```csharp
[Fact]
public async Task CreateRequisition_WithReserveRequest_RoutesToCommander()
{
    // Arrange
    var request = new CreateRequisitionRequest
    {
        Items = new[]
        {
            new RequisitionItemRequest
            {
                ItemId = 1,
                Quantity = 100,
                UseCommanderReserve = true  // ⭐
            }
        }
    };
    
    // Act
    var result = await _controller.CreateRequisition(request);
    
    // Assert
    var requisition = await _context.Requisitions
        .Include(r => r.Approvals)
        .FirstAsync(r => r.Id == result.Id);
    
    Assert.True(requisition.RequiresCommanderApproval);
    Assert.Equal(RequisitionStatus.PENDING_COMMANDER, requisition.Status);
}
```

**E2E Tests:**
```typescript
// Playwright/Cypress
describe('Commander Reserve Release', () => {
  it('should require commander authentication', async () => {
    // Login as department head
    await login('dept_head', 'password');
    
    // Try to access reserve release page
    await page.goto('/inventory/reserve/release');
    
    // Should redirect to unauthorized
    expect(page.url()).toContain('/unauthorized');
  });
  
  it('commander can release reserve', async () => {
    // Login as factory commander
    await login('factory_commander', 'password');
    
    // Navigate to pending reserve requests
    await page.goto('/requisitions/reserve-pending');
    
    // Approve first request
    await page.click('[data-testid="approve-btn-1"]');
    
    // Verify success
    await expect(page.locator('.success-message')).toBeVisible();
  });
});
```

---

## 8. UI/UX Design Requirements

### 8.1 Design System

**Color Palette:**
```css
/* Military Dark Theme */
:root {
  /* Base colors */
  --military-dark: #1a1f2e;
  --military-darker: #0f1419;
  --military-light: #2d3748;
  
  /* Accent colors */
  --military-accent: #2d5016;     /* Olive green */
  --military-gold: #8b6914;       /* Commander's reserve */
  --military-danger: #7a1a1a;
  --military-success: #1a4d2e;
  --military-warning: #8b6914;
  --military-info: #1a3d5d;
  
  /* Text colors */
  --text-primary: #e2e8f0;
  --text-secondary: #94a3b8;
  --text-muted: #64748b;
  
  /* Grid colors (Excel-like) */
  --grid-border: #404854;
  --grid-header: #252d3d;
  --grid-selected: #2d5016;
  --grid-hover: #1e2532;
  
  /* Status colors */
  --status-ok: #1a4d2e;
  --status-low: #8b6914;
  --status-critical: #7a1a1a;
  --status-reserve: #8b6914;
}
```

**Typography:**
```css
/* Data display (Excel-like monospace) */
.data-cell {
  font-family: 'JetBrains Mono', 'Consolas', monospace;
  font-size: 14px;
  letter-spacing: -0.02em;
}

/* UI text (Arabic-friendly sans-serif) */
body {
  font-family: 'Inter', 'Cairo', sans-serif;
  font-size: 16px;
}

/* Headers */
h1 { font-size: 2rem; font-weight: 700; }
h2 { font-size: 1.5rem; font-weight: 600; }
h3 { font-size: 1.25rem; font-weight: 600; }
```

**Components:**

1. **Military Badge** (for ranks, status)
```tsx`

Continue

11:40 PM

<Badge variant="rank" rank="COLONEL">
  ⭐⭐⭐ عقيد
</Badge>
<Badge variant="reserve">
  ⭐ احتياطي القائد
</Badge>
<Badge variant="status" status="CRITICAL">
  🔴 حرج
</Badge>
````

1. **Data Grid** (ag-Grid configuration)

typescript

`const gridOptions: GridOptions = {
  theme: 'ag-theme-alpine-dark',
  rowHeight: 35,
  headerHeight: 40,
  enableRangeSelection: true,
  enableCellTextSelection: true,
  suppressRowClickSelection: true,
  
  // Excel-like features
  enableFillHandle: true,
  undoRedoCellEditing: true,
  
  // Keyboard navigation
  navigateToNextCell: (params) => {
    // Tab moves to next editable cell
    // Enter moves down
  },
  
  // Column definitions
  columnDefs: [
    {
      field: 'code',
      headerName: 'CODE',
      pinned: 'left',
      width: 120,
      cellClass: 'font-mono'
    },
    {
      field: 'nameEn',
      headerName: 'ITEM NAME',
      flex: 1
    },
    {
      field: 'generalQuantity',
      headerName: 'GENERAL',
      type: 'numericColumn',
      cellClass: 'font-mono text-right',
      valueFormatter: (params) => params.value.toLocaleString()
    },
    {
      field: 'commanderReserveQuantity',
      headerName: 'RESERVE ⭐',
      type: 'numericColumn',
      cellClass: 'font-mono text-right bg-military-gold',
      valueFormatter: (params) => params.value.toLocaleString(),
      cellStyle: { fontWeight: 'bold' }
    },
    {
      field: 'availableQuantity',
      headerName: 'AVAILABLE',
      type: 'numericColumn',
      cellClass: 'font-mono text-right'
    },
    {
      field: 'status',
      headerName: 'STATUS',
      cellRenderer: (params) => {
        const status = params.value;
        const icon = {
          'OK': '🟢',
          'LOW': '🟡',
          'CRITICAL': '🔴',
          'RESERVE_LOW': '🔶'
        }[status];
        return `${icon} ${status}`;
      }
    }
  ]
};`

1. **Command Palette** (Ctrl+K)

tsx

`<Command>
  <CommandInput placeholder="Search commands..." />
  <CommandList>
    <CommandGroup heading="Inventory">
      <CommandItem onSelect={() => navigate('/inventory')}>
        📦 View Inventory
      </CommandItem>
      <CommandItem onSelect={() => navigate('/inventory/low-stock')}>
        ⚠️ Low Stock Alerts
      </CommandItem>
    </CommandGroup>
    <CommandGroup heading="Requisitions">
      <CommandItem onSelect={() => navigate('/requisitions/create')}>
        ➕ Create Requisition
      </CommandItem>
      <CommandItem onSelect={() => navigate('/requisitions/pending')}>
        ⏳ Pending Approvals
      </CommandItem>
    </CommandGroup>
    <CommandGroup heading="Commander ⭐">
      <CommandItem onSelect={() => navigate('/reserve/pending')}>
        ⭐ Reserve Approvals
      </CommandItem>
    </CommandGroup>
  </CommandList>
</Command>`

### 8.2 Key Screens

### **8.2.1 Dashboard (Control Center)**

`export default function DashboardPage() {
  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">
            {factory.nameEn} - Inventory Command Center
          </h1>
          <p className="text-text-secondary">
            Factory Commander: {commander.rank} {commander.fullNameEn}
          </p>
        </div>
        <Badge variant="rank">{commander.rank}</Badge>
      </div>
      
      {/* Key Metrics */}
      <div className="grid grid-cols-4 gap-4">
        <MetricCard
          title="General Stock"
          value="12,450 KG"
          icon="📦"
          trend="+5.2%"
        />
        <MetricCard
          title="Commander's Reserve ⭐"
          value="3,120 KG"
          icon="⭐"
          className="bg-military-gold"
        />
        <MetricCard
          title="Low Stock Alerts"
          value="12 Items"
          icon="⚠️"
          variant="warning"
        />
        <MetricCard
          title="Pending Approvals"
          value="8"
          icon="⏳"
        />
      </div>
      
      {/* Charts */}
      <div className="grid grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Inventory Trend</CardTitle>
          </CardHeader>
          <CardContent>
            <InventoryTrendChart />
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>Reserve Usage ⭐</CardTitle>
          </CardHeader>
          <CardContent>
            <ReserveUsageChart />
          </CardContent>
        </Card>
      </div>
      
      {/* Recent Activity */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Requisitions</CardTitle>
        </CardHeader>
        <CardContent>
          <RecentRequisitionsTable />
        </CardContent>
      </Card>
    </div>
  );
}`

### **8.2.2 Inventory Grid**

`export default function InventoryGridPage() {
  return (
    <div className="h-screen flex flex-col">
      {/* Toolbar */}
      <div className="bg-military-darker p-4 border-b border-grid-border">
        <div className="flex items-center gap-4">
          {/* Filters */}
          <Select>
            <SelectTrigger className="w-48">
              <SelectValue placeholder="Category" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Categories</SelectItem>
              <SelectItem value="raw">Raw Materials</SelectItem>
              <SelectItem value="component">Components</SelectItem>
            </SelectContent>
          </Select>
          
          <Select>
            <SelectTrigger className="w-48">
              <SelectValue placeholder="Location" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All Locations</SelectItem>
              <SelectItem value="central">Central Warehouse</SelectItem>
              <SelectItem value="factory">Factory Warehouse</SelectItem>
            </SelectContent>
          </Select>
          
          {/* Search */}
          <div className="flex-1">
            <Input
              type="search"
              placeholder="🔍 Search items..."
              className="bg-military-dark"
            />
          </div>
          
          {/* Actions */}
          <Button variant="outline">
            📥 Export Excel
          </Button>
          <Button>
            ➕ Create Requisition
          </Button>
        </div>
        
        {/* Quick Filters */}
        <div className="flex gap-2 mt-3">
          <Badge 
            variant="outline" 
            className="cursor-pointer hover:bg-military-light"
            onClick={() => filter('low-stock')}
          >
            ⚠️ Low Stock (12)
          </Badge>
          <Badge 
            variant="outline"
            className="cursor-pointer hover:bg-military-gold"
            onClick={() => filter('reserve-low')}
          >
            ⭐ Reserve Low (5)
          </Badge>
          <Badge 
            variant="outline"
            className="cursor-pointer"
            onClick={() => filter('expiring')}
          >
            📅 Expiring Soon (3)
          </Badge>
        </div>
      </div>
      
      {/* Grid */}
      <div className="flex-1">
        <AgGridReact
          gridOptions={gridOptions}
          rowData={inventoryData}
          className="ag-theme-alpine-dark"
        />
      </div>
      
      {/* Footer Stats */}
      <div className="bg-military-darker p-3 border-t border-grid-border">
        <div className="flex justify-between text-sm text-text-secondary">
          <span>
            Total Items: {inventoryData.length.toLocaleString()}
          </span>
          <span>
            General Stock: {totalGeneral.toLocaleString()} KG
          </span>
          <span className="text-military-gold">
            ⭐ Reserve: {totalReserve.toLocaleString()} KG
          </span>
          <span>
            Total Value: ${totalValue.toLocaleString()}
          </span>
        </div>
      </div>
    </div>
  );
}`

### **8.2.3 Create Requisition Form**

`export default function CreateRequisitionPage() {
  const form = useForm<RequisitionFormData>({
    resolver: zodResolver(requisitionSchema)
  });
  
  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        {/* Header */}
        <Card>
          <CardHeader>
            <CardTitle>Create Material Requisition</CardTitle>
            <CardDescription>
              Request materials from warehouse for project/department use
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            {/* Request Number (Auto-generated) */}
            <FormField
              control={form.control}
              name="requestNumber"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Request Number</FormLabel>
                  <FormControl>
                    <Input {...field} disabled className="bg-military-light" />
                  </FormControl>
                </FormItem>
              )}
            />
            
            {/* Project/Department */}
            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="projectId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Project (Optional)</FormLabel>
                    <Select onValueChange={field.onChange}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select project" />
                      </SelectTrigger>
                      <SelectContent>
                        {projects.map(p => (
                          <SelectItem key={p.id} value={p.id.toString()}>
                            {p.nameEn}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="departmentId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Department (Optional)</FormLabel>
                    <Select onValueChange={field.onChange}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select department" />
                      </SelectTrigger>
                      <SelectContent>
                        {departments.map(d => (
                          <SelectItem key={d.id} value={d.id.toString()}>
                            {d.nameEn}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </FormItem>
                )}
              />
            </div>
            
            {/* Priority & Required Date */}
            <div className="grid grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="priority"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Priority</FormLabel>
                    <RadioGroup onValueChange={field.onChange} defaultValue="NORMAL">
                      <div className="flex items-center space-x-2">
                        <RadioGroupItem value="NORMAL" id="normal" />
                        <Label htmlFor="normal">⚪ Normal</Label>
                      </div>
                      <div className="flex items-center space-x-2">
                        <RadioGroupItem value="HIGH" id="high" />
                        <Label htmlFor="high">🟡 High</Label>
                      </div>
                      <div className="flex items-center space-x-2">
                        <RadioGroupItem value="EMERGENCY" id="emergency" />
                        <Label htmlFor="emergency">🔴 Emergency</Label>
                      </div>
                    </RadioGroup>
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="requiredDate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Required Date</FormLabel>
                    <FormControl>
                      <Input type="date" {...field} />
                    </FormControl>
                  </FormItem>
                )}
              />
            </div>
            
            {/* Purpose */}
            <FormField
              control={form.control}
              name="purpose"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Purpose / Justification</FormLabel>
                  <FormControl>
                    <Textarea 
                      {...field} 
                      rows={3}
                      placeholder="Detailed explanation of why these materials are needed..."
                    />
                  </FormControl>
                  <FormDescription>
                    Provide clear justification, especially for emergency or reserve requests
                  </FormDescription>
                </FormItem>
              )}
            />
          </CardContent>
        </Card>
        
        {/* Items Section */}
        <Card>
          <CardHeader>
            <CardTitle>Requested Items</CardTitle>
            <CardDescription>
              Add items to your requisition. Use ⭐ Commander's Reserve only for emergencies.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {/* Add Item Button */}
              <Button
                type="button"
                variant="outline"
                onClick={openItemSelector}
              >
                ➕ Add Item
              </Button>
              
              {/* Items Table */}
              {form.watch('items')?.length > 0 && (
                <div className="border border-grid-border rounded-lg overflow-hidden">
                  <Table>
                    <TableHeader className="bg-grid-header">
                      <TableRow>
                        <TableHead>Item Code</TableHead>
                        <TableHead>Item Name</TableHead>
                        <TableHead className="text-right">Requested Qty</TableHead>
                        <TableHead className="text-right">Available</TableHead>
                        <TableHead className="text-right">Reserve ⭐</TableHead>
                        <TableHead>From Reserve?</TableHead>
                        <TableHead>Purpose</TableHead>
                        <TableHead></TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {form.watch('items').map((item, index) => (
                        <TableRow key={index}>
                          <TableCell className="font-mono">{item.itemCode}</TableCell>
                          <TableCell>{item.itemName}</TableCell>
                          <TableCell className="text-right font-mono">
                            <Input
                              type="number"
                              {...form.register(`items.${index}.quantity`)}
                              className="w-24 text-right"
                            />
                          </TableCell>
                          <TableCell className="text-right font-mono text-text-secondary">
                            {item.availableGeneral.toLocaleString()}
                          </TableCell>
                          <TableCell className="text-right font-mono text-military-gold">
                            {item.availableReserve.toLocaleString()}
                          </TableCell>
                          <TableCell>
                            <div className="flex items-center gap-2">
                              <Checkbox
                                checked={item.useCommanderReserve}
                                onCheckedChange={(checked) => 
                                  form.setValue(`items.${index}.useCommanderReserve`, checked)
                                }
                              />
                              {item.useCommanderReserve && (
                                <Badge variant="outline" className="bg-military-gold">
                                  ⭐ Reserve
                                </Badge>
                              )}
                            </div>
                          </TableCell>
                          <TableCell>
                            <Input
                              {...form.register(`items.${index}.itemPurpose`)}
                              placeholder="Specific use..."
                              className="w-48"
                            />
                          </TableCell>
                          <TableCell>
                            <Button
                              type="button"
                              variant="ghost"
                              size="sm"
                              onClick={() => removeItem(index)}
                            >
                              🗑️
                            </Button>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              )}
              
              {/* Reserve Warning */}
              {hasReserveItems && (
                <Alert variant="warning" className="bg-military-gold/20 border-military-gold">
                  <AlertTitle className="flex items-center gap-2">
                    ⭐ Commander's Reserve Request
                  </AlertTitle>
                  <AlertDescription>
                    You are requesting items from Commander's Reserve. This requisition will be 
                    routed directly to the Factory Commander for approval. Ensure your justification 
                    is detailed and the need is genuine.
                  </AlertDescription>
                </Alert>
              )}
            </div>
          </CardContent>
        </Card>
        
        {/* Actions */}
        <div className="flex justify-between">
          <Button type="button" variant="outline" onClick={() => router.back()}>
            Cancel
          </Button>
          <div className="flex gap-2">
            <Button type="button" variant="outline" onClick={() => saveDraft()}>
              💾 Save Draft
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? 'Submitting...' : '✅ Submit Request'}
            </Button>
          </div>
        </div>
      </form>
    </Form>
  );
}`

### **8.2.4 Commander's Reserve Approval Screen ⭐**

tsx

`export default function ReserveApprovalPage() {
  const { data: pendingRequisitions } = useQuery({
    queryKey: ['reserve-pending'],
    queryFn: fetchPendingReserveRequisitions
  });
  
  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold flex items-center gap-2">
            ⭐ Commander's Reserve Approvals
          </h1>
          <p className="text-text-secondary">
            Review and approve requisitions requesting materials from emergency reserve
          </p>
        </div>
        <Badge variant="outline" className="text-lg px-4 py-2">
          {pendingRequisitions?.length || 0} Pending
        </Badge>
      </div>
      
      {/* Reserve Status Overview */}
      <Card className="bg-military-gold/10 border-military-gold">
        <CardHeader>
          <CardTitle>Reserve Status</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-4 gap-4">
            <div>
              <p className="text-sm text-text-secondary">Total Reserve</p>
              <p className="text-2xl font-bold text-military-gold">
                {reserveStatus.total.toLocaleString()} KG
              </p>
            </div>
            <div>
              <p className="text-sm text-text-secondary">Allocated</p>
              <p className="text-2xl font-bold">
                {reserveStatus.allocated.toLocaleString()} KG
              </p>
            </div>
            <div>
              <p className="text-sm text-text-secondary">Available</p>
              <p className="text-2xl font-bold text-military-success">
                {reserveStatus.available.toLocaleString()} KG
              </p>
            </div>
            <div>
              <p className="text-sm text-text-secondary">Usage This Month</p>
              <p className="text-2xl font-bold">
                {reserveStatus.usageThisMonth.toLocaleString()} KG
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
      
      {/* Pending Requisitions */}
      <div className="space-y-4">
        {pendingRequisitions?.map(req => (
          <Card key={req.id} className="border-military-gold">
            <CardHeader>
              <div className="flex items-start justify-between">
                <div>
                  <CardTitle className="flex items-center gap-2">
                    {req.requestNumber}
                    <Badge variant="outline" className={getPriorityColor(req.priority)}>
                      {req.priority}
                    </Badge>
                  </CardTitle>
                  <CardDescription>
                    Requested by: {req.requestedBy.rank} {req.requestedBy.fullNameEn}
                    <br />
                    Date: {formatDate(req.requestedDate)} | Required: {formatDate(req.requiredDate)}
                  </CardDescription>
                </div>
                <Badge className="bg-military-gold">
                  ⭐ Commander Approval Required
                </Badge>
              </div>
            </CardHeader>
            <CardContent className="space-y-4">
              {/* Purpose */}
              <div>
                <Label className="text-sm font-semibold">Justification:</Label>
                <p className="mt-1 p-3 bg-military-dark rounded border border-grid-border">
                  {req.purpose}
                </p>
              </div>
              
              {/* Project Context */}
              {req.project && (
                <div>
                  <Label className="text-sm font-semibold">Project:</Label>
                  <p className="mt-1">
                    {req.project.nameEn} | Budget: ${req.project.budget.toLocaleString()} | 
                    Status: {req.project.status}
                  </p>
                </div>
              )}
              
              {/* Reserve Items */}
              <div>
                <Label className="text-sm font-semibold">Items from Reserve:</Label>
                <Table className="mt-2">
                  <TableHeader className="bg-grid-header">
                    <TableRow>
                      <TableHead>Item</TableHead>
                      <TableHead className="text-right">Requested</TableHead>
                      <TableHead className="text-right">Reserve Available</TableHead>
                      <TableHead className="text-right">% of Reserve</TableHead>
                      <TableHead>Purpose</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {req.items.filter(i => i.requestFromReserve).map(item => (
                      <TableRow key={item.id}>
                        <TableCell>
                          {item.item.code} - {item.item.nameEn}
                        </TableCell>
                        <TableCell className="text-right font-mono">
                          {item.requestedQuantity.toLocaleString()} {item.item.unitOfMeasure}
                        </TableCell>
                        <TableCell className="text-right font-mono text-military-gold">
                          {item.reserveAvailable.toLocaleString()}
                        </TableCell>
                        <TableCell className="text-right">
                          {((item.requestedQuantity / item.reserveAvailable) * 100).toFixed(1)}%
                        </TableCell>
                        <TableCell>{item.itemPurpose}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
              
              {/* Actions */}
              <div className="flex justify-between items-center pt-4 border-t border-grid-border">
                <Button
                  variant="outline"
                  onClick={() => viewFullDetails(req.id)}
                >
                  📄 View Full Details
                </Button>
                <div className="flex gap-2">
                  <Dialog>
                    <DialogTrigger asChild>
                      <Button variant="destructive">
                        ❌ Reject
                      </Button>
                    </DialogTrigger>
                    <DialogContent>
                      <DialogHeader>
                        <DialogTitle>Reject Reserve Request</DialogTitle>
                        <DialogDescription>
                          Please provide a reason for rejecting this request
                        </DialogDescription>
                      </DialogHeader>
                      <Textarea
                        placeholder="Reason for rejection..."
                        value={rejectReason}
                        onChange={(e) => setRejectReason(e.target.value)}
                      />
                      <DialogFooter>
                        <Button variant="outline" onClick={closeDialog}>Cancel</Button>
                        <Button 
                          variant="destructive" 
                          onClick={() => rejectRequisition(req.id, rejectReason)}
                        >
                          Confirm Rejection
                        </Button>
                      </DialogFooter>
                    </DialogContent>
                  </Dialog>
                  
                  <Dialog>
                    <DialogTrigger asChild>
                      <Button className="bg-military-gold hover:bg-military-gold/80">
                        ✅ Approve Reserve Release
                      </Button>
                    </DialogTrigger>
                    <DialogContent>
                      <DialogHeader>
                        <DialogTitle>⭐ Approve Commander's Reserve Release</DialogTitle>
                        <DialogDescription>
                          You are about to release materials from emergency reserve
                        </DialogDescription>
                      </DialogHeader>
                      <div className="space-y-4">
                        <Alert>
                          <AlertTitle>Items to be released:</AlertTitle>
                          <AlertDescription>
                            <ul className="mt-2 space-y-1">
                              {req.items.filter(i => i.requestFromReserve).map(item => (
                                <li key={item.id}>
                                  • {item.item.nameEn}: {item.requestedQuantity} {item.item.unitOfMeasure}
                                </li>
                              ))}
                            </ul>
                          </AlertDescription>
                        </Alert>
                        
                        <div>
                          <Label>Commander's Comments (Optional)</Label>
                          <Textarea
                            placeholder="Any additional notes or conditions..."
                            value={approvalComments}
                            onChange={(e) => setApprovalComments(e.target.value)}
                          />
                        </div>
                      </div>
                      <DialogFooter>
                        <Button variant="outline" onClick={closeDialog}>Cancel</Button>
                        <Button 
                          className="bg-military-gold hover:bg-military-gold/80"
                          onClick={() => approveRequisition(req.id, approvalComments)}
                        >
                          ✅ Confirm Approval
                        </Button>
                      </DialogFooter>
                    </DialogContent>
                  </Dialog>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
        
        {pendingRequisitions?.length === 0 && (
          <Card>
            <CardContent className="text-center py-12">
              <p className="text-text-secondary text-lg">
                ✅ No pending reserve approvals
              </p>
            </CardContent>
          </Card>
        )}
      </div>
    </div>
  );
}`

### 8.3 Keyboard Shortcuts

typescript

`// Global shortcuts
const shortcuts = {
  'Ctrl+K': 'Open command palette',
  'Ctrl+/': 'Show shortcuts help',
  'Ctrl+N': 'New requisition',
  'Ctrl+F': 'Focus search',
  'Ctrl+E': 'Export current view',
  
  // Grid navigation
  'Arrow Keys': 'Navigate cells',
  'Tab': 'Move to next editable cell',
  'Shift+Tab': 'Move to previous cell',
  'Enter': 'Edit cell / Move down',
  'Escape': 'Cancel edit',
  'Ctrl+C': 'Copy cell',
  'Ctrl+V': 'Paste cell',
  
  // Commander shortcuts
  'Ctrl+R': 'View reserve status (commanders only)',
  'Ctrl+A': 'Approve selected (commanders only)'
};

// Implementation
useEffect(() => {
  const handleKeyDown = (e: KeyboardEvent) => {
    if (e.ctrlKey && e.key === 'k') {
      e.preventDefault();
      openCommandPalette();
    }
    
    if (e.ctrlKey && e.key === 'n') {
      e.preventDefault();
      router.push('/requisitions/create');
    }
    
    // Commander-only shortcuts
    if (isCommander) {
      if (e.ctrlKey && e.key === 'r') {
        e.preventDefault();
        router.push('/reserve/status');
      }
    }
  };
  
  document.addEventListener('keydown', handleKeyDown);
  return () => document.removeEventListener('keydown', handleKeyDown);
}, []);`

### 8.4 Responsive Design

tsx

`// Mobile/Tablet considerations
const InventoryGridMobile = () => {
  return (
    <div className="lg:hidden">
      {/* Card-based view for mobile */}
      {inventoryData.map(item => (
        <Card key={item.id} className="mb-3">
          <CardHeader>
            <div className="flex justify-between">
              <div>
                <CardTitle className="text-base">{item.code}</CardTitle>
                <CardDescription>{item.nameEn}</CardDescription>
              </div>
              <Badge variant={getStatusVariant(item.status)}>
                {item.status}
              </Badge>
            </div>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 gap-2 text-sm">
              <div>
                <span className="text-text-secondary">General:</span>
                <span className="font-mono ml-2">{item.generalQuantity}</span>
              </div>
              <div>
                <span className="text-text-secondary">Reserve:</span>
                <span className="font-mono ml-2 text-military-gold">
                  ⭐ {item.commanderReserveQuantity}
                </span>
              </div>
              <div>
                <span className="text-text-secondary">Available:</span>
                <span className="font-mono ml-2">{item.availableQuantity}</span>
              </div>
              <div>
                <span className="text-text-secondary">Value:</span>
                <span className="font-mono ml-2">${item.totalValue}</span>
              </div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
};`

---

## 9. Reporting Requirements

### 9.1 Standard Reports

### **9.1.1 Inventory Valuation Report**

typescript

`interface InventoryValuationReport {
  reportDate: Date;
  warehouse: Warehouse;
  summary: {
    totalItems: number;
    totalQuantity: number;
    generalStockValue: number;
    reserveStockValue: number;
    totalValue: number;
  };
  byCategory: {
    category: string;
    itemCount: number;
    totalQuantity: number;
    totalValue: number;
  }[];
  items: {
    code: string;
    name: string;
    quantity: number;
    averageCost: number;
    totalValue: number;
  }[];
}`

### **9.1.2 Movement Summary Report**

typescript

`interface MovementSummaryReport {
  startDate: Date;
  endDate: Date;
  warehouse: Warehouse;
  summary: {
    receipts: { count: number; quantity: number; value: number };
    issues: { count: number; quantity: number; value: number };
    transfers: { count: number; quantity: number; value: number };
    adjustments: { count: number; quantity: number; value: number };
  };
  byItem: {
    itemCode: string;
    itemName: string;
    openingBalance: number;
    receipts: number;
    issues: number;
    transfers: number;
    closingBalance: number;
  }[];
}`

### **9.1.3 Commander's Reserve Usage Report ⭐**

typescript

`interface CommanderReserveUsageReport {
  startDate: Date;
  endDate: Date;
  factory: Factory;
  
  summary: {
    totalReleases: number;
    totalQuantityReleased: number;
    totalValueReleased: number;
    averageApprovalTime: number;  // hours
  };
  
  byProject: {
    projectName: string;
    releasesCount: number;
    quantityReleased: number;
    valueReleased: number;
  }[];
  
  byItem: {
    itemCode: string;
    itemName: string;
    releasesCount: number;
    quantityReleased: number;
    percentOfTotalReserve: number;
  }[];
  
  releases: {
    releaseDate: Date;
    requisitionNumber: string;
    approvedBy: string;  // Commander name
    project: string;
    items: {
      itemName: string;
      quantity: number;
      value: number;
    }[];
    justification: string;
  }[];
  
  trends: {
    month: string;
    releasesCount: number;
    totalValue: number;
  }[];
}`

### **9.1.4 Project Cost Report**

typescript

`interface ProjectCostReport {
  project: Project;
  reportDate: Date;
  
  summary: {
    budget: number;
    allocatedCost: number;
    consumedCost: number;
    remainingBudget: number;
    budgetUtilization: number;  // percentage
  };
  
  materialCosts: {
    itemCode: string;
    itemName: string;
    plannedQuantity: number;
    allocatedQuantity: number;
    consumedQuantity: number;
    estimatedCost: number;
    actualCost: number;
    variance: number;
  }[];
  
  costByPhase: {
    phase: string;
    budgeted: number;
    actual: number;
    variance: number;
  }[];
}`

### **9.1.5 Low Stock Alert Report**

typescript

`interface LowStockAlertReport {
  reportDate: Date;
  criticalItems: {
    itemCode: string;
    itemName: string;
    currentQty: number;
    reorderPoint: number;
    minimumRequired: number;
    daysUntilStockout: number;
    recommendedOrderQty: number;
  }[];
  
  reserveLowItems: {
    itemCode: string;
    itemName: string;
    reserveQty: number;
    minimumReserveRequired: number;
    deficitQty: number;
  }[];
}`

### 9.2 Report Export Formats

- PDF (formatted, print-ready)
- Excel (.xlsx) with formulas
- CSV (raw data)
- JSON (API integration)

### 9.3 Report Scheduling

typescript

`interface ScheduledReport {
  id: number;
  reportType: ReportType;
  schedule: 'DAILY' | 'WEEKLY' | 'MONTHLY';
  recipients: string[];  // Email addresses
  parameters: Record<string, any>;
  isActive: boolean;
}

// Example: Daily low stock alert
const dailyLowStockReport: ScheduledReport = {
  id: 1,
  reportType: 'LOW_STOCK_ALERT',
  schedule: 'DAILY',
  recipients: [
    'warehouse.keeper@military.eg',
    'factory.commander@military.eg'
  ],
  parameters: {
    warehouseId: 1,
    includeReserve: true
  },
  isActive: true
};`

---

## 10. Implementation Roadmap

### Phase 1: Foundation (Weeks 1-4)

**Goal: Setup & Core Infrastructure**

**Week 1: Project Setup**

- [ ]  Initialize Git repository
- [ ]  Setup backend project (ASP.NET Core 8)
- [ ]  Setup frontend project (Next.js 14)
- [ ]  Configure PostgreSQL database
- [ ]  Setup development environment
- [ ]  Configure Docker for local development

**Week 2: Database & Domain Models**

- [ ]  Create all database tables
- [ ]  Configure EF Core relationships
- [ ]  Seed initial data (test factories, items, users)
- [ ]  Create domain entities
- [ ]  Setup migrations

**Week 3: Authentication & Authorization**

- [ ]  Implement ASP.NET Identity
- [ ]  Create JWT authentication
- [ ]  Configure role-based policies
- [ ]  Implement permission system
- [ ]  Create login/logout endpoints

**Week 4: Basic API Structure**

- [ ]  Setup Clean Architecture layers
- [ ]  Configure AutoMapper
- [ ]  Setup FluentValidation
- [ ]  Configure Swagger/OpenAPI
- [ ]  Create base repository pattern
- [ ]  Setup logging (Serilog)

**Deliverables:**

- Working development environment
- Database with seed data
- Authentication system
- Basic API skeleton

---

### Phase 2: Core Inventory (Weeks 5-8)

**Goal: Inventory Management Foundation**

**Week 5: Item & Warehouse Management**

- [ ]  Items CRUD API endpoints
- [ ]  Warehouses CRUD API endpoints
- [ ]  Inventory records creation
- [ ]  Stock level calculations
- [ ]  Frontend: Items list/create/edit
- [ ]  Frontend: Warehouses list/create/edit

**Week 6: Commander's Reserve Implementation ⭐**

- [ ]  Reserve allocation logic
- [ ]  Reserve tracking in inventory records
- [ ]  Reserve thresholds and alerts
- [ ]  Frontend: Reserve visualization in grids
- [ ]  Frontend: Reserve status dashboard

**Week 7: Inventory Transactions**

- [ ]  Receipt transaction (from suppliers)
- [ ]  Issue transaction (to projects/departments)
- [ ]  Adjustment transaction
- [ ]  Transaction validation logic
- [ ]  Inventory update triggers

**Week 8: Inventory UI**

- [ ]  ag-Grid integration
- [ ]  Excel-like inventory grid
- [ ]  Quick filters and search
- [ ]  Stock level indicators
- [ ]  Low stock alerts
- [ ]  Reserve status indicators

**Deliverables:**

- Complete inventory management
- Commander's Reserve tracking
- Transaction system
- Excel-like UI

---

### Phase 3: Requisitions & Approvals (Weeks 9-12)

**Goal: Workflow Automation**

**Week 9: Requisition System**

- [ ]  Create requisition API
- [ ]  Requisition item allocation
- [ ]  Auto-routing logic
- [ ]  Normal vs Reserve detection
- [ ]  Frontend: Create requisition form
- [ ]  Frontend: Requisition list view

**Week 10: Approval Workflows**

- [ ]  Warehouse keeper approval
- [ ]  Commander approval (for reserve) ⭐
- [ ]  Multi-level approval chain
- [ ]  Approval notifications
- [ ]  Frontend: Approval screens
- [ ]  Frontend: Commander reserve approval screen

**Week 11: Fulfillment Process**

- [ ]  Issue materials to requisition
- [ ]  Partial fulfillment support
- [ ]  Inventory deduction
- [ ]  Transaction creation
- [ ]  Frontend: Fulfillment interface

**Week 12: Requisition UI Polish**

- [ ]  Requisition status tracking
- [ ]  History and audit trail
- [ ]  Search and filtering
- [ ]  Bulk actions
- [ ]  Mobile-responsive views

**Deliverables:**

- Complete requisition workflow
- Approval system with commander override
- Fulfillment process
- User-friendly interfaces

---

### Phase 4: Transfers & Projects (Weeks 13-16)

**Goal: Material Movement & Project Tracking**

**Week 13: Transfer System**

- [ ]  Transfer request API
- [ ]  Central ↔ Factory transfers
- [ ]  Transfer approval workflow
- [ ]  Dual inventory updates
- [ ]  Frontend: Transfer request form
- [ ]  Frontend: Transfer management

**Week 14: Project Management**

- [ ]  Projects CRUD API
- [ ]  Project-item allocation
- [ ]  Budget tracking
- [ ]  BOM (Bill of Materials) support
- [ ]  Frontend: Project creation
- [ ]  Frontend: Project dashboard

**Week 15: Material Consumption**

- [ ]  Consumption transaction
- [ ]  Project allocation updates
- [ ]  Cost tracking
- [ ]  Remaining materials handling
- [ ]  Frontend: Consumption recording

**Week 16: Departments**

- [ ]  Departments CRUD
- [ ]  Department requisitions
- [ ]  Department-level reporting
- [ ]  Frontend: Department management

**Deliverables:**

- Transfer system
- Project management
- Material consumption tracking
- Department integration

---

### Phase 5: Reporting & Analytics (Weeks 17-20)

**Goal: Insights & Decision Support**

**Week 17: Core Reports**

- [ ]  Inventory valuation report
- [ ]  Movement summary report
- [ ]  Stock status report
- [ ]  Report export (PDF, Excel)

**Week 18: Commander's Reserve Reports ⭐**

- [ ]  Reserve usage report
- [ ]  Reserve trend analysis
- [ ]  Commander approval history
- [ ]  Reserve depletion forecasts

**Week 19: Project & Cost Reports**

- [ ]  Project cost analysis
- [ ]  Budget vs actual reports
- [ ]  Project profitability
- [ ]  Material waste analysis

**Week 20: Dashboards & Analytics**

- [ ]  Executive dashboard
- [ ]  Factory commander dashboard
- [ ]  Warehouse keeper dashboard
- [ ]  Real-time charts and KPIs

**Deliverables:**

- Comprehensive reporting system
- Executive dashboards
- Export capabilities
- Scheduled reports

---

### Phase 6: Advanced Features (Weeks 21-24)

**Goal: Production-Ready Features**

**Week 21: Supplier Management**

- [ ]  Suppliers CRUD
- [ ]  Purchase orders
- [ ]  Supplier performance tracking
- [ ]  PO to receipt linkage

**Week 22: User Management**

- [ ]  User CRUD with roles
- [ ]  Permission customization
- [ ]  User activity logging
- [ ]  Password management

**Week 23: Audit & Compliance**

- [ ]  Complete audit trail
- [ ]  Compliance reports
- [ ]  Data export for auditors
- [ ]  Change history tracking

**Week 24: System Configuration**

- [ ]  System settings
- [ ]  Email notifications
- [ ]  Backup/restore
- [ ]  Data archiving

**Deliverables:**

- Supplier integration
- User administration
- Audit capabilities
- System configuration

---

### Phase 7: Testing & Deployment (Weeks 25-28)

**Goal: Production Deployment**

**Week 25: Testing**

- [ ]  Unit tests (80% coverage)
- [ ]  Integration tests
- [ ]  E2E tests (critical paths)
- [ ]  Load testing
- [ ]  Security testing

**Week 26: Performance Optimization**

- [ ]  Database query optimization
- [ ]  API response time improvements
- [ ]  Frontend bundle optimization
- [ ]  Caching strategy
- [ ]  CDN setup

**Week 27: Deployment Preparation**

- [ ]  Production environment setup
- [ ]  CI/CD pipeline
- [ ]  Database migration scripts
- [ ]  Monitoring and logging
- [ ]  Backup strategy

**Week 28: Go-Live**

- [ ]  Data migration from legacy system
- [ ]  User training
- [ ]  Documentation
- [ ]  Production deployment
- [ ]  Post-launch support

**Deliverables:**

- Tested, optimized system
- Production deployment
- User documentation
- Training materials

---

## 11. Success Criteria

### 11.1 Functional Requirements ✅

- [ ]  All user roles can access their authorized data
- [ ]  Commander's Reserve requires proper authorization
- [ ]  Inventory quantities are accurate in real-time
- [ ]  Requisition workflows route correctly
- [ ]  Transfers update both warehouses correctly
- [ ]  Projects track costs accurately
- [ ]  Reports generate correct data
- [ ]  Audit trail captures all changes

### 11.2 Performance Metrics 📊

- [ ]  API response time < 200ms (95th percentile)
- [ ]  Page load time < 2 seconds
- [ ]  Support 500 concurrent users
- [ ]  Database queries optimized (< 100ms)
- [ ]  No data loss or corruption
- [ ]  99.9% uptime

### 11.3 Security Requirements 🔒

- [ ]  No unauthorized data access
- [ ]  Commander's Reserve properly protected
- [ ]  All transactions logged
- [ ]  Password policies enforced
- [ ]  SQL injection prevented
- [ ]  XSS/CSRF protection active

### 11.4 User Satisfaction 😊

- [ ]  Intuitive UI (< 2 hours training needed)
- [ ]  Excel-like familiarity
- [ ]  Fast data entry
- [ ]  Clear error messages
- [ ]  Responsive on tablets
- [ ]  Arabic language support

---

## 12. Appendices

### Appendix A: Glossary

| Term (English) | Term (Arabic) | Definition |
| --- | --- | --- |
| Complex | المجمع | The entire military industrial facility |
| Factory | المصنع | Individual production facility within complex |
| Warehouse | المخزن | Storage facility for materials |
| Commander's Reserve | احتياطي القائد | Emergency stock requiring commander approval |
| Requisition | طلب | Request for materials |
| Transfer | تحويل | Movement of materials between warehouses |
| Issue | صرف | Release of materials from warehouse |
| Receipt | استلام | Receiving materials into warehouse |
| Allocation | تخصيص | Reserving materials for a project |
| Consumption | استهلاك | Actual use of materials in production |

### Appendix B: Sample Data

**Sample Factory:**

json

`{
  "code": "FAC-ARM-01",
  "nameEn": "Armoring Factory",
  "nameAr": "مصنع التدريع",
  "commanderName": "Colonel Ahmed Hassan",
  "commanderRank": "COLONEL",
  "specializationType": "ARMORING"
}`

**Sample Item:**

json

`{
  "code": "MTL-ST-001",
  "nameEn": "Armored Steel Plate",
  "nameAr": "صاج حديدي مدرع",
  "category": "RAW_MATERIAL",
  "unitOfMeasure": "KG",
  "minimumStockLevel": 500,
  "reorderPoint": 1000,
  "defaultReservePercentage": 20
}`

**Sample Inventory Record:**

json

`{
  "warehouseId": 1,
  "itemId": 1,
  "totalQuantity": 5000,
  "generalQuantity": 4000,
  "commanderReserveQuantity": 1000,
  "generalAllocated": 500,
  "reserveAllocated": 0,
  "generalAvailable": 3500,
  "reserveAvailable": 1000,
  "averageCost": 245.50,
  "totalValue": 1227500.00,
  "status": "OK"
}`


Updates 
1.1 Project Overview
This system is a sovereign, mission-critical inventory command platform designed exclusively for the Egyptian Armed Forces Engineering Industries Complex. It is a closed-loop system intended to secure military supply chains, not a commercial ERP.

1.2 Problem Statement
The complex currently faces:

❌ Data Inconsistencies: Disconnect between physical stock and digital records.

❌ Language Barriers: Legacy systems lack proper Arabic support, leading to data entry errors by non-English speaking staff.

❌ Security Risks: Lack of granular control over strategic reserves.

❌ Traceability Gaps: Inability to audit the "Chain of Custody" for sensitive materials.

1.3 Goals & Objectives
✅ Native Arabic First: The system must be built with Arabic as the primary language (RTL) for all interfaces and reports, with English as a secondary option.

✅ Sovereignty & Security: On-premise deployment with air-gapped capability. No external cloud dependencies.

✅ Clean Architecture: strict adherence to separation of concerns to ensure maintainability and testability for decades.

✅ Commander's Reserve: Strategic stockpile management with distinct authorization flows.

2. System Overview & Language Strategy
2.1 Globalization & Localization (Arabic Support)
Unlike standard ERPs, this system treats Arabic as the Default Culture.

UI/UX: Native Right-to-Left (RTL) layout. Not just mirrored, but optimized for Arabic readability (fonts, spacing).

Database: PostgreSQL configured with Arabic Collation (ar_EG.utf8) for correct sorting and searching (handling Alef variations: أ, إ, ا).

Input: Smart input masking ensuring numbers are handled correctly (Western vs. Eastern Arabic numerals configurable).

Reports: All official military reports generated in formal Arabic.

2.2 Organizational Structure (Unchanged from v1.0)
See Section 2.1 in original text.

3. Critical Business Concept: Commander's Reserve
See Section 3 in original text. No changes required to business logic, but strict adherence to Clean Architecture in implementation is required.

4. User Roles (Access Control)
See Section 4 in original text.

5. Data Model
See Section 5 in original text. Note: All nameAr fields are mandatory and indexed.

6. Business Workflows & Design Patterns
To adhere to Clean Architecture and Best Practices, the system will utilize specific design patterns for workflows.

6.1 Patterns Applied
CQRS (Command Query Responsibility Segregation): Separating read operations (Reporting/UI) from write operations (Transactions) for performance and security.

Mediator Pattern (MediatR): To decouple feature slices and handle in-process messaging between commands and handlers.

Specification Pattern: For complex inventory filtering and validation rules (e.g., "Is item hazardous AND low stock AND in reserve?").

Chain of Responsibility: For the Approval Workflow (Warehouse Keeper -> Officer -> Commander).

6.2 Material Receipt Workflow (Arabic Context)
Input: Supplier Delivery Note (often hand-written in Arabic).

System Action: OCR Scanning (Future Phase) or Manual Entry via Arabic UI.

Validation: System checks against PO using Specification Pattern rules.

7. Technical Architecture (Strict Clean Architecture)
7.1 Technology Stack
Backend (Core):

Framework: ASP.NET Core 8 Web API

Language: C# 12

Architecture: Clean Architecture + DDD (Domain-Driven Design) + CQRS

Database: PostgreSQL 16 (Collation: ar_EG.utf8)

Messaging: MediatR (In-process), MassTransit (Integration events if needed)

Validation: FluentValidation

Object Mapping: Mapster (High performance)

Frontend (Client):

Framework: Next.js 14 (App Router)

Language: TypeScript 5

Localization: next-intl (Robust i18n routing)

State Management: TanStack Query (Server state) + Zustand (Client state)

UI Library: Shadcn/ui + Radix UI (Native RTL support)

Styling: Tailwind CSS (using rtl: modifiers)

Infrastructure (On-Premise):

Containerization: Docker & Kubernetes (K8s) for internal cluster

CI/CD: Jenkins or GitLab CI (Self-hosted)

Identity: Keycloak (Self-hosted SSO) or ASP.NET Identity

7.2 Solution Structure (The "Clean" Layout)
Plaintext
src/
├── 1. Domain/                      # Core Business Logic (No dependencies)
│   ├── Entities/                   # Enterprise Business Rules (e.g., InventoryRecord)
│   ├── ValueObjects/               # (e.g., Money, Quantity, ArabicName)
│   ├── Enums/                      # (e.g., Rank, WarehouseType)
│   ├── Events/                     # Domain Events (e.g., ReserveStockDepleted)
│   ├── Exceptions/                 # Domain Exceptions
│   └── Interfaces/                 # Repository Contracts
│
├── 2. Application/                 # Use Cases (Orchestration)
│   ├── Common/                     # Behaviors (Logging, Validation, Performance)
│   ├── Features/                   # Feature Slices (CQRS)
│   │   ├── Inventory/
│   │   │   ├── Commands/           # (e.g., AdjustStockCommand)
│   │   │   ├── Queries/            # (e.g., GetStockLevelQuery)
│   │   │   └── Validators/
│   │   ├── Requisitions/
│   │   └── Reports/
│   └── Interfaces/                 # Application Interfaces (ICurrentUserService)
│
├── 3. Infrastructure/              # External Concerns (Impl. of Interfaces)
│   ├── Persistence/                # EF Core DbContext, Migrations
│   │   ├── Configurations/         # Entity Configurations
│   │   └── Repositories/           # Repository Implementations
│   ├── Identity/                   # Auth logic
│   ├── Files/                      # File storage (PDF generation)
│   └── Services/                   # DateTime, Email services
│
└── 4. WebAPI/                      # Entry Point
    ├── Controllers/                # Minimal Controllers (calling MediatR)
    ├── Middleware/                 # Global Error Handling, RTL Content-Type
    └── Program.cs                  # DI Composition Root
7.3 Domain-Driven Design (DDD) Implementation
Aggregate Root Example: InventoryItem

C#
// Domain/Entities/InventoryItem.cs
public class InventoryItem : AggregateRoot
{
    public Quantity GeneralStock { get; private set; }
    public Quantity CommanderReserve { get; private set; } // Value Object

    // Business Logic is ENCAPSULATED here, not in services
    public void ConsumeStock(decimal amount, bool fromReserve, UserRank requestorRank)
    {
        if (fromReserve)
        {
            if (!CanAccessReserve(requestorRank))
                throw new DomainException("Unauthorized access to Commander Reserve");
                
            if (CommanderReserve.Value < amount)
                throw new DomainException("Insufficient Reserve Stock");

            CommanderReserve = CommanderReserve.Subtract(amount);
            AddDomainEvent(new CommanderReserveAccessedEvent(this.Id, amount));
        }
        else 
        {
            // General logic...
        }
    }
}
8. UI/UX Design Requirements (Arabic First)
8.1 RTL Design System
Directionality: The <html> tag will default to dir="rtl".

Typography: Primary Font: Cairo or IBM Plex Sans Arabic (optimized for technical/dashboard use). Monospace Font (for codes): JetBrains Mono.

Iconography: Directional icons (arrows, chevrons, progress bars) must mirror automatically based on locale using lucide-react RTL transforms.

8.2 Design Specs
Theme: "Military Dark Operations" (Low eye strain for 24/7 Ops Rooms).

Colors: Dark Slate Grays, Muted Golds (Reserve), Tactical Greens (Status).

Dashboard: High-contrast data grids for quick scanning in low light.

9. Resource & Infrastructure Requirements (Internal Costing)
Since we are not selling this, we focus on TCO (Total Cost of Ownership) for the military budget.

9.1 Hardware Requirements (On-Premise)
Database Server: High-memory tier (min 64GB RAM) for PostgreSQL caching. SSD NVMe storage for rapid query execution.

Application Server: Scalable cluster (Kubernetes) to handle concurrent connections from multiple factories.

Backup Server: Physically separate location (Disaster Recovery) with Write-Once-Read-Many (WORM) storage for audit logs.

9.2 Development Team Resources
1x Solution Architect: Responsible for Clean Architecture integrity.

2x Backend Developers: C# / PostgreSQL specialists.

2x Frontend Developers: React / Next.js / RTL specialists.

1x QA Engineer: Automated testing and Arabic linguistic validation.

10. Implementation Roadmap
Phase 1: Foundation (Weeks 1-4)
Setup Clean Architecture solution with Strict Mode.

Configure Arabic Localization Infrastructure (i18n, RTL Layouts).

Implement Identity Server with Military ID login.

Phase 2: Core Domain (Weeks 5-8)
Implement Inventory Aggregates and Value Objects.

Build "Commander's Reserve" Logic using Strategy Pattern.

Develop Data Grids with Arabic filtering/sorting.

Phase 3: Workflows (Weeks 9-12)
Implement Requisition CQRS commands.

Build Approval Chain of Responsibility.

Generate PDF Reports (using QuestPDF for C#) with Arabic font embedding.

Phase 4: Hardening & Deployment (Weeks 13-16)
Performance Tuning (PostgreSQL Indexes).

Security Audits (Penetration testing).

On-premise Deployment to Military Intranet.

11. Success Criteria
Linguistic Accuracy: 100% of UI and Reports are grammatically correct in Arabic.

Performance: Inventory queries execute in < 200ms regardless of database size.

Security: Penetration testing confirms zero unauthorized access to Commander's Reserve.

Architecture: Codebase passes static analysis for dependency rule violations (Domain depends on nothing).