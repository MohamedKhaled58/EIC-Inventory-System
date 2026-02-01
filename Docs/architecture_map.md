# EIC Inventory System - Architecture Map

## Overview

The EIC Inventory System follows **Clean Architecture** with **CQRS (Command Query Responsibility Segregation)** pattern using **MediatR** for request/response handling.

---

## Layer Structure

```
┌─────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                        │
│                     (EICInventorySystem.WebAPI)                  │
│  • Controllers (API endpoints)                                   │
│  • Dependency Injection Configuration                            │
│  • Authentication/Authorization Setup                            │
└───────────────────────────┬─────────────────────────────────────┘
                            │ References
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                       APPLICATION LAYER                          │
│                  (EICInventorySystem.Application)                │
│  • Commands/ (Write operations - Create, Update, Delete)        │
│  • Queries/ (Read operations)                                    │
│  • Common/DTOs/ (Data Transfer Objects)                          │
│  • Interfaces/ (Service & Repository contracts)                  │
└───────────────────────────┬─────────────────────────────────────┘
                            │ References
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                       INFRASTRUCTURE LAYER                       │
│                 (EICInventorySystem.Infrastructure)              │
│  • Data/ (ApplicationDbContext, Migrations)                     │
│  • Repositories/ (Repository implementations)                   │
│  • Services/ (Service implementations)                          │
│  • External integrations                                         │
└───────────────────────────┬─────────────────────────────────────┘
                            │ References
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                         DOMAIN LAYER                             │
│                    (EICInventorySystem.Domain)                   │
│  • Entities/ (Business entities with behavior)                  │
│  • Enums/ (Domain enumerations)                                  │
│  • Value Objects                                                 │
│  • Domain Events                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Dependency Flow (CRITICAL RULE)

```
WebAPI → Application → Infrastructure → Domain
   ↓         ↓              ↓              ↓
References  References   References    NO References
   All      Domain &     Domain        (Core/Innermost)
            Interfaces   Only
```

### Key Principle:
- **Application layer CANNOT reference Infrastructure layer directly**
- Application layer defines **Interfaces** (contracts)
- Infrastructure layer **implements** those interfaces
- WebAPI registers dependencies via **Dependency Injection**

---

## CQRS Pattern Implementation

### Commands (Write Operations)

Location: `Application/Commands/`

```
Controller                    Command                Handler              Service/Repository
    │                           │                       │                        │
    │  CreateTransferCommand    │                       │                        │
    │──────────────────────────▶│                       │                        │
    │                           │  IRequestHandler      │                        │
    │                           │──────────────────────▶│                        │
    │                           │                       │  ITransferService      │
    │                           │                       │───────────────────────▶│
    │                           │                       │                        │
    │◀──────────────────────────│◀──────────────────────│◀───────────────────────│
    │       TransferDto         │                       │                        │
```

### Queries (Read Operations)

Location: `Application/Queries/`

```
Controller                     Query                 Handler              Service/Repository
    │                           │                       │                        │
    │  GetTransfersQuery        │                       │                        │
    │──────────────────────────▶│                       │                        │
    │                           │  IRequestHandler      │                        │
    │                           │──────────────────────▶│                        │
    │                           │                       │  ITransferService      │
    │                           │                       │───────────────────────▶│
    │                           │                       │                        │
    │◀──────────────────────────│◀──────────────────────│◀───────────────────────│
    │    IEnumerable<Dto>       │                       │                        │
```

---

## File Naming Conventions

### DTOs (Data Transfer Objects)
- Location: `Application/Common/DTOs/`
- Pattern: `{EntityName}DTOs.cs`
- Examples:
  - [TransferDTOs.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Common/DTOs/TransferDTOs.cs) → Contains [TransferDto](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Common/DTOs/TransferDTOs.cs#3-28), [CreateTransferDto](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Common/DTOs/TransferDTOs.cs#48-55), [ApproveTransferDto](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.WebAPI/Controllers/TransfersController.cs#172-176)
  - [CustodyDTOs.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Common/DTOs/CustodyDTOs.cs) → Contains [CustodyDto](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Common/DTOs/CustodyDTOs.cs#116-123), [CreateCustodyDto](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Common/DTOs/CustodyDTOs.cs#101-115), etc.

### Interfaces (Contracts)
- Location: `Application/Interfaces/`
- Pattern: `I{ServiceName}.cs`
- Examples:
  - [ITransferService.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Interfaces/ITransferService.cs) → Transfer operations contract
  - [IInventoryService.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Interfaces/IInventoryService.cs) → Inventory operations contract
  - [IRepositories.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Interfaces/IRepositories.cs) → Contains [IUnitOfWork](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Interfaces/IRepositories.cs#26-52) and all repository interfaces

### Services (Implementations)
- Location: `Infrastructure/Services/`
- Pattern: `{ServiceName}.cs`
- Examples:
  - [TransferService.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Infrastructure/Services/TransferService.cs) → Implements [ITransferService](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Interfaces/ITransferService.cs#8-66)
  - [OperationalCustodyService.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Infrastructure/Services/OperationalCustodyService.cs) → Implements `ICustodyService`

### Queries
- Location: `Application/Queries/`
- Pattern: `{EntityName}Queries.cs`
- Contains: Query records + Handler classes
- Example structure:
  ```csharp
  // Query record (request)
  public record GetTransfersQuery(...) : IRequest<IEnumerable<TransferDto>>;
  
  // Handler class (processes the request)
  public class GetTransfersQueryHandler : IRequestHandler<GetTransfersQuery, IEnumerable<TransferDto>>
  {
      private readonly ITransferService _service;  // Inject interface, NOT ApplicationDbContext
      
      public async Task<IEnumerable<TransferDto>> Handle(GetTransfersQuery request, ...)
      {
          return await _service.GetTransfersAsync(...);
      }
  }
  ```

### Commands
- Location: `Application/Commands/`
- Pattern: `{EntityName}Commands.cs`
- Structure same as Queries but for write operations

---

## Repository Pattern

### IUnitOfWork (Central Access Point)
Location: [Application/Interfaces/IRepositories.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/Interfaces/IRepositories.cs)

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;  // Generic access
    IUserRepository UserRepository { get; }          // Specific repositories
    IWarehouseRepository WarehouseRepository { get; }
    IItemRepository ItemRepository { get; }
    // ... other repositories
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### Usage in Handlers:
```csharp
public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, IEnumerable<ItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<IEnumerable<ItemDto>> Handle(...)
    {
        // Access specific repository
        var items = await _unitOfWork.ItemRepository.GetAllAsync(cancellationToken);
        
        // Or use generic repository
        var records = await _unitOfWork.Repository<InventoryRecord>().FindAsync(...);
    }
}
```

---

## Domain Entities Property Naming

### Item Entity
```csharp
public class Item : BaseEntity
{
    public string ItemCode { get; }        // NOT "Code"
    public string Name { get; }
    public string NameArabic { get; }      // NOT "NameAr"
    public string Category { get; }        // String, NOT CategoryId
    public string CategoryArabic { get; }
    public string Unit { get; }
    public int ReorderPoint { get; }       // int, NOT decimal
    public int MinimumStock { get; }
    public bool IsActive { get; }
}
```

### Warehouse Entity
```csharp
public class Warehouse : BaseEntity
{
    public string Code { get; }
    public string Name { get; }
    public string NameArabic { get; }      // NOT "NameAr"
    public string Type { get; }            // String: "Central" or "Factory"
    public string Location { get; }
    public int? FactoryId { get; }
    public Factory? Factory { get; }       // Navigation property
}
```

### InventoryRecord Entity
```csharp
public class InventoryRecord : BaseEntity
{
    public int WarehouseId { get; }
    public int ItemId { get; }
    public decimal TotalQuantity { get; }          // NOT "CurrentQuantity"
    public decimal GeneralQuantity { get; }
    public decimal CommanderReserveQuantity { get; }
    public decimal AvailableQuantity => ...;       // Computed
    public Item Item { get; }                      // Navigation
    public Warehouse Warehouse { get; }            // Navigation
}
```

---

## Adding New Feature Checklist

### 1. Create DTOs
Path: `Application/Common/DTOs/{Feature}DTOs.cs`
```csharp
public record FeatureDto { ... }
public record CreateFeatureDto { ... }
```

### 2. Create Interface
Path: `Application/Interfaces/I{Feature}Service.cs`
```csharp
public interface IFeatureService
{
    Task<IEnumerable<FeatureDto>> GetAllAsync(...);
    Task<FeatureDto> CreateAsync(...);
}
```

### 3. Create Queries
Path: `Application/Queries/{Feature}Queries.cs`
```csharp
public record GetFeaturesQuery(...) : IRequest<IEnumerable<FeatureDto>>;

public class GetFeaturesQueryHandler : IRequestHandler<...>
{
    private readonly IFeatureService _service;  // Interface injection
    // Implementation
}
```

### 4. Create Commands
Path: `Application/Commands/{Feature}Commands.cs`
```csharp
public record CreateFeatureCommand(...) : IRequest<FeatureDto>;

public class CreateFeatureCommandHandler : IRequestHandler<...>
{
    private readonly IFeatureService _service;
    // Implementation
}
```

### 5. Implement Service
Path: `Infrastructure/Services/{Feature}Service.cs`
```csharp
public class FeatureService : IFeatureService
{
    private readonly ApplicationDbContext _context;  // CAN use DbContext here
    // Implementation
}
```

### 6. Create Controller
Path: `WebAPI/Controllers/{Feature}Controller.cs`
```csharp
[ApiController]
[Route("api/[controller]")]
public class FeatureController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeatureDto>>> Get()
    {
        var result = await _mediator.Send(new GetFeaturesQuery());
        return Ok(result);
    }
}
```

### 7. Register in DI
Path: [WebAPI/Program.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.WebAPI/Program.cs) or [Infrastructure/DependencyInjection.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Infrastructure/DependencyInjection.cs)
```csharp
services.AddScoped<IFeatureService, FeatureService>();
```

---

## Frontend (React + TypeScript)

### Structure
```
Frontend/src/
├── components/        # Reusable UI components
├── pages/            # Page components (route targets)
├── services/         # API client services
├── stores/           # State management (Zustand)
├── types/            # TypeScript type definitions
└── App.tsx           # Route definitions
```

### API Client Pattern
```typescript
// services/featureService.ts
import apiClient from './apiClient';

export const featureService = {
    getAll: () => apiClient.get('/features'),
    create: (data: CreateFeatureDto) => apiClient.post('/features', data),
};
```

---

## Dependency Injection Registration

### Location
Path: [Infrastructure/DependencyInjection.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Infrastructure/DependencyInjection.cs)

### Registered Services (Current)

```csharp
public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // DbContext
    services.AddDbContext<ApplicationDbContext>(...);

    // Unit of Work
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Repositories
    services.AddScoped<IUserRepository, UserRepository>();
    services.AddScoped<IFactoryRepository, FactoryRepository>();
    services.AddScoped<IWarehouseRepository, WarehouseRepository>();
    services.AddScoped<IItemRepository, ItemRepository>();
    services.AddScoped<IProjectRepository, ProjectRepository>();
    services.AddScoped<IDepartmentRepository, DepartmentRepository>();
    services.AddScoped<IRequisitionRepository, RequisitionRepository>();
    services.AddScoped<ITransferRepository, TransferRepository>();
    // ... more repositories

    // Core Services
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<IInventoryService, InventoryService>();
    services.AddScoped<IRequisitionService, RequisitionService>();
    services.AddScoped<ITransferService, TransferService>();          // ← ADD FOR TRANSFERS
    services.AddScoped<ICommanderReserveService, CommanderReserveService>();
    services.AddScoped<INotificationService, NotificationService>();
    services.AddScoped<IReportService, ReportService>();
    // ... more services

    // BOQ & Custody Services
    services.AddScoped<IProjectBOQService, ProjectBOQService>();
    services.AddScoped<IWorkerService, WorkerService>();
    services.AddScoped<IOperationalCustodyService, OperationalCustodyService>();  // Note: NOT ICustodyService
}
```

### MediatR Registration (Application Layer)
Path: [Application/DependencyInjection.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/DependencyInjection.cs)

```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
```
> [!IMPORTANT]
> MediatR auto-discovers all `IRequestHandler<,>` implementations in the Application assembly.
> When you create a new Query/Command Handler, it's automatically registered!

### Common DI Errors and Fixes

| Error | Cause | Fix |
|-------|-------|-----|
| `Unable to resolve service for type 'IXxxService'` | Service not registered in DI | Add `services.AddScoped<IXxxService, XxxService>();` in [DependencyInjection.cs](file:///d:/EIC%20Inventory%20System/src/EICInventorySystem.Application/DependencyInjection.cs) |
| `Error while validating service descriptor ... IRequestHandler` | Handler depends on unregistered service | Register the missing service in DI |

---

## Summary

| Layer | References | Contains | Key Rule |
|-------|------------|----------|----------|
| **Domain** | Nothing | Entities, Enums | Pure business logic, no external deps |
| **Application** | Domain | DTOs, Interfaces, Queries, Commands | Define contracts, NO Infrastructure refs |
| **Infrastructure** | Domain, Application | DbContext, Services, Repositories | Implement contracts |
| **WebAPI** | All | Controllers, DI Config | Entry point, orchestration |

