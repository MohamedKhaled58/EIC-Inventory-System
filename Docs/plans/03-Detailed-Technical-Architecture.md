# Detailed Technical Architecture
## Enginerring Industrial Complex Inventory Command System

**Document Version:** 1.0  
**Architecture Date:** January 30, 2025  
**Architect:** Architect Mode

---

## Table of Contents

1. [Technology Stack Details](#1-technology-stack-details)
2. [Backend Architecture Components](#2-backend-architecture-components)
3. [Frontend Architecture Components](#3-frontend-architecture-components)
4. [Database Architecture](#4-database-architecture)
5. [API Design Specifications](#5-api-design-specifications)
6. [Integration Patterns](#6-integration-patterns)
7. [Caching Strategy](#7-caching-strategy)
8. [Logging & Monitoring](#8-logging--monitoring)

---

## 1. Technology Stack Details

### 1.1 Backend Stack

| Component | Technology | Version | Purpose |
|------------|-------------|----------|---------|
| **Framework** | ASP.NET Core | 8.0 | Web API framework |
| **Language** | C# | 12 | Backend language |
| **Runtime** | .NET | 8.0 | Runtime environment |
| **Database** | PostgreSQL | 16.0 | Primary database |
| **ORM** | Entity Framework Core | 8.0 | Data access |
| **Authentication** | ASP.NET Identity | 8.0 | User management |
| **JWT** | System.IdentityModel.Tokens.Jwt | 7.0 | Token-based auth |
| **Mediation** | MediatR | 12.0 | CQRS mediator |
| **Validation** | FluentValidation | 11.9 | Input validation |
| **Mapping** | Mapster | 7.4 | Object mapping |
| **Logging** | Serilog | 3.1 | Structured logging |
| **Caching** | StackExchange.Redis | 2.7 | Distributed cache |
| **Testing** | xUnit, Moq, FluentAssertions | Latest | Unit testing |
| **API Docs** | Swashbuckle.AspNetCore | 6.5 | OpenAPI/Swagger |

---

### 1.2 Frontend Stack

| Component | Technology | Version | Purpose |
|------------|-------------|----------|---------|
| **Framework** | Next.js | 14.0 | React framework |
| **Language** | TypeScript | 5.3 | Type-safe JS |
| **UI Library** | Shadcn/ui + Radix UI | Latest | Component library |
| **Styling** | Tailwind CSS | 3.4 | Utility-first CSS |
| **Data Grid** | ag-Grid Enterprise | 31.0 | Data tables |
| **State (Server)** | TanStack Query | 5.0 | Server state |
| **State (Client)** | Zustand | 4.4 | Client state |
| **Forms** | React Hook Form | 7.48 | Form management |
| **Validation** | Zod | 3.22 | Schema validation |
| **HTTP Client** | Axios | 1.6 | API requests |
| **i18n** | next-intl | 3.0 | Internationalization |
| **Charts** | Recharts | 2.10 | Data visualization |
| **Icons** | Lucide React | 0.300 | Icon library |
| **Testing** | Jest, React Testing Library | Latest | Unit testing |
| **E2E Testing** | Playwright | 1.40 | End-to-end tests |

---

### 1.3 Infrastructure Stack

| Component | Technology | Version | Purpose |
|------------|-------------|----------|---------|
| **Containerization** | Docker | 24.0 | Container runtime |
| **Orchestration** | Kubernetes | 1.28 | Container orchestration |
| **Ingress** | NGINX Ingress Controller | 1.9 | Load balancing |
| **CI/CD** | GitLab CI / Jenkins | Latest | Pipeline automation |
| **Monitoring** | Prometheus + Grafana | Latest | Metrics & dashboards |
| **Logging** | ELK Stack (Elasticsearch, Logstash, Kibana) | 8.11 | Log aggregation |
| **Object Storage** | MinIO | Latest | S3-compatible storage |
| **Secrets** | HashiCorp Vault | Latest | Secret management |
| **Reverse Proxy** | HAProxy | 2.8 | Load balancer |

---

## 2. Backend Architecture Components

### 2.1 Solution Structure

```
EIC.Inventory.System/
├── src/
│   ├── 1. Domain/
│   │   ├── Entities/
│   │   │   ├── InventoryItem.cs
│   │   │   ├── Requisition.cs
│   │   │   ├── Project.cs
│   │   │   ├── User.cs
│   │   │   ├── Warehouse.cs
│   │   │   └── ...
│   │   ├── ValueObjects/
│   │   │   ├── Quantity.cs
│   │   │   ├── Money.cs
│   │   │   ├── ArabicName.cs
│   │   │   └── ...
│   │   ├── Enums/
│   │   │   ├── UserRole.cs
│   │   │   ├── InventoryStatus.cs
│   │   │   ├── TransactionType.cs
│   │   │   └── ...
│   │   ├── Events/
│   │   │   ├── IDomainEvent.cs
│   │   │   ├── StockReceivedEvent.cs
│   │   │   ├── CommanderReserveAccessedEvent.cs
│   │   │   └── ...
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs
│   │   │   ├── InsufficientStockException.cs
│   │   │   └── ...
│   │   └── Interfaces/
│   │       ├── IAggregateRoot.cs
│   │       ├── IRepository.cs
│   │       ├── IUnitOfWork.cs
│   │       └── ...
│   │
│   ├── 2. Application/
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   ├── TransactionBehavior.cs
│   │   │   │   └── ...
│   │   │   ├── Mappings/
│   │   │   │   └── MappingProfile.cs
│   │   │   └── Interfaces/
│   │   │       ├── ICurrentUserService.cs
│   │   │       ├── IDateTimeService.cs
│   │   │       └── ...
│   │   │
│   │   └── Features/
│   │       ├── Inventory/
│   │       │   ├── Commands/
│   │       │   │   ├── CreateInventoryRecordCommand.cs
│   │       │   │   ├── AdjustStockCommand.cs
│   │       │   │   ├── ConsumeStockCommand.cs
│   │       │   │   └── ...
│   │       │   ├── Queries/
│   │       │   │   ├── GetInventoryByWarehouseQuery.cs
│   │       │   │   ├── SearchItemsQuery.cs
│   │       │   │   └── ...
│   │       │   ├── Handlers/
│   │       │   │   ├── CreateInventoryRecordCommandHandler.cs
│   │       │   │   ├── GetInventoryByWarehouseQueryHandler.cs
│   │       │   │   └── ...
│   │       │   ├── Validators/
│   │       │   │   └── CreateInventoryRecordValidator.cs
│   │       │   └── DTOs/
│   │       │       ├── InventoryItemDto.cs
│   │       │       └── ...
│   │       │
│   │       ├── Requisitions/
│   │       │   ├── Commands/
│   │       │   ├── Queries/
│   │       │   ├── Handlers/
│   │       │   ├── Validators/
│   │       │   └── DTOs/
│   │       │
│   │       ├── Projects/
│   │       │   ├── Commands/
│   │       │   ├── Queries/
│   │       │   ├── Handlers/
│   │       │   ├── Validators/
│   │       │   └── DTOs/
│   │       │
│   │       ├── Users/
│   │       │   ├── Commands/
│   │       │   ├── Queries/
│   │       │   ├── Handlers/
│   │       │   ├── Validators/
│   │       │   └── DTOs/
│   │       │
│   │       └── Reports/
│   │           ├── Commands/
│   │           ├── Queries/
│   │           ├── Handlers/
│   │           └── DTOs/
│   │
│   ├── 3. Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── InventoryItemConfiguration.cs
│   │   │   │   ├── RequisitionConfiguration.cs
│   │   │   │   └── ...
│   │   │   ├── Migrations/
│   │   │   └── Repositories/
│   │   │       ├── Repository.cs
│   │   │       ├── InventoryItemRepository.cs
│   │   │       ├── RequisitionRepository.cs
│   │   │       └── ...
│   │   │
│   │   ├── Identity/
│   │   │   ├── ApplicationUser.cs
│   │   │   ├── ApplicationRole.cs
│   │   │   ├── TokenService.cs
│   │   │   └── PasswordHasher.cs
│   │   │
│   │   ├── Cache/
│   │   │   ├── ICacheService.cs
│   │   │   └── RedisCacheService.cs
│   │   │
│   │   ├── Services/
│   │   │   ├── EmailService.cs
│   │   │   ├── PdfService.cs
│   │   │   ├── NotificationService.cs
│   │   │   └── DateTimeService.cs
│   │   │
│   │   └── Files/
│   │       ├── IFileStorageService.cs
│   │       └── MinioFileStorageService.cs
│   │
│   └── 4. WebAPI/
│       ├── Controllers/
│       │   ├── InventoryController.cs
│       │   ├── RequisitionsController.cs
│       │   ├── ProjectsController.cs
│       │   ├── UsersController.cs
│       │   └── ...
│       ├── Middleware/
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   ├── RequestLoggingMiddleware.cs
│       │   ├── RtlContentMiddleware.cs
│       │   └── ...
│       ├── Filters/
│       │   └── ...
│       ├── Extensions/
│       │   └── ServiceCollectionExtensions.cs
│       └── Program.cs
│
├── tests/
│   ├── Unit/
│   │   ├── Domain/
│   │   ├── Application/
│   │   └── Infrastructure/
│   ├── Integration/
│   │   └── ...
│   └── E2E/
│       └── ...
│
├── docker/
│   ├── Dockerfile.backend
│   ├── Dockerfile.frontend
│   └── docker-compose.yml
│
├── k8s/
│   ├── backend/
│   ├── frontend/
│   └── ingress/
│
├── scripts/
│   ├── build.sh
│   ├── deploy.sh
│   └── migrate.sh
│
├── .editorconfig
├── .gitignore
├── Directory.Build.props
├── EIC.Inventory.System.sln
└── README.md
```

---

### 2.2 Domain Layer Components

#### **2.2.1 Base Classes**

```csharp
// Domain/Interfaces/IAggregateRoot.cs
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

// Domain/Interfaces/IDomainEvent.cs
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

// Domain/Interfaces/IEntity.cs
public interface IEntity
{
    Guid Id { get; }
}

// Domain/AggregateRoot.cs
public abstract class AggregateRoot : IEntity, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public Guid Id { get; protected set; }
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

---

#### **2.2.2 Repository Interfaces**

```csharp
// Domain/Interfaces/IRepository.cs
public interface IRepository<T> where T : IEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<T>> FindAsync(
        ISpecification<T> specification,
        CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<int> CountAsync(CancellationToken ct = default);
}

// Domain/Interfaces/IInventoryItemRepository.cs
public interface IInventoryItemRepository : IRepository<InventoryItem>
{
    Task<InventoryItem?> GetByWarehouseAndItemAsync(
        Guid warehouseId,
        Guid itemId,
        CancellationToken ct = default);
    
    Task<IReadOnlyList<InventoryItem>> GetLowStockItemsAsync(
        Guid warehouseId,
        CancellationToken ct = default);
    
    Task<IReadOnlyList<InventoryItem>> GetReserveLowItemsAsync(
        Guid warehouseId,
        CancellationToken ct = default);
}

// Domain/Interfaces/IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    IInventoryItemRepository InventoryItems { get; }
    IRequisitionRepository Requisitions { get; }
    IProjectRepository Projects { get; }
    IUserRepository Users { get; }
    IWarehouseRepository Warehouses { get; }
    
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
```

---

#### **2.2.3 Specification Pattern**

```csharp
// Domain/Interfaces/ISpecification.cs
public interface ISpecification<T>
{
    Expression<Func<T, bool>> ToExpression();
}

// Domain/Specifications/LowStockSpecification.cs
public class LowStockSpecification : ISpecification<InventoryItem>
{
    public Expression<Func<InventoryItem, bool>> ToExpression()
    {
        return item => item.GeneralAvailable <= item.ReorderPoint;
    }
}

// Domain/Specifications/ReserveLowSpecification.cs
public class ReserveLowSpecification : ISpecification<InventoryItem>
{
    public Expression<Func<InventoryItem, bool>> ToExpression()
    {
        return item => item.ReserveAvailable <= item.MinimumReserveRequired;
    }
}

// Domain/Specifications/CompositeSpecification.cs
public class CompositeSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;
    private readonly string _operator;
    
    public CompositeSpecification(
        ISpecification<T> left,
        ISpecification<T> right,
        string @operator)
    {
        _left = left;
        _right = right;
        _operator = @operator;
    }
    
    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();
        
        return _operator switch
        {
            "AND" => Expression.AndAlso(leftExpr, rightExpr),
            "OR" => Expression.OrElse(leftExpr, rightExpr),
            _ => throw new ArgumentException($"Unknown operator: {_operator}")
        };
    }
}
```

---

### 2.3 Application Layer Components

#### **2.3.1 MediatR Behaviors**

```csharp
// Application/Common/Behaviors/ValidationBehavior.cs
public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, ct)));
            
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();
            
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }
        
        return await next();
    }
}

// Application/Common/Behaviors/LoggingBehavior.cs
public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUser;
    
    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId;
        
        _logger.LogInformation(
            "Handling {RequestName} for User {UserId}",
            requestName, userId);
        
        var timer = Stopwatch.StartNew();
        var response = await next();
        timer.Stop();
        
        _logger.LogInformation(
            "Handled {RequestName} in {ElapsedMilliseconds}ms for User {UserId}",
            requestName, timer.ElapsedMilliseconds, userId);
        
        return response;
    }
}

// Application/Common/Behaviors/TransactionBehavior.cs
public class TransactionBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    
    public TransactionBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        // Only use transactions for commands (write operations)
        if (typeof(TRequest).Name.EndsWith("Command"))
        {
            await _unitOfWork.BeginTransactionAsync(ct);
            
            try
            {
                var response = await next();
                await _unitOfWork.CommitTransactionAsync(ct);
                return response;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(ct);
                throw;
            }
        }
        
        return await next();
    }
}
```

---

#### **2.3.2 Result Pattern**

```csharp
// Application/Common/Results/Result.cs
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }
    
    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result Success() => new Result(true, string.Empty);
    public static Result Failure(string error) => new Result(false, error);
}

// Application/Common/Results/Result.cs
public class Result<T> : Result
{
    public T Value { get; }
    
    protected Result(bool isSuccess, T value, string error)
        : base(isSuccess, error)
    {
        Value = value;
    }
    
    public static Result<T> Success(T value) => 
        new Result<T>(true, value, string.Empty);
    
    public new static Result<T> Failure(string error) => 
        new Result<T>(false, default, error);
}
```

---

#### **2.3.3 Current User Service**

```csharp
// Application/Common/Interfaces/ICurrentUserService.cs
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    Guid? FactoryId { get; }
    Guid? DepartmentId { get; }
    UserRole? Role { get; }
    UserRank? Rank { get; }
    bool IsAuthenticated { get; }
}

// Infrastructure/Identity/CurrentUserService.cs
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Guid? UserId => 
        Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)
            ? userId
            : null;
    
    public string? UserName => 
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
    
    public string? Email => 
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
    
    public Guid? FactoryId => 
        Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("FactoryId")?.Value, out var factoryId)
            ? factoryId
            : null;
    
    public Guid? DepartmentId => 
        Guid.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("DepartmentId")?.Value, out var deptId)
            ? deptId
            : null;
    
    public UserRole? Role => 
        Enum.TryParse<UserRole>(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value, out var role)
            ? role
            : null;
    
    public UserRank? Rank => 
        Enum.TryParse<UserRank>(_httpContextAccessor.HttpContext?.User?.FindFirst("Rank")?.Value, out var rank)
            ? rank
            : null;
    
    public bool IsAuthenticated => UserId.HasValue;
}
```

---

### 2.4 Infrastructure Layer Components

#### **2.4.1 Entity Framework Configuration**

```csharp
// Infrastructure/Persistence/Configurations/InventoryItemConfiguration.cs
public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryRecords");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()")
            .ValueGeneratedOnAdd();
        
        // Arabic collation for text fields
        builder.Property(x => x.Item.NameAr)
            .IsRequired()
            .HasMaxLength(200)
            .UseCollation("ar_EG.utf8");
        
        builder.Property(x => x.Item.NameEn)
            .IsRequired()
            .HasMaxLength(200);
        
        // Decimal precision for quantities
        builder.Property(x => x.TotalQuantity)
            .HasPrecision(18, 3)
            .IsRequired();
        
        builder.Property(x => x.GeneralQuantity)
            .HasPrecision(18, 3)
            .IsRequired();
        
        builder.Property(x => x.CommanderReserveQuantity)
            .HasPrecision(18, 3)
            .IsRequired();
        
        // Money precision
        builder.Property(x => x.TotalValue)
            .HasPrecision(18, 2)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(x => new { x.WarehouseId, x.ItemId })
            .IsUnique()
            .HasDatabaseName("UQ_Inventory_Warehouse_Item");
        
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Inventory_Status");
        
        // Relationships
        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.Item)
            .WithMany()
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Concurrency
        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }
}
```

---

#### **2.4.2 Repository Implementation**

```csharp
// Infrastructure/Persistence/Repositories/Repository.cs
public class Repository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }
    
    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await _dbSet.ToListAsync(ct);
    }
    
    public virtual async Task<IReadOnlyList<T>> FindAsync(
        ISpecification<T> specification,
        CancellationToken ct = default)
    {
        var query = ApplySpecification(specification);
        return await query.ToListAsync(ct);
    }
    
    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }
    
    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
    }
    
    public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
    }
    
    public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _dbSet.AnyAsync(x => x.Id == id, ct);
    }
    
    public virtual async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await _dbSet.CountAsync(ct);
    }
    
    protected IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        return _dbSet.Where(specification.ToExpression());
    }
}

// Infrastructure/Persistence/Repositories/InventoryItemRepository.cs
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
        var specification = new LowStockSpecification();
        return await _dbSet
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == warehouseId)
            .Where(specification.ToExpression())
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<InventoryItem>> GetReserveLowItemsAsync(
        Guid warehouseId,
        CancellationToken ct = default)
    {
        var specification = new ReserveLowSpecification();
        return await _dbSet
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == warehouseId)
            .Where(specification.ToExpression())
            .ToListAsync(ct);
    }
}
```

---

#### **2.4.3 Unit of Work Implementation**

```csharp
// Infrastructure/Persistence/ApplicationDbContext.cs
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    // DbSets
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Requisition> Requisitions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<InventoryTransaction> Transactions { get; set; }
    
    // Repository implementations
    public IInventoryItemRepository InventoryItems => 
        new InventoryItemRepository(this);
    
    public IRequisitionRepository Requisitions => 
        new RequisitionRepository(this);
    
    public IProjectRepository Projects => 
        new ProjectRepository(this);
    
    public IUserRepository Users => 
        new UserRepository(this);
    
    public IWarehouseRepository Warehouses => 
        new WarehouseRepository(this);
    
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        // Update audit fields
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUser.UserId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUser.UserId;
                    break;
            }
        }
        
        return await base.SaveChangesAsync(ct);
    }
    
    private IDbContextTransaction? _transaction;
    
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await Database.BeginTransactionAsync(ct);
    }
    
    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
        
        // Global query filters for soft delete
        modelBuilder.Entity<ISoftDeletable>()
            .HasQueryFilter(x => !((ISoftDeletable)x).IsDeleted);
    }
}
```

---

#### **2.4.4 Redis Cache Service**

```csharp
// Infrastructure/Cache/ICacheService.cs
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default);
}

// Infrastructure/Cache/RedisCacheService.cs
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly ILogger<RedisCacheService> _logger;
    
    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger)
    {
        _redis = redis;
        _db = redis.GetDatabase();
        _logger = logger;
    }
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return default;
            
            return JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key: {Key}", key);
            return default;
        }
    }
    
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken ct = default)
    {
        try
        {
            var serialized = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, serialized, expiry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
        }
    }
    
    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try
        {
            await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
        }
    }
    
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        try
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"{prefix}*");
            
            foreach (var key in keys)
            {
                await _db.KeyDeleteAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache keys with prefix: {Prefix}", prefix);
        }
    }
}
```

---

### 2.5 WebAPI Layer Components

#### **2.5.1 Exception Handling Middleware**

```csharp
// WebAPI/Middleware/ExceptionHandlingMiddleware.cs
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
                errorResponse.Errors = validationEx.Errors
                    .Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage })
                    .ToList();
                break;
                
            case DomainException domainEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = domainEx.Message;
                break;
                
            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = notFoundEx.Message;
                break;
                
            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                break;
                
            case DbUpdateConcurrencyException:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                errorResponse.Message = "The record was modified by another user";
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

public class ErrorResponse
{
    public string Message { get; set; }
    public List<object> Errors { get; set; }
}
```

---

#### **2.5.2 JWT Token Service**

```csharp
// Infrastructure/Identity/TokenService.cs
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public TokenService(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }
    
    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("Rank", user.Rank.ToString()),
            new Claim("FactoryId", user.FactoryId?.ToString() ?? string.Empty),
            new Claim("DepartmentId", user.DepartmentId?.ToString() ?? string.Empty)
        };
        
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);
            
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
```

---

## 3. Frontend Architecture Components

### 3.1 Frontend Project Structure

```
eic-inventory-frontend/
├── src/
│   ├── app/
│   │   ├── (auth)/
│   │   │   ├── login/
│   │   │   │   └── page.tsx
│   │   │   └── layout.tsx
│   │   │
│   │   ├── (dashboard)/
│   │   │   ├── layout.tsx
│   │   │   ├── page.tsx
│   │   │   │
│   │   │   ├── inventory/
│   │   │   │   ├── page.tsx
│   │   │   │   ├── create/
│   │   │   │   └── [id]/
│   │   │   │
│   │   │   ├── requisitions/
│   │   │   │   ├── page.tsx
│   │   │   │   ├── create/
│   │   │   │   ├── pending/
│   │   │   │   └── approvals/
│   │   │   │
│   │   │   ├── projects/
│   │   │   │   ├── page.tsx
│   │   │   │   ├── create/
│   │   │   │   └── [id]/
│   │   │   │
│   │   │   ├── reports/
│   │   │   │   ├── page.tsx
│   │   │   │   ├── inventory/
│   │   │   │   └── reserve/
│   │   │   │
│   │   │   └── settings/
│   │   │       └── page.tsx
│   │   │
│   │   └── api/
│   │       └── [...nextauth]/
│   │           └── route.ts
│   │
│   ├── components/
│   │   ├── ui/
│   │   │   ├── button.tsx
│   │   │   ├── input.tsx
│   │   │   ├── select.tsx
│   │   │   ├── table.tsx
│   │   │   ├── dialog.tsx
│   │   │   ├── badge.tsx
│   │   │   └── ...
│   │   │
│   │   ├── forms/
│   │   │   ├── requisition-form.tsx
│   │   │   ├── project-form.tsx
│   │   │   └── user-form.tsx
│   │   │
│   │   ├── tables/
│   │   │   ├── inventory-grid.tsx
│   │   │   ├── requisitions-table.tsx
│   │   │   └── projects-table.tsx
│   │   │
│   │   ├── layout/
│   │   │   ├── header.tsx
│   │   │   ├── sidebar.tsx
│   │   │   ├── footer.tsx
│   │   │   └── command-palette.tsx
│   │   │
│   │   └── shared/
│   │       ├── loading-spinner.tsx
│   │       ├── error-boundary.tsx
│   │       └── notification-toast.tsx
│   │
│   ├── lib/
│   │   ├── api/
│   │   │   ├── client.ts
│   │   │   ├── inventory.ts
│   │   │   ├── requisitions.ts
│   │   │   ├── projects.ts
│   │   │   └── auth.ts
│   │   │
│   │   ├── hooks/
│   │   │   ├── use-auth.ts
│   │   │   ├── use-permissions.ts
│   │   │   ├── use-inventory.ts
│   │   │   └── use-debounce.ts
│   │   │
│   │   ├── utils/
│   │   │   ├── formatters.ts
│   │   │   ├── validators.ts
│   │   │   └── helpers.ts
│   │   │
│   │   └── validations/
│   │       ├── requisition-schema.ts
│   │       ├── project-schema.ts
│   │       └── user-schema.ts
│   │
│   ├── store/
│   │   ├── auth-store.ts
│   │   ├── ui-store.ts
│   │   └── inventory-store.ts
│   │
│   ├── types/
│   │   ├── api.ts
│   │   ├── entities.ts
│   │   ├── enums.ts
│   │   └── index.ts
│   │
│   ├── locales/
│   │   ├── ar.json
│   │   ├── en.json
│   │   └── index.ts
│   │
│   ├── styles/
│   │   ├── globals.css
│   │   └── themes.css
│   │
│   └── middleware.ts
│
├── public/
│   ├── fonts/
│   ├── icons/
│   └── images/
│
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
│
├── .eslintrc.json
├── .prettierrc.json
├── next.config.js
├── tailwind.config.ts
├── tsconfig.json
└── package.json
```

---

### 3.2 API Client Setup

```typescript
// lib/api/client.ts
import axios, { AxiosInstance, AxiosError } from 'axios';
import { toast } from '@/components/ui/use-toast';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: process.env.NEXT_PUBLIC_API_URL,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor - Add auth token
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

    // Response interceptor - Handle errors
    this.client.interceptors.response.use(
      (response) => response,
      async (error: AxiosError) => {
        const originalRequest = error.config;

        // Handle 401 - Unauthorized
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;
          
          // Try to refresh token
          try {
            const refreshToken = localStorage.getItem('refresh_token');
            const response = await this.client.post('/api/auth/refresh', {
              refreshToken,
            });
            
            const { accessToken, refreshToken: newRefreshToken } = response.data;
            localStorage.setItem('access_token', accessToken);
            localStorage.setItem('refresh_token', newRefreshToken);
            
            // Retry original request
            originalRequest.headers.Authorization = `Bearer ${accessToken}`;
            return this.client(originalRequest);
          } catch (refreshError) {
            // Refresh failed - logout
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

  public get<T>(url: string, config?: any) {
    return this.client.get<T>(url, config);
  }

  public post<T>(url: string, data?: any, config?: any) {
    return this.client.post<T>(url, data, config);
  }

  public put<T>(url: string, data?: any, config?: any) {
    return this.client.put<T>(url, data, config);
  }

  public delete<T>(url: string, config?: any) {
    return this.client.delete<T>(url, config);
  }
}

export const apiClient = new ApiClient();
```

---

### 3.3 TanStack Query Setup

```typescript
// lib/api/inventory.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from './client';
import type { InventoryItem, CreateInventoryDto } from '@/types';

export const useInventory = (warehouseId: string) => {
  return useQuery({
    queryKey: ['inventory', warehouseId],
    queryFn: async () => {
      const { data } = await apiClient.get<InventoryItem[]>(
        `/api/inventory/warehouses/${warehouseId}/items`
      );
      return data;
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    cacheTime: 10 * 60 * 1000, // 10 minutes
  });
};

export const useCreateInventory = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (dto: CreateInventoryDto) => {
      const { data } = await apiClient.post<InventoryItem>(
        '/api/inventory',
        dto
      );
      return data;
    },
    onSuccess: (data, variables) => {
      // Invalidate related queries
      queryClient.invalidateQueries(['inventory', variables.warehouseId]);
      queryClient.invalidateQueries(['inventory-summary']);
      
      toast({
        title: 'Success',
        description: 'Inventory item created successfully',
      });
    },
    onError: (error: any) => {
      toast({
        variant: 'destructive',
        title: 'Error',
        description: error.response?.data?.message || 'Failed to create inventory item',
      });
    },
  });
};

export const useAdjustStock = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, quantity, type }: AdjustStockDto) => {
      const { data } = await apiClient.post<InventoryItem>(
        `/api/inventory/${id}/adjust`,
        { quantity, type }
      );
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries(['inventory']);
      queryClient.invalidateQueries(['inventory-summary']);
      
      toast({
        title: 'Success',
        description: 'Stock adjusted successfully',
      });
    },
  });
};
```

---

### 3.4 Zustand Store Setup

```typescript
// store/auth-store.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { User, UserRole } from '@/types';

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  login: (user: User, token: string) => void;
  logout: () => void;
  updateUser: (user: Partial<User>) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      isAuthenticated: false,
      
      login: (user, token) => {
        localStorage.setItem('access_token', token);
        set({ user, isAuthenticated: true });
      },
      
      logout: () => {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        set({ user: null, isAuthenticated: false });
      },
      
      updateUser: (updates) => {
        set((state) => ({
          user: state.user ? { ...state.user, ...updates } : null,
        }));
      },
    }),
    {
      name: 'auth-storage',
    }
  )
);

// store/ui-store.ts
import { create } from 'zustand';

interface UIState {
  sidebarOpen: boolean;
  commandPaletteOpen: boolean;
  currentLocale: 'ar' | 'en';
  theme: 'dark' | 'light';
  toggleSidebar: () => void;
  toggleCommandPalette: () => void;
  setLocale: (locale: 'ar' | 'en') => void;
  setTheme: (theme: 'dark' | 'light') => void;
}

export const useUIStore = create<UIState>((set) => ({
  sidebarOpen: true,
  commandPaletteOpen: false,
  currentLocale: 'ar',
  theme: 'dark',
  
  toggleSidebar: () => set((state) => ({ sidebarOpen: !state.sidebarOpen })),
  toggleCommandPalette: () => set((state) => ({ commandPaletteOpen: !state.commandPaletteOpen })),
  setLocale: (locale) => set({ currentLocale: locale }),
  setTheme: (theme) => set({ theme }),
}));
```

---

### 3.5 ag-Grid Configuration

```typescript
// components/tables/inventory-grid.tsx
'use client';

import { AgGridReact } from 'ag-grid-react';
import type { ColDef, GridOptions } from 'ag-grid-community';
import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine-dark.css';
import { useInventory } from '@/lib/api/inventory';
import { Badge } from '@/components/ui/badge';

export function InventoryGrid({ warehouseId }: { warehouseId: string }) {
  const { data: inventory, isLoading } = useInventory(warehouseId);

  const columnDefs: ColDef[] = [
    {
      field: 'item.code',
      headerName: 'CODE',
      pinned: 'left',
      width: 120,
      cellClass: 'font-mono',
    },
    {
      field: 'item.nameAr',
      headerName: 'الصنف',
      flex: 1,
      cellClass: 'text-right',
    },
    {
      field: 'item.nameEn',
      headerName: 'ITEM NAME',
      flex: 1,
    },
    {
      field: 'totalQuantity',
      headerName: 'TOTAL',
      type: 'numericColumn',
      width: 100,
      cellClass: 'font-mono text-right',
      valueFormatter: (params) => params.value?.toLocaleString('ar-EG') || '',
    },
    {
      field: 'generalQuantity',
      headerName: 'GENERAL',
      type: 'numericColumn',
      width: 100,
      cellClass: 'font-mono text-right',
      valueFormatter: (params) => params.value?.toLocaleString('ar-EG') || '',
    },
    {
      field: 'commanderReserveQuantity',
      headerName: 'RESERVE ⭐',
      type: 'numericColumn',
      width: 120,
      cellClass: 'font-mono text-right bg-military-gold',
      valueFormatter: (params) => params.value?.toLocaleString('ar-EG') || '',
      cellStyle: { fontWeight: 'bold', backgroundColor: '#8b6914' },
    },
    {
      field: 'generalAvailable',
      headerName: 'AVAILABLE',
      type: 'numericColumn',
      width: 100,
      cellClass: 'font-mono text-right',
      valueFormatter: (params) => params.value?.toLocaleString('ar-EG') || '',
    },
    {
      field: 'status',
      headerName: 'STATUS',
      width: 100,
      cellRenderer: (params) => {
        const status = params.value;
        const config = {
          OK: { icon: '🟢', color: 'bg-green-600' },
          LOW: { icon: '🟡', color: 'bg-yellow-600' },
          CRITICAL: { icon: '🔴', color: 'bg-red-600' },
          RESERVE_LOW: { icon: '🔶', color: 'bg-orange-600' },
        }[status];
        
        return (
          <Badge className={config.color}>
            {config.icon} {status}
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
  };

  if (isLoading) {
    return <div className="flex items-center justify-center h-64">Loading...</div>;
  }

  return (
    <div className="ag-theme-alpine-dark" style={{ height: '600px', width: '100%' }}>
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

## 4. Database Architecture

### 4.1 Database Schema

```sql
-- Complex
CREATE TABLE Complexes (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Code VARCHAR(20) UNIQUE NOT NULL,
    NameAr VARCHAR(200) NOT NULL,
    NameEn VARCHAR(200) NOT NULL,
    Location VARCHAR(500),
    CommanderName VARCHAR(200),
    EstablishedDate DATE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy UUID NOT NULL,
    UpdatedAt TIMESTAMP,
    UpdatedBy UUID
);

-- Factories
CREATE TABLE Factories (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ComplexId UUID NOT NULL REFERENCES Complexes(Id),
    Code VARCHAR(30) UNIQUE NOT NULL,
    NameAr VARCHAR(200) NOT NULL,
    NameEn VARCHAR(200) NOT NULL,
    SpecializationType VARCHAR(50) NOT NULL,
    Location VARCHAR(500),
    CommanderName VARCHAR(200),
    CommanderRank VARCHAR(50),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy UUID NOT NULL,
    UpdatedAt TIMESTAMP,
    UpdatedBy UUID
);

CREATE INDEX IX_Factories_ComplexId ON Factories(ComplexId);
CREATE INDEX IX_Factories_Specialization ON Factories(SpecializationType);

-- Warehouses
CREATE TABLE Warehouses (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Code VARCHAR(30) UNIQUE NOT NULL,
    NameAr VARCHAR(200) NOT NULL,
    NameEn VARCHAR(200) NOT NULL,
    Location VARCHAR(500),
    WarehouseKeeperId UUID REFERENCES Users(Id),
    Type VARCHAR(20) NOT NULL, -- CENTRAL or FACTORY
    Capacity DECIMAL(18,3),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    FactoryId UUID REFERENCES Factories(Id),
    ComplexId UUID REFERENCES Complexes(Id),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy UUID NOT NULL,
    UpdatedAt TIMESTAMP,
    UpdatedBy UUID
);

CREATE INDEX IX_Warehouses_Type ON Warehouses(Type);
CREATE INDEX IX_Warehouses_FactoryId ON Warehouses(FactoryId);
CREATE INDEX IX_Warehouses_ComplexId ON Warehouses(ComplexId);

-- Items
CREATE TABLE Items (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Code VARCHAR(30) UNIQUE NOT NULL,
    NameAr VARCHAR(200) NOT NULL,
    NameEn VARCHAR(200) NOT NULL,
    Category VARCHAR(50) NOT NULL,
    SubCategory VARCHAR(100),
    NatoStockNumber VARCHAR(50),
    UnitOfMeasure VARCHAR(20) NOT NULL,
    StandardCost DECIMAL(18,2) NOT NULL,
    AverageCost DECIMAL(18,2),
    Weight DECIMAL(10,3),
    Dimensions JSONB,
    Specifications JSONB,
    Manufacturer VARCHAR(200),
    PartNumber VARCHAR(100),
    IsHazardous BOOLEAN NOT NULL DEFAULT FALSE,
    RequiresSpecialStorage BOOLEAN NOT NULL DEFAULT FALSE,
    StorageConditions TEXT,
    ShelfLife INT,
    MinimumStockLevel DECIMAL(18,3) NOT NULL,
    MaximumStockLevel DECIMAL(18,3),
    ReorderPoint DECIMAL(18,3) NOT NULL,
    ReorderQuantity DECIMAL(18,3),
    LeadTimeDays INT,
    DefaultReservePercentage DECIMAL(5,2) NOT NULL DEFAULT 20.00,
    MinimumReserveQuantity DECIMAL(18,3) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    IsDiscontinued BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy UUID NOT NULL,
    UpdatedAt TIMESTAMP,
    UpdatedBy UUID
);

CREATE INDEX IX_Items_Code ON Items(Code);
CREATE INDEX IX_Items_Category ON Items(Category);
CREATE INDEX IX_Items_NameAr ON Items(NameAr COLLATE "ar_EG.utf8");

-- Inventory Records
CREATE TABLE InventoryRecords (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    WarehouseId UUID NOT NULL REFERENCES Warehouses(Id),
    ItemId UUID NOT NULL REFERENCES Items(Id),
    TotalQuantity DECIMAL(18,3) NOT NULL,
    GeneralQuantity DECIMAL(18,3) NOT NULL,
    CommanderReserveQuantity DECIMAL(18,3) NOT NULL,
    GeneralAllocated DECIMAL(18,3) NOT NULL DEFAULT 0,
    ReserveAllocated DECIMAL(18,3) NOT NULL DEFAULT 0,
    MinimumReserveRequired DECIMAL(18,3) NOT NULL,
    ReorderPoint DECIMAL(18,3) NOT NULL,
    BatchNumber VARCHAR(100),
    LotNumber VARCHAR(100),
    SerialNumber VARCHAR(100),
    ExpiryDate DATE,
    StorageLocation VARCHAR(50),
    AverageCost DECIMAL(18,2),
    LastCost DECIMAL(18,2),
    TotalValue DECIMAL(18,2),
    Status VARCHAR(20) NOT NULL,
    LastPhysicalCount DATE,
    LastCountBy UUID REFERENCES Users(Id),
    RowVersion BYTEA NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy UUID NOT NULL,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy UUID NOT NULL,
    
    CONSTRAINT CHK_TotalQuantity CHECK (TotalQuantity = GeneralQuantity + CommanderReserveQuantity),
    CONSTRAINT CHK_AllocatedQuantity CHECK (
        GeneralAllocated >= 0 AND 
        ReserveAllocated >= 0 AND
        GeneralAllocated <= GeneralQuantity AND
        ReserveAllocated <= CommanderReserveQuantity
    ),
    CONSTRAINT UQ_Inventory_Warehouse_Item UNIQUE (WarehouseId, ItemId)
);

CREATE INDEX IX_Inventory_Warehouse_Item ON InventoryRecords(WarehouseId, ItemId);
CREATE INDEX IX_Inventory_Status ON InventoryRecords(Status);
CREATE INDEX IX_Inventory_ExpiryDate ON InventoryRecords(ExpiryDate);

-- Inventory Transactions
CREATE TABLE InventoryTransactions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    TransactionNumber VARCHAR(50) UNIQUE NOT NULL,
    TransactionType VARCHAR(50) NOT NULL,
    TransactionDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ItemId UUID NOT NULL REFERENCES Items(Id),
    WarehouseId UUID NOT NULL REFERENCES Warehouses(Id),
    Quantity DECIMAL(18,3) NOT NULL,
    SourceWarehouseId UUID REFERENCES Warehouses(Id),
    DestinationWarehouseId UUID REFERENCES Warehouses(Id),
    ProjectId UUID REFERENCES Projects(Id),
    DepartmentId UUID REFERENCES Departments(Id),
    SupplierId UUID REFERENCES Suppliers(Id),
    IsFromCommanderReserve BOOLEAN NOT NULL DEFAULT FALSE,
    CommanderApprovalId UUID REFERENCES Users(Id),
    CommanderApprovalDate TIMESTAMP,
    UnitCost DECIMAL(18,2) NOT NULL,
    TotalCost DECIMAL(18,2) NOT NULL,
    ReferenceType VARCHAR(20),
    ReferenceNumber VARCHAR(100),
    BatchNumber VARCHAR(100),
    LotNumber VARCHAR(100),
    ExpiryDate DATE,
    Notes TEXT,
    Attachments JSONB,
    Status VARCHAR(20) NOT NULL,
    IsReversed BOOLEAN NOT NULL DEFAULT FALSE,
    ReversedBy UUID REFERENCES Users(Id),
    ReversedDate TIMESTAMP,
    ReversingTransactionId UUID REFERENCES InventoryTransactions(Id),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy UUID NOT NULL,
    
    CONSTRAINT CHK_PositiveTransactionQty CHECK (Quantity > 0),
    CONSTRAINT CHK_CommanderReserveApproval CHECK (
        IsFromCommanderReserve = FALSE OR
        (IsFromCommanderReserve = TRUE AND CommanderApprovalId IS NOT NULL)
    )
);

CREATE INDEX IX_Transaction_Date ON InventoryTransactions(TransactionDate DESC);
CREATE INDEX IX_Transaction_Warehouse_Item_Date ON InventoryTransactions(WarehouseId, ItemId, TransactionDate);
CREATE INDEX IX_Transaction_Reference ON InventoryTransactions(ReferenceType, ReferenceNumber);

-- Requisitions
CREATE TABLE Requisitions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    RequestNumber VARCHAR(50) UNIQUE NOT NULL,
    RequisitionType VARCHAR(50) NOT NULL,
    RequestedById UUID NOT NULL REFERENCES Users(Id),
    RequestedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RequiredDate DATE NOT NULL,
    Priority VARCHAR(20) NOT NULL,
    ProjectId UUID REFERENCES Projects(Id),
    DepartmentId UUID REFERENCES Departments(Id),
    SourceWarehouseId UUID NOT NULL REFERENCES Warehouses(Id),
    Purpose TEXT NOT NULL,
    WorkOrderNumber VARCHAR(100),
    Status VARCHAR(30) NOT NULL,
    RequiresCommanderApproval BOOLEAN NOT NULL DEFAULT FALSE,
    FulfilledDate TIMESTAMP,
    FulfilledBy UUID REFERENCES Users(Id),
    PartiallyFulfilled BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy UUID NOT NULL,
    UpdatedAt TIMESTAMP,
    UpdatedBy UUID
);

CREATE INDEX IX_Requisition_Status_Date ON Requisitions(Status, RequestedDate DESC);
CREATE INDEX IX_Requisition_RequestedBy ON Requisitions(RequestedById);
CREATE INDEX IX_Requisition_ProjectId ON Requisitions(ProjectId);

-- Requisition Items
CREATE TABLE RequisitionItems (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    RequisitionId UUID NOT NULL REFERENCES Requisitions(Id) ON DELETE CASCADE,
    ItemId UUID NOT NULL REFERENCES Items(Id),
    RequestedQuantity DECIMAL(18,3) NOT NULL,
    ApprovedQuantity DECIMAL(18,3),
    IssuedQuantity DECIMAL(18,3) NOT NULL DEFAULT 0,
    RemainingQuantity DECIMAL(18,3),
    RequestFromReserve BOOLEAN NOT NULL DEFAULT FALSE,
    ApprovedFromReserve BOOLEAN,
    ItemPurpose TEXT,
    ItemStatus VARCHAR(30) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP
);

CREATE INDEX IX_RequisitionItems_RequisitionId ON RequisitionItems(RequisitionId);

-- Requisition Approvals
CREATE TABLE RequisitionApprovals (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    RequisitionId UUID NOT NULL REFERENCES Requisitions(Id) ON DELETE CASCADE,
    ApproverId UUID NOT NULL REFERENCES Users(Id),
    ApprovalLevel VARCHAR(50) NOT NULL,
    Decision VARCHAR(20) NOT NULL,
    DecisionDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Comments TEXT
);

CREATE INDEX IX_RequisitionApprovals_RequisitionId ON RequisitionApprovals(RequisitionId);
```

---

### 4.2 Database Indexes

```sql
-- Performance indexes for common queries
CREATE INDEX IX_Inventory_Warehouse_Status 
ON InventoryRecords(WarehouseId, Status);

CREATE INDEX IX_Inventory_LowStock 
ON InventoryRecords(WarehouseId) 
WHERE GeneralAvailable <= ReorderPoint;

CREATE INDEX IX_Inventory_ReserveLow 
ON InventoryRecords(WarehouseId) 
WHERE ReserveAvailable <= MinimumReserveRequired;

CREATE INDEX IX_Transaction_Item_Date 
ON InventoryTransactions(ItemId, TransactionDate DESC);

CREATE INDEX IX_Requisition_Pending 
ON Requisitions(Status, RequestedDate) 
WHERE Status IN ('SUBMITTED', 'PENDING_WAREHOUSE', 'PENDING_COMMANDER');

-- Full-text search for Arabic
CREATE INDEX IX_Items_NameAr_FTS 
ON Items 
USING gin(to_tsvector('arabic', NameAr));

CREATE INDEX IX_Items_NameEn_FTS 
ON Items 
USING gin(to_tsvector('english', NameEn));
```

---

## 5. API Design Specifications

### 5.1 API Endpoint Structure

```
/api/v1/
├── auth/
│   ├── POST   /login
│   ├── POST   /logout
│   ├── POST   /refresh
│   └── GET    /me
│
├── inventory/
│   ├── GET    /warehouses/{id}
│   ├── GET    /warehouses/{id}/items
│   ├── GET    /items/{id}
│   ├── GET    /low-stock
│   ├── GET    /reserve/{warehouseId}
│   ├── POST   /transactions/receipt
│   ├── POST   /transactions/issue
│   ├── POST   /transactions/transfer
│   ├── POST   /transactions/adjust
│   └── GET    /transactions/{id}
│
├── requisitions/
│   ├── GET    /
│   ├── POST   /
│   ├── GET    /{id}
│   ├── PUT    /{id}
│   ├── POST   /{id}/approve
│   ├── POST   /{id}/reject
│   ├── POST   /{id}/fulfill
│   ├── GET    /pending
│   └── GET    /pending-commander
│
├── reserve/
│   ├── POST   /release
│   ├── GET    /pending-approvals
│   └── GET    /audit-trail
│
├── projects/
│   ├── GET    /
│   ├── POST   /
│   ├── GET    /{id}
│   ├── PUT    /{id}
│   ├── GET    /{id}/allocations
│   ├── POST   /{id}/allocate
│   └── POST   /{id}/consume
│
├── factories/
│   ├── GET    /
│   ├── GET    /{id}
│   ├── GET    /{id}/warehouses
│   └── GET    /{id}/projects
│
├── departments/
│   ├── GET    /
│   └── GET    /{id}
│
├── users/
│   ├── GET    /
│   ├── POST   /
│   ├── GET    /{id}
│   ├── PUT    /{id}
│   └── DELETE /{id}
│
└── reports/
    ├── GET    /inventory-valuation
    ├── GET    /movement-summary
    ├── GET    /project-costs
    ├── GET    /commander-reserve-usage
    └── POST   /export
```

---

### 5.2 API Response Format

```typescript
// Success Response
interface ApiResponse<T> {
  success: true;
  data: T;
  message?: string;
}

// Error Response
interface ApiError {
  success: false;
  error: {
    code: string;
    message: string;
    details?: any;
  };
}

// Paginated Response
interface PaginatedResponse<T> {
  success: true;
  data: T[];
  pagination: {
    pageNumber: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNext: boolean;
    hasPrevious: boolean;
  };
}
```

---

## 6. Integration Patterns

### 6.1 Event-Driven Integration

```csharp
// Application/Integration/Events/IntegrationEvent.cs
public abstract class IntegrationEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public string EventType { get; }
}

// Application/Integration/Events/StockAdjustedIntegrationEvent.cs
public record StockAdjustedIntegrationEvent(
    Guid InventoryItemId,
    decimal NewQuantity,
    decimal OldQuantity,
    string Reason
) : IntegrationEvent
{
    public override string EventType => "StockAdjusted";
}

// Infrastructure/Services/EventBusService.cs
public class EventBusService : IEventBus
{
    private readonly IServiceProvider _serviceProvider;
    
    public EventBusService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task PublishAsync<T>(T integrationEvent) where T : IntegrationEvent
    {
        // Get all handlers for this event type
        var handlers = _serviceProvider.GetServices<IIntegrationEventHandler<T>>();
        
        // Execute all handlers in parallel
        var tasks = handlers.Select(handler => handler.HandleAsync(integrationEvent));
        await Task.WhenAll(tasks);
    }
}

// Application/Integration/Handlers/NotificationIntegrationEventHandler.cs
public class NotificationIntegrationEventHandler : IIntegrationEventHandler<StockAdjustedIntegrationEvent>
{
    private readonly INotificationService _notification;
    
    public NotificationIntegrationEventHandler(INotificationService notification)
    {
        _notification = notification;
    }
    
    public async Task HandleAsync(StockAdjustedIntegrationEvent @event)
    {
        // Send notification to relevant users
        await _notification.SendAlertAsync(
            recipients: GetRecipients(@event.InventoryItemId),
            subject: "Stock Adjusted",
            message: $"Stock adjusted from {@event.OldQuantity} to {@event.NewQuantity}"
        );
    }
}
```

---

## 7. Caching Strategy

### 7.1 Cache Keys & TTL

| Cache Key Pattern | TTL | Purpose |
|------------------|-----|---------|
| `inventory:{warehouseId}` | 5 min | Warehouse inventory list |
| `inventory:{warehouseId}:{itemId}` | 10 min | Single inventory item |
| `item:{itemId}` | 1 hour | Item details |
| `warehouse:{warehouseId}` | 1 hour | Warehouse details |
| `user:{userId}` | 30 min | User profile |
| `permissions:{userId}` | 30 min | User permissions |
| `requisitions:{status}` | 2 min | Requisition list |
| `reports:{type}:{params}` | 15 min | Report results |

---

### 7.2 Cache Invalidation Strategy

```csharp
// Application/Common/Behaviors/CacheInvalidationBehavior.cs
public class CacheInvalidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICacheService _cache;
    
    public CacheInvalidationBehavior(ICacheService cache)
    {
        _cache = cache;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next();
        
        // Invalidate cache based on request type
        await InvalidateCacheAsync(request);
        
        return response;
    }
    
    private async Task InvalidateCacheAsync<TRequest>(TRequest request)
    {
        switch (request)
        {
            case AdjustStockCommand cmd:
                await _cache.RemoveAsync($"inventory:{cmd.WarehouseId}");
                await _cache.RemoveAsync($"inventory:{cmd.WarehouseId}:{cmd.ItemId}");
                break;
                
            case CreateRequisitionCommand cmd:
                await _cache.RemoveByPrefixAsync("requisitions:");
                break;
                
            case ApproveRequisitionCommand cmd:
                await _cache.RemoveAsync($"requisition:{cmd.RequisitionId}");
                await _cache.RemoveByPrefixAsync("requisitions:");
                break;
        }
    }
}
```

---

## 8. Logging & Monitoring

### 8.1 Serilog Configuration

```csharp
// WebAPI/Extensions/ServiceCollectionExtensions.cs
public static class LoggingExtensions
{
    public static IHostBuilder AddSerilogLogging(this IHostBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "EIC.Inventory.System")
            .Enrich.WithProperty("Environment", builder.GetSetting("Environment"))
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.Elasticsearch(
                nodeUris: new Uri(builder.GetSetting("Elasticsearch:Uri")),
                indexFormat: "eic-inventory-{0:yyyy.MM.dd}",
                autoRegisterTemplateVersion: true
            )
            .CreateLogger();
        
        return builder.UseSerilog();
    }
}
```

---

### 8.2 Structured Logging

```csharp
// Application/Common/Behaviors/LoggingBehavior.cs (Enhanced)
public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken ct)
{
    var requestName = typeof(TRequest).Name;
    var userId = _currentUser.UserId;
    var userName = _currentUser.UserName;
    var factoryId = _currentUser.FactoryId;
    
    using (_logger.BeginScope(new Dictionary<string, object>
    {
        ["RequestName"] = requestName,
        ["UserId"] = userId,
        ["UserName"] = userName,
        ["FactoryId"] = factoryId,
        ["RequestId"] = Guid.NewGuid()
    }))
    {
        _logger.LogInformation("Handling {RequestName}", requestName);
        
        var timer = Stopwatch.StartNew();
        try
        {
            var response = await next();
            timer.Stop();
            
            _logger.LogInformation(
                "Completed {RequestName} in {ElapsedMilliseconds}ms",
                requestName, timer.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            timer.Stop();
            _logger.LogError(ex, 
                "Error handling {RequestName} after {ElapsedMilliseconds}ms",
                requestName, timer.ElapsedMilliseconds);
            throw;
        }
    }
}
```

---

## 9. Conclusion

This detailed technical architecture provides:

✅ **Complete component breakdown** for backend and frontend  
✅ **Clean Architecture** implementation with all layers  
✅ **DDD patterns** with aggregates, value objects, and domain events  
✅ **CQRS implementation** with MediatR  
✅ **Database schema** with all tables and indexes  
✅ **API design** with RESTful endpoints  
✅ **Caching strategy** with Redis  
✅ **Logging and monitoring** with Serilog and ELK  

**Next Steps:**
1. Review and approve technical architecture
2. Create comprehensive implementation plan
3. Develop security and compliance documentation
4. Create deployment and infrastructure plan

---

**Document Status:** Ready for Review  
**Next Review:** After Implementation Plan Completion
