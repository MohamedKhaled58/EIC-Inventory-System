# Technical Specification
## Enginerring Industrial Complex Inventory Command System

**Document Version:** 1.0  
**Spec Date:** January 30, 2025  
**Author:** Architect Mode

---

## Table of Contents

1. [System Requirements](#1-system-requirements)
2. [Backend Specifications](#2-backend-specifications)
3. [Frontend Specifications](#3-frontend-specifications)
4. [Database Specifications](#4-database-specifications)
5. [API Specifications](#5-api-specifications)
6. [Security Specifications](#6-security-specifications)
7. [Performance Specifications](#7-performance-specifications)
8. [Testing Specifications](#8-testing-specifications)

---

## 1. System Requirements

### 1.1 Functional Requirements

| ID | Requirement | Description | Priority |
|----|-------------|-------------|----------|
| **FR-001** | User Authentication | Users must authenticate with military ID and password | Critical |
| **FR-002** | Role-Based Access | System must enforce role-based permissions | Critical |
| **FR-003** | Inventory Tracking | Track all inventory items with quantities | Critical |
| **FR-004** | Commander's Reserve | Separate tracking for emergency stock | Critical |
| **FR-005** | Requisition Workflow | Create, approve, and fulfill requisitions | Critical |
| **FR-006** | Reserve Authorization | Commander approval required for reserve access | Critical |
| **FR-007** | Transfer System | Transfer materials between warehouses | High |
| **FR-008** | Project Management | Create and manage projects | High |
| **FR-009** | Material Allocation | Allocate materials to projects | High |
| **FR-010** | Consumption Tracking | Track material consumption | High |
| **FR-011** | Reporting | Generate standard reports | High |
| **FR-012** | Arabic Support | Full Arabic language support with RTL | Critical |
| **FR-013** | Audit Trail | Complete audit trail for all actions | Critical |
| **FR-014** | Offline Operations | Support offline warehouse operations | Medium |
| **FR-015** | Notifications | Real-time notifications for approvals | Medium |

---

### 1.2 Non-Functional Requirements

| ID | Requirement | Specification | Priority |
|----|-------------|----------------|----------|
| **NFR-001** | Response Time | API < 200ms (95th percentile) | Critical |
| **NFR-002** | Page Load | < 2 seconds | Critical |
| **NFR-003** | Concurrency | Support 500 concurrent users | Critical |
| **NFR-004** | Availability | 99.9% uptime | Critical |
| **NFR-005** | Security | HTTPS only, TLS 1.3 | Critical |
| **NFR-006** | Data Integrity | Zero data loss | Critical |
| **NFR-007** | Scalability | Handle 10M+ transactions | High |
| **NFR-008** | Arabic Performance | Arabic queries < 100ms | High |
| **NFR-009** | Mobile Support | Responsive on tablets | High |
| **NFR-010** | Offline Support | 24-hour offline operation | Medium |

---

## 2. Backend Specifications

### 2.1 Technology Stack

| Component | Technology | Version | Rationale |
|-----------|-------------|----------|-----------|
| **Framework** | ASP.NET Core | 8.0 | Latest LTS, high performance |
| **Language** | C# | 12 | Latest features, performance |
| **Runtime** | .NET | 8.0 | Cross-platform support |
| **Database** | PostgreSQL | 16.0 | Open source, Arabic collation |
| **ORM** | Entity Framework Core | 8.0 | Official .NET ORM |
| **Authentication** | ASP.NET Identity | 8.0 | Built-in identity management |
| **JWT** | System.IdentityModel.Tokens.Jwt | 7.0 | Industry standard |
| **Mediation** | MediatR | 12.0 | CQRS mediator pattern |
| **Validation** | FluentValidation | 11.9 | Fluent validation API |
| **Mapping** | Mapster | 7.4 | High performance mapping |
| **Logging** | Serilog | 3.1 | Structured logging |
| **Caching** | StackExchange.Redis | 2.7 | Distributed caching |
| **Testing** | xUnit, Moq, FluentAssertions | Latest | Comprehensive testing |
| **API Docs** | Swashbuckle.AspNetCore | 6.5 | OpenAPI/Swagger |

---

### 2.2 Project Structure

```
EIC.Inventory.System.sln
├── src/
│   ├── EIC.Inventory.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Enums/
│   │   ├── Events/
│   │   ├── Exceptions/
│   │   └── Interfaces/
│   ├── EIC.Inventory.Application/
│   │   ├── Common/
│   │   ├── Features/
│   │   └── Interfaces/
│   ├── EIC.Inventory.Infrastructure/
│   │   ├── Persistence/
│   │   ├── Identity/
│   │   ├── Cache/
│   │   ├── Services/
│   │   └── Files/
│   └── EIC.Inventory.WebAPI/
│       ├── Controllers/
│       ├── Middleware/
│       └── Filters/
└── tests/
    ├── EIC.Inventory.Domain.Tests/
    ├── EIC.Inventory.Application.Tests/
    ├── EIC.Inventory.Infrastructure.Tests/
    └── EIC.Inventory.WebAPI.Tests/
```

---

### 2.3 Domain Layer Specifications

#### **2.3.1 Entities**

All entities must:
- Inherit from `AggregateRoot` or implement `IEntity`
- Have private setters with public methods for business logic
- Include audit fields (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`)
- Implement `IAuditable` interface
- Use value objects for complex attributes

**Example:**
```csharp
public class InventoryItem : AggregateRoot, IAuditable
{
    // Private setters for encapsulation
    public Guid Id { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid ItemId { get; private set; }
    
    // Value objects
    public Quantity GeneralStock { get; private set; }
    public Quantity CommanderReserve { get; private set; }
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Business logic methods
    public void ReceiveStock(decimal quantity, decimal reservePercentage)
    {
        var reservePortion = quantity * (reservePercentage / 100);
        var generalPortion = quantity - reservePortion;
        
        GeneralStock = GeneralStock.Add(generalPortion);
        CommanderReserve = CommanderReserve.Add(reservePortion);
        
        AddDomainEvent(new StockReceivedEvent(Id, quantity));
    }
}
```

---

#### **2.3.2 Value Objects**

Value objects must:
- Be immutable (readonly properties)
- Override `Equals` and `GetHashCode`
- Implement value-based equality
- Have private constructors with factory methods
- Validate invariants in constructor

**Example:**
```csharp
public readonly record Quantity
{
    public decimal Value { get; }
    public UnitOfMeasure Unit { get; }
    
    private Quantity(decimal value, UnitOfMeasure unit)
    {
        if (value < 0)
            throw new DomainException("Quantity cannot be negative");
        
        Value = value;
        Unit = unit;
    }
    
    public static Quantity Create(decimal value, UnitOfMeasure unit) => 
        new Quantity(value, unit);
    
    public Quantity Add(Quantity other) => 
        new Quantity(Value + other.Value, Unit);
    
    public Quantity Subtract(Quantity other) => 
        new Quantity(Value - other.Value, Unit);
    
    public bool IsGreaterThan(Quantity other) => 
        Value > other.Value;
    
    public bool IsLessThan(Quantity other) => 
        Value < other.Value;
}
```

---

#### **2.3.3 Domain Events**

Domain events must:
- Implement `IDomainEvent` interface
- Be immutable (record types)
- Include `OccurredOn` timestamp
- Contain all relevant data
- Have descriptive names

**Example:**
```csharp
public record CommanderReserveAccessedEvent(
    Guid InventoryItemId,
    decimal Quantity,
    Guid RequestedBy,
    UserRank RequestorRank,
    DateTime AccessedAt,
    string Justification
) : IDomainEvent
{
    public DateTime OccurredOn => AccessedAt;
}
```

---

### 2.4 Application Layer Specifications

#### **2.4.1 Commands**

Commands must:
- Be immutable (record types)
- Implement `IRequest<TResponse>` or `IRequest`
- Include validation attributes
- Have descriptive names ending with "Command"
- Include all necessary data

**Example:**
```csharp
public record CreateRequisitionCommand(
    Guid? ProjectId,
    Guid? DepartmentId,
    Guid SourceWarehouseId,
    PriorityLevel Priority,
    DateTime RequiredDate,
    [StringLength(1000, MinimumLength = 10)]
    string Purpose,
    List<RequisitionItemDto> Items
) : IRequest<Result<Guid>>;
```

---

#### **2.4.2 Queries**

Queries must:
- Be immutable (record types)
- Implement `IRequest<TResponse>`
- Include only read parameters
- Have descriptive names ending with "Query"
- Support pagination

**Example:**
```csharp
public record GetInventoryByWarehouseQuery(
    Guid WarehouseId,
    string? SearchTerm,
    ItemCategory? Category,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<InventoryItemDto>>;
```

---

#### **2.4.3 Command Handlers**

Command handlers must:
- Implement `IRequestHandler<TRequest, TResponse>`
- Use repository interfaces (not concrete implementations)
- Execute business logic through domain entities
- Return `Result<T>` pattern
- Dispatch domain events
- Use transactions for write operations

**Example:**
```csharp
public class CreateRequisitionCommandHandler 
    : IRequestHandler<CreateRequisitionCommand, Result<Guid>>
{
    private readonly IRequisitionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDomainEventDispatcher _eventDispatcher;
    
    public async Task<Result<Guid>> Handle(
        CreateRequisitionCommand request,
        CancellationToken ct)
    {
        // 1. Validate user permissions
        if (!_currentUser.CanCreateRequisition())
            return Result.Failure<Guid>("Unauthorized");
        
        // 2. Check if any item requests commander's reserve
        var hasReserveRequest = request.Items.Any(i => i.UseCommanderReserve);
        
        // 3. Create requisition aggregate
        var requisition = new Requisition(
            requestedBy: _currentUser.UserId,
            sourceWarehouseId: request.SourceWarehouseId,
            priority: request.Priority,
            requiredDate: request.RequiredDate,
            purpose: request.Purpose,
            requiresCommanderApproval: hasReserveRequest
        );
        
        // 4. Add items
        foreach (var itemDto in request.Items)
        {
            requisition.AddItem(new RequisitionItem(
                itemId: itemDto.ItemId,
                requestedQuantity: itemDto.RequestedQuantity,
                requestFromReserve: itemDto.UseCommanderReserve,
                itemPurpose: itemDto.ItemPurpose
            ));
        }
        
        // 5. Persist
        await _repository.AddAsync(requisition, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        // 6. Dispatch domain events
        await _eventDispatcher.DispatchAsync(requisition.DomainEvents);
        
        return Result.Success(requisition.Id);
    }
}
```

---

#### **2.4.4 Validators**

Validators must:
- Inherit from `AbstractValidator<T>`
- Use FluentValidation rules
- Include Arabic validation messages
- Validate business rules
- Be registered in DI container

**Example:**
```csharp
public class CreateRequisitionValidator : AbstractValidator<CreateRequisitionCommand>
{
    public CreateRequisitionValidator()
    {
        RuleFor(x => x.SourceWarehouseId)
            .NotEmpty()
            .WithMessage("المخزن المطلوب مطلوب"); // Source warehouse is required
        
        RuleFor(x => x.RequiredDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("التاريخ المطلوب يجب أن يكون اليوم أو في المستقبل"); // Required date must be today or in the future
        
        RuleFor(x => x.Purpose)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(1000)
            .WithMessage("الغرض مطلوب ويجب أن يكون بين 10 و 1000 حرف"); // Purpose is required and must be between 10 and 1000 characters
        
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("يجب إضافة صنف واحد على الأقل"); // At least one item must be added
        
        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(i => i.ItemId)
                    .NotEmpty()
                    .WithMessage("الصنف مطلوب"); // Item is required
                
                item.RuleFor(i => i.RequestedQuantity)
                    .GreaterThan(0)
                    .WithMessage("الكمية المطلوبة يجب أن تكون أكبر من صفر"); // Requested quantity must be greater than zero
                
                item.When(i => i.UseCommanderReserve, () =>
                {
                    item.RuleFor(i => i.ItemPurpose)
                        .NotEmpty()
                        .WithMessage("الغرض مطلوب عند طلب احتياطي القائد"); // Purpose is required when requesting commander's reserve
                });
            });
    }
}
```

---

### 2.5 Infrastructure Layer Specifications

#### **2.5.1 Entity Framework Configurations**

Configurations must:
- Inherit from `IEntityTypeConfiguration<T>`
- Configure all entity properties
- Set Arabic collation for text fields
- Define precision for decimal fields
- Create indexes for performance
- Configure relationships
- Set up constraints

**Example:**
```csharp
public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryRecords");
        
        // Primary key
        builder.HasKey(x => x.Id);
        
        // Arabic collation
        builder.Property(x => x.Item.NameAr)
            .IsRequired()
            .HasMaxLength(200)
            .UseCollation("ar_EG.utf8");
        
        // Decimal precision
        builder.Property(x => x.TotalQuantity)
            .HasPrecision(18, 3)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(x => new { x.WarehouseId, x.ItemId })
            .IsUnique()
            .HasDatabaseName("UQ_Inventory_Warehouse_Item");
        
        // Relationships
        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Concurrency
        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}
```

---

#### **2.5.2 Repository Implementations**

Repositories must:
- Implement repository interface from Domain layer
- Use `ApplicationDbContext` for data access
- Include related entities when needed
- Use async methods
- Handle exceptions appropriately
- Support specifications

**Example:**
```csharp
public class InventoryItemRepository : Repository<InventoryItem>, IInventoryItemRepository
{
    public InventoryItemRepository(ApplicationDbContext context)
        : base(context)
    {
    }
    
    public async Task<InventoryItem?> GetByWarehouseAndItemAsync(
        Guid warehouseId,
        Guid itemId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Include(x => x.Warehouse)
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => 
                x.WarehouseId == warehouseId && 
                x.ItemId == itemId, ct);
    }
    
    public async Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(
        Guid warehouseId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == warehouseId)
            .Where(x => x.GeneralAvailable <= x.ReorderPoint)
            .ToListAsync(ct);
    }
}
```

---

### 2.6 WebAPI Layer Specifications

#### **2.6.1 Controllers**

Controllers must:
- Inherit from `ApiController`
- Use minimal controllers where possible
- Use `[Authorize]` attribute for authorization
- Use `[Route]` attribute for routing
- Return `IActionResult` or `ActionResult<T>`
- Use `MediatR` for command/query handling
- Include XML documentation comments

**Example:**
```csharp
/// <summary>
/// Inventory management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InventoryController> _logger;
    
    public InventoryController(IMediator mediator, ILogger<InventoryController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Get inventory by warehouse
    /// </summary>
    /// <param name="warehouseId">Warehouse ID</param>
    /// <param name="searchTerm">Search term (optional)</param>
    /// <param name="category">Item category (optional)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <returns>Paginated inventory items</returns>
    [HttpGet("warehouses/{warehouseId}/items")]
    [ProducesResponseType(typeof(PagedResult<InventoryItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResult<InventoryItemDto>>> GetInventoryByWarehouse(
        Guid warehouseId,
        string? searchTerm,
        ItemCategory? category,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var query = new GetInventoryByWarehouseQuery(
            warehouseId,
            searchTerm,
            category,
            pageNumber,
            pageSize
        );
        
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
    
    /// <summary>
    /// Adjust inventory stock
    /// </summary>
    /// <param name="id">Inventory item ID</param>
    /// <param name="command">Adjust stock command</param>
    /// <returns>Result</returns>
    [HttpPost("{id}/adjust")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result>> AdjustStock(
        Guid id,
        [FromBody] AdjustStockCommand command)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command);
        
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok(result);
    }
}
```

---

#### **2.6.2 Middleware**

Middleware must:
- Be registered in `Program.cs`
- Handle cross-cutting concerns
- Not modify request/response content
- Use dependency injection
- Log appropriately

**Example:**
```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");
        
        var response = context.Response;
        response.ContentType = "application/json";
        
        var errorResponse = new ErrorResponse();
        
        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.Errors = validationEx.Errors;
                break;
                
            case DomainException domainEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = domainEx.Message;
                break;
                
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                break;
        }
        
        var result = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(result);
    }
}
```

---

## 3. Frontend Specifications

### 3.1 Technology Stack

| Component | Technology | Version | Rationale |
|-----------|-------------|----------|-----------|
| **Framework** | Next.js | 14.0 | React framework with App Router |
| **Language** | TypeScript | 5.3 | Type-safe JavaScript |
| **UI Library** | Shadcn/ui + Radix UI | Latest | Accessible components, RTL support |
| **Styling** | Tailwind CSS | 3.4 | Utility-first CSS |
| **Data Grid** | ag-Grid Enterprise | 31.0 | Excel-like grid, RTL support |
| **State (Server)** | TanStack Query | 5.0 | Server state management |
| **State (Client)** | Zustand | 4.4 | Client state management |
| **Forms** | React Hook Form | 7.48 | Form management |
| **Validation** | Zod | 3.22 | Schema validation |
| **HTTP Client** | Axios | 1.6 | API requests |
| **i18n** | next-intl | 3.0 | Internationalization |
| **Charts** | Recharts | 2.10 | Data visualization |
| **Icons** | Lucide React | 0.300 | Icon library |
| **Testing** | Jest, React Testing Library | Latest | Unit testing |
| **E2E Testing** | Playwright | 1.40 | End-to-end tests |

---

### 3.2 Project Structure

```
eic-inventory-frontend/
├── src/
│   ├── app/
│   │   ├── (auth)/
│   │   │   ├── login/
│   │   │   └── layout.tsx
│   │   ├── (dashboard)/
│   │   │   ├── layout.tsx
│   │   │   ├── page.tsx
│   │   │   ├── inventory/
│   │   │   ├── requisitions/
│   │   │   ├── projects/
│   │   │   └── reports/
│   │   └── api/
│   ├── components/
│   │   ├── ui/
│   │   ├── forms/
│   │   ├── tables/
│   │   ├── layout/
│   │   └── shared/
│   ├── lib/
│   │   ├── api/
│   │   ├── hooks/
│   │   ├── utils/
│   │   └── validations/
│   ├── store/
│   │   ├── auth-store.ts
│   │   ├── ui-store.ts
│   │   └── inventory-store.ts
│   ├── types/
│   │   ├── api.ts
│   │   ├── entities.ts
│   │   └── enums.ts
│   ├── locales/
│   │   ├── ar.json
│   │   ├── en.json
│   │   └── index.ts
│   └── styles/
├── public/
├── tests/
└── package.json
```

---

### 3.3 Component Specifications

#### **3.3.1 UI Components**

All UI components must:
- Use Shadcn/ui or Radix UI as base
- Support RTL layout
- Have Arabic and English labels
- Include proper ARIA labels
- Support keyboard navigation
- Have consistent styling
- Include loading states
- Handle errors gracefully

**Example:**
```typescript
'use client';

import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useTranslations } from 'next-intl';

interface InventoryItemFormProps {
  onSubmit: (data: InventoryItemFormData) => void;
  isLoading?: boolean;
}

export function InventoryItemForm({ onSubmit, isLoading }: InventoryItemFormProps) {
  const t = useTranslations('inventory');
  const { locale } = useLocale();
  const isRTL = locale === 'ar';

  return (
    <form onSubmit={onSubmit} dir={isRTL ? 'rtl' : 'ltr'} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="code">{t('code')}</Label>
          <Input
            id="code"
            name="code"
            placeholder={t('codePlaceholder')}
            required
          />
        </div>
        
        <div className="space-y-2">
          <Label htmlFor="nameAr">{t('nameAr')}</Label>
          <Input
            id="nameAr"
            name="nameAr"
            placeholder={t('nameArPlaceholder')}
            required
            dir="rtl"
            className="text-right"
          />
        </div>
      </div>
      
      <Button type="submit" disabled={isLoading}>
        {isLoading ? t('saving') : t('save')}
      </Button>
    </form>
  );
}
```

---

#### **3.3.2 Data Grid Components**

Data grids must:
- Use ag-Grid Enterprise
- Support RTL layout
- Have Arabic text support
- Include sorting and filtering
- Support pagination
- Have virtual scrolling
- Support column customization
- Include export functionality
- Handle large datasets (1000+ rows)

**Example:**
```typescript
'use client';

import { AgGridReact } from 'ag-grid-react';
import type { ColDef, GridOptions } from 'ag-grid-community';
import { useInventory } from '@/lib/api/inventory';
import { Badge } from '@/components/ui/badge';
import { useTranslations, useLocale } from 'next-intl';

export function InventoryGrid({ warehouseId }: { warehouseId: string }) {
  const t = useTranslations('inventory');
  const { locale } = useLocale();
  const isRTL = locale === 'ar';
  const { data: inventory, isLoading } = useInventory(warehouseId);

  const columnDefs: ColDef[] = [
    {
      field: 'item.code',
      headerName: t('code'),
      pinned: 'left',
      width: 120,
      cellClass: 'font-mono',
    },
    {
      field: 'item.nameAr',
      headerName: t('nameAr'),
      flex: 1,
      cellClass: 'text-right',
    },
    {
      field: 'item.nameEn',
      headerName: t('nameEn'),
      flex: 1,
    },
    {
      field: 'totalQuantity',
      headerName: t('total'),
      type: 'numericColumn',
      width: 100,
      cellClass: 'font-mono text-right',
      valueFormatter: (params) => 
        params.value?.toLocaleString(locale === 'ar' ? 'ar-EG' : 'en-US') || '',
    },
    {
      field: 'commanderReserveQuantity',
      headerName: `${t('reserve')} ⭐`,
      type: 'numericColumn',
      width: 120,
      cellClass: 'font-mono text-right bg-military-gold',
      valueFormatter: (params) => 
        params.value?.toLocaleString(locale === 'ar' ? 'ar-EG' : 'en-US') || '',
      cellStyle: { fontWeight: 'bold', backgroundColor: '#8b6914' },
    },
    {
      field: 'status',
      headerName: t('status'),
      width: 100,
      cellRenderer: (params) => {
        const status = params.value;
        const config = {
          OK: { icon: '🟢', label: t('statusOK') },
          LOW: { icon: '🟡', label: t('statusLow') },
          CRITICAL: { icon: '🔴', label: t('statusCritical') },
          RESERVE_LOW: { icon: '🔶', label: t('statusReserveLow') },
        }[status];
        
        return (
          <Badge variant={status === 'CRITICAL' ? 'destructive' : 'default'}>
            {config.icon} {config.label}
          </Badge>
        );
      },
    },
  ];

  const gridOptions: GridOptions = {
    theme: 'ag-theme-alpine-dark',
    rowHeight: 35,
    headerHeight: 40,
    enableRangeSelection: true,
    enableCellTextSelection: true,
    suppressRowClickSelection: true,
    enableFillHandle: true,
    undoRedoCellEditing: true,
    animateRows: true,
    pagination: true,
    paginationPageSize: 50,
    paginationPageSizeSelector: [25, 50, 100, 200],
    defaultColDef: {
      resizable: true,
      sortable: true,
      filter: true,
      floatingFilter: true,
    },
    rtl: isRTL,
  };

  if (isLoading) {
    return <div className="flex items-center justify-center h-64">{t('loading')}</div>;
  }

  return (
    <div 
      className="ag-theme-alpine-dark" 
      style={{ height: '600px', width: '100%' }}
      dir={isRTL ? 'rtl' : 'ltr'}
    >
      <AgGridReact
        columnDefs={columnDefs}
        rowData={inventory}
        gridOptions={gridOptions}
      />
    </div>
  );
}
```

---

#### **3.3.3 Form Components**

Forms must:
- Use React Hook Form
- Validate with Zod schemas
- Show real-time validation errors
- Support Arabic input
- Include loading states
- Handle submission errors
- Show success/error toasts
- Support draft saving

**Example:**
```typescript
'use client';

import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useTranslations } from 'next-intl';
import { toast } from '@/components/ui/use-toast';

const requisitionSchema = z.object({
  sourceWarehouseId: z.string().uuid(t('warehouseRequired')),
  priority: z.enum(['LOW', 'NORMAL', 'HIGH', 'EMERGENCY']),
  requiredDate: z.date().min(new Date(), t('dateFuture')),
  purpose: z.string().min(10).max(1000),
  items: z.array(z.object({
    itemId: z.string().uuid(),
    requestedQuantity: z.number().positive(),
    useCommanderReserve: z.boolean(),
    itemPurpose: z.string().optional(),
  })).min(1, t('atLeastOneItem')),
});

type RequisitionFormData = z.infer<typeof requisitionSchema>;

export function CreateRequisitionForm() {
  const t = useTranslations('requisition');
  const { locale } = useLocale();
  const isRTL = locale === 'ar';
  
  const form = useForm<RequisitionFormData>({
    resolver: zodResolver(requisitionSchema),
    defaultValues: {
      priority: 'NORMAL',
      items: [],
    },
  });
  
  const { handleSubmit, formState: { isSubmitting } } = form;
  
  const onSubmit = async (data: RequisitionFormData) => {
    try {
      // Submit to API
      await apiClient.post('/api/requisitions', data);
      
      toast({
        title: t('success'),
        description: t('requisitionCreated'),
      });
      
      // Redirect to requisition list
      router.push('/requisitions');
    } catch (error: any) {
      toast({
        variant: 'destructive',
        title: t('error'),
        description: error.response?.data?.message || t('submissionFailed'),
      });
    }
  };
  
  return (
    <form onSubmit={handleSubmit(onSubmit)} dir={isRTL ? 'rtl' : 'ltr'} className="space-y-6">
      {/* Form fields */}
      
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => router.back()}>
          {t('cancel')}
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? t('submitting') : t('submit')}
        </Button>
      </div>
    </form>
  );
}
```

---

### 3.4 API Client Specifications

API client must:
- Use Axios for HTTP requests
- Include request/response interceptors
- Handle JWT token refresh
- Handle errors gracefully
- Support request cancellation
- Include timeout handling
- Log requests/responses
- Support retry logic

**Example:**
```typescript
import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig } from 'axios';
import { toast } from '@/components/ui/use-toast';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: process.env.NEXT_PUBLIC_API_URL,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
        'Accept-Language': this.getLocale(),
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor
    this.client.interceptors.request.use(
      (config) => {
        const token = localStorage.getItem('access_token');
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor
    this.client.interceptors.response.use(
      (response) => response,
      async (error: AxiosError) => {
        const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

        // Handle 401 - Unauthorized
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;
          
          try {
            const refreshToken = localStorage.getItem('refresh_token');
            const response = await this.client.post('/api/auth/refresh', {
              refreshToken,
            });
            
            const { accessToken, refreshToken: newRefreshToken } = response.data;
            localStorage.setItem('access_token', accessToken);
            localStorage.setItem('refresh_token', newRefreshToken);
            
            originalRequest.headers.Authorization = `Bearer ${accessToken}`;
            return this.client(originalRequest);
          } catch (refreshError) {
            localStorage.removeItem('access_token');
            localStorage.removeItem('refresh_token');
            window.location.href = '/login';
            return Promise.reject(refreshError);
          }
        }

        // Handle other errors
        const message = error.response?.data?.message || 'An error occurred';
        toast({
          variant: 'destructive',
          title: 'Error',
          description: message,
        });

        return Promise.reject(error);
      }
    );
  }

  private getLocale(): string {
    return localStorage.getItem('locale') || 'ar';
  }

  public get<T>(url: string, config?: AxiosRequestConfig) {
    return this.client.get<T>(url, config);
  }

  public post<T>(url: string, data?: any, config?: AxiosRequestConfig) {
    return this.client.post<T>(url, data, config);
  }

  public put<T>(url: string, data?: any, config?: AxiosRequestConfig) {
    return this.client.put<T>(url, data, config);
  }

  public delete<T>(url: string, config?: AxiosRequestConfig) {
    return this.client.delete<T>(url, config);
  }
}

export const apiClient = new ApiClient();
```

---

## 4. Database Specifications

### 4.1 PostgreSQL Configuration

**Database Settings:**
```sql
-- PostgreSQL configuration
-- postgresql.conf

-- Arabic collation
lc_collate = 'ar_EG.utf8'
lc_ctype = 'ar_EG.utf8'

-- Performance
shared_buffers = 256MB
effective_cache_size = 1GB
maintenance_work_mem = 64MB
checkpoint_completion_target = 0.9
wal_buffers = 16MB
default_statistics_target = 100
random_page_cost = 1.1
effective_io_concurrency = 200

-- Connection pooling
max_connections = 200
superuser_reserved_connections = 3

-- Logging
logging_collector = 'csvlog'
log_destination = 'csvlog'
log_directory = 'pg_log'
log_filename = 'postgresql-%Y-%m-%d_%H%M%S.log'
log_min_duration_statement = 1000
log_checkpoints = on
log_connections = on
log_disconnections = on
log_duration = on
log_line_prefix = '%t [%p]: '
log_lock_waits = on
log_statement = 'mod'
log_temp_files = 0
```

---

### 4.2 Database Schema

See [`03-Detailed-Technical-Architecture.md`](03-Detailed-Technical-Architecture.md#4-database-architecture) for complete schema.

---

### 4.3 Indexes

**Performance Indexes:**
```sql
-- Inventory indexes
CREATE INDEX IX_Inventory_Warehouse_Item 
ON InventoryRecords(WarehouseId, ItemId);

CREATE INDEX IX_Inventory_Status 
ON InventoryRecords(WarehouseId, Status);

CREATE INDEX IX_Inventory_LowStock 
ON InventoryRecords(WarehouseId) 
WHERE GeneralAvailable <= ReorderPoint;

CREATE INDEX IX_Inventory_ReserveLow 
ON InventoryRecords(WarehouseId) 
WHERE ReserveAvailable <= MinimumReserveRequired;

-- Transaction indexes
CREATE INDEX IX_Transaction_Date 
ON InventoryTransactions(TransactionDate DESC);

CREATE INDEX IX_Transaction_Warehouse_Item_Date 
ON InventoryTransactions(WarehouseId, ItemId, TransactionDate);

-- Requisition indexes
CREATE INDEX IX_Requisition_Status_Date 
ON Requisitions(Status, RequestedDate DESC);

CREATE INDEX IX_Requisition_RequestedBy 
ON Requisitions(RequestedById);

CREATE INDEX IX_Requisition_ProjectId 
ON Requisitions(ProjectId);

-- Full-text search for Arabic
CREATE INDEX IX_Items_NameAr_FTS 
ON Items 
USING gin(to_tsvector('arabic', NameAr));

CREATE INDEX IX_Items_NameEn_FTS 
ON Items 
USING gin(to_tsvector('english', NameEn));
```

---

## 5. API Specifications

### 5.1 API Standards

**HTTP Methods:**
- `GET` - Retrieve resources
- `POST` - Create resources
- `PUT` - Update resources
- `DELETE` - Delete resources
- `PATCH` - Partial updates (rare)

**Status Codes:**
- `200 OK` - Request succeeded
- `201 Created` - Resource created
- `204 No Content` - Request succeeded, no content returned
- `400 Bad Request` - Invalid request
- `401 Unauthorized` - Authentication required/failed
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Resource conflict (concurrency)
- `422 Unprocessable Entity` - Validation failed
- `500 Internal Server Error` - Server error

**Request Headers:**
```
Content-Type: application/json
Accept: application/json
Authorization: Bearer <token>
Accept-Language: ar|en
```

**Response Headers:**
```
Content-Type: application/json; charset=utf-8
Content-Language: ar|en
X-Request-ID: <guid>
X-Response-Time: <ms>
```

---

### 5.2 API Endpoints

See [`03-Detailed-Technical-Architecture.md`](03-Detailed-Technical-Architecture.md#51-api-endpoint-structure) for complete endpoint list.

---

## 6. Security Specifications

### 6.1 Authentication

**JWT Token Configuration:**
```csharp
{
  "Jwt": {
    "Key": "<256-bit-secret-key>",
    "Issuer": "https://inventory.eic.internal",
    "Audience": "https://inventory.eic.internal",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  }
}
```

**Token Claims:**
```json
{
  "sub": "<user-id>",
  "name": "<username>",
  "email": "<email>",
  "role": "<user-role>",
  "rank": "<military-rank>",
  "factoryId": "<factory-id>",
  "departmentId": "<department-id>",
  "iat": "<issued-at>",
  "exp": "<expiration>",
  "jti": "<token-id>"
}
```

---

### 6.2 Authorization

**Role-Based Policies:**
```csharp
services.AddAuthorization(options =>
{
    // Commander's Reserve Policy
    options.AddPolicy("AccessCommanderReserve", policy =>
        policy.RequireAssertion(context =>
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            return role == UserRole.FACTORY_COMMANDER.ToString() ||
                   role == UserRole.COMPLEX_COMMANDER.ToString();
        }));
    
    // Factory Access Policy
    options.AddPolicy("FactoryAccess", policy =>
        policy.Requirements.Add(new FactoryAccessRequirement()));
    
    // Warehouse Keeper Policy
    options.AddPolicy("WarehouseKeeperAccess", policy =>
        policy.RequireRole(UserRole.FACTORY_WAREHOUSE_KEEPER.ToString()));
    
    // Department Head Policy
    options.AddPolicy("DepartmentHeadAccess", policy =>
        policy.RequireRole(UserRole.DEPARTMENT_HEAD.ToString()));
});
```

---

### 6.3 Data Protection

**Encryption:**
- Data at rest: AES-256 encryption for sensitive fields
- Data in transit: TLS 1.3 for all HTTP traffic
- Passwords: Argon2id hashing with work factor 3

**SQL Injection Prevention:**
- Parameterized queries only
- No dynamic SQL
- Input validation
- ORM usage (EF Core)

**XSS Protection:**
- Input sanitization
- Output encoding
- Content Security Policy (CSP)
- HttpOnly cookies

---

## 7. Performance Specifications

### 7.1 Performance Targets

| Metric | Target | Measurement |
|--------|---------|-------------|
| **API Response Time** | < 200ms (95th percentile) | API gateway |
| **Page Load Time** | < 2 seconds | Browser |
| **Database Query Time** | < 100ms (average) | Database logs |
| **Grid Rendering** | < 500ms for 1000 rows | Browser performance |
| **Cache Hit Rate** | > 80% | Redis metrics |
| **Concurrent Users** | 500 | Load testing |
| **Transactions/Day** | 10,000 | Database metrics |
| **Uptime** | 99.9% | Monitoring |

---

### 7.2 Optimization Strategies

**Database:**
- Use indexes for frequent queries
- Implement query caching
- Use read replicas for reporting
- Optimize complex queries
- Use connection pooling

**API:**
- Implement response caching
- Use pagination
- Compress responses
- Optimize serialization
- Use async operations

**Frontend:**
- Code splitting and lazy loading
- Image optimization
- Virtual scrolling for large lists
- Client-side caching (TanStack Query)
- Bundle optimization

---

## 8. Testing Specifications

### 8.1 Unit Testing

**Coverage Target:** 80%

**Test Categories:**
- Domain logic tests
- Command handler tests
- Query handler tests
- Validator tests
- Service tests
- Repository tests

**Example:**
```csharp
public class InventoryItemTests
{
    [Fact]
    public void ReceiveStock_ShouldSplitBetweenGeneralAndReserve()
    {
        // Arrange
        var item = new InventoryItem(
            warehouseId: Guid.NewGuid(),
            itemId: Guid.NewGuid()
        );
        var quantity = 1000m;
        var reservePercentage = 20m;
        
        // Act
        item.ReceiveStock(quantity, reservePercentage);
        
        // Assert
        item.GeneralStock.Value.Should().Be(800m);
        item.CommanderReserve.Value.Should().Be(200m);
        item.TotalQuantity.Should().Be(1000m);
    }
    
    [Fact]
    public void ConsumeFromReserve_WithoutCommanderApproval_ShouldThrow()
    {
        // Arrange
        var item = new InventoryItem(
            warehouseId: Guid.NewGuid(),
            itemId: Guid.NewGuid()
        );
        item.ReceiveStock(1000m, 20m);
        
        // Act & Assert
        Assert.Throws<DomainException>(() =>
            item.ConsumeStock(50m, fromReserve: true, requestorRank: UserRank.OFFICER)
        );
    }
}
```

---

### 8.2 Integration Testing

**Test Scenarios:**
- API endpoint tests
- Database operation tests
- Authentication flow tests
- Authorization policy tests
- Cache behavior tests
- Event handler tests

**Example:**
```csharp
public class InventoryApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public InventoryApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetInventoryByWarehouse_ShouldReturnOk()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/api/inventory/warehouses/{warehouseId}/items");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<PagedResult<InventoryItemDto>>();
        content.Should().NotBeNull();
    }
    
    [Fact]
    public async Task GetInventoryByWarehouse_UnauthorizedUser_ShouldReturn401()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/api/inventory/warehouses/{warehouseId}/items");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
```

---

### 8.3 E2E Testing

**Test Scenarios:**
- User login flow
- Requisition creation and approval
- Inventory adjustment
- Commander's Reserve access
- Report generation
- Mobile responsiveness

**Example:**
```typescript
import { test, expect } from '@playwright/test';

test('should create and approve requisition', async ({ page }) => {
  // Login
  await page.goto('/login');
  await page.fill('[name="militaryId"]', '12345');
  await page.fill('[name="password"]', 'password');
  await page.click('button[type="submit"]');
  
  // Navigate to requisitions
  await page.goto('/requisitions');
  await page.click('text=Create Requisition');
  
  // Fill requisition form
  await page.selectOption('[name="sourceWarehouseId"]', '1');
  await page.fill('[name="purpose"]', 'Test requisition');
  await page.click('text=Add Item');
  await page.selectOption('[name="items[0].itemId"]', '1');
  await page.fill('[name="items[0].requestedQuantity"]', '100');
  await page.click('button[type="submit"]');
  
  // Verify requisition created
  await expect(page.locator('text=Requisition created successfully')).toBeVisible();
  
  // Login as commander
  await page.goto('/login');
  await page.fill('[name="militaryId"]', '67890');
  await page.fill('[name="password"]', 'commander-password');
  await page.click('button[type="submit"]');
  
  // Approve requisition
  await page.goto('/requisitions/pending-commander');
  await page.click('button:has-text("Approve")');
  
  // Verify approval
  await expect(page.locator('text=Requisition approved successfully')).toBeVisible();
});
```

---

## 9. Conclusion

This technical specification provides:

✅ **Complete system requirements** (functional and non-functional)  
✅ **Detailed backend specifications** with code examples  
✅ **Comprehensive frontend specifications** with component examples  
✅ **Database specifications** with configuration and schema  
✅ **API specifications** with standards and endpoints  
✅ **Security specifications** with authentication and authorization  
✅ **Performance specifications** with targets and optimization  
✅ **Testing specifications** with examples  

**Next Steps:**
1. Review and approve technical specification
2. Begin Phase 1: Foundation
3. Follow implementation plan
4. Adhere to Clean Architecture principles
5. Maintain Arabic-first approach

---

**Document Status:** Ready for Development  
**Next Review:** During Phase 1 implementation
