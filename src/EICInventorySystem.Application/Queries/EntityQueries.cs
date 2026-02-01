using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Queries;

// Transfer Queries
public record GetTransfersQuery(int? FromWarehouseId = null, int? ToWarehouseId = null, string? Status = null, int PageNumber = 1, int PageSize = 50) : IRequest<IEnumerable<TransferDto>>;
public record GetTransferByIdQuery(int Id) : IRequest<TransferDto?>;

// Warehouse Queries
public record GetWarehousesQuery(int? FactoryId = null, string? Type = null, bool? IsActive = null) : IRequest<IEnumerable<WarehouseDto>>;
public record GetWarehouseByIdQuery(int Id) : IRequest<WarehouseDto?>;
public record SearchWarehousesQuery(string? Query = null, int? FactoryId = null) : IRequest<IEnumerable<WarehouseDto>>;

// Item Queries
public record GetItemsQuery(int? CategoryId = null, string? Type = null, bool? IsActive = null, int PageNumber = 1, int PageSize = 100) : IRequest<IEnumerable<ItemDto>>;
public record GetItemByIdQuery(int Id) : IRequest<ItemDto?>;
public record SearchItemsQuery(string? Query = null, int? WarehouseId = null, int? CategoryId = null) : IRequest<IEnumerable<ItemDto>>;
public record GetItemCategoriesQuery() : IRequest<IEnumerable<ItemCategoryDto>>;
public record GetItemsWithInventoryQuery(int WarehouseId) : IRequest<IEnumerable<ItemWithInventoryDto>>;

// Commander Reserve Query
public record GetPendingCommanderReserveRequestsQuery() : IRequest<IEnumerable<CommanderReserveRequestDto>>;

// Query Handlers
public class GetTransfersQueryHandler : IRequestHandler<GetTransfersQuery, IEnumerable<TransferDto>>
{
    private readonly ITransferService _transferService;

    public GetTransfersQueryHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<IEnumerable<TransferDto>> Handle(GetTransfersQuery request, CancellationToken cancellationToken)
    {
        TransferStatus? status = null;
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<TransferStatus>(request.Status, out var parsedStatus))
        {
            status = parsedStatus;
        }
        
        return await _transferService.GetTransfersAsync(request.FromWarehouseId, request.ToWarehouseId, status, null, null, cancellationToken);
    }
}

public class GetTransferByIdQueryHandler : IRequestHandler<GetTransferByIdQuery, TransferDto?>
{
    private readonly ITransferService _transferService;

    public GetTransferByIdQueryHandler(ITransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<TransferDto?> Handle(GetTransferByIdQuery request, CancellationToken cancellationToken)
    {
        return await _transferService.GetTransferByIdAsync(request.Id, cancellationToken);
    }
}

public class GetWarehousesQueryHandler : IRequestHandler<GetWarehousesQuery, IEnumerable<WarehouseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWarehousesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WarehouseDto>> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        var warehouses = await _unitOfWork.WarehouseRepository.GetAllAsync(cancellationToken);

        return warehouses.Select(w => new WarehouseDto
        {
            Id = w.Id,
            Name = w.Name,
            NameAr = w.NameArabic,
            Code = w.Code,
            FactoryId = w.FactoryId,
            FactoryName = w.Factory?.Name ?? "",
            FactoryNameAr = w.Factory?.NameArabic ?? "",
            Type = w.Type,
            Location = w.Location ?? "",
            IsActive = w.IsActive
        });
    }
}

public class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, WarehouseDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWarehouseByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WarehouseDto?> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var w = await _unitOfWork.WarehouseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (w == null) return null;

        return new WarehouseDto
        {
            Id = w.Id,
            Name = w.Name,
            NameAr = w.NameArabic,
            Code = w.Code,
            FactoryId = w.FactoryId,
            FactoryName = w.Factory?.Name ?? "",
            FactoryNameAr = w.Factory?.NameArabic ?? "",
            Type = w.Type,
            Location = w.Location ?? "",
            IsActive = w.IsActive
        };
    }
}

public class SearchWarehousesQueryHandler : IRequestHandler<SearchWarehousesQuery, IEnumerable<WarehouseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchWarehousesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WarehouseDto>> Handle(SearchWarehousesQuery request, CancellationToken cancellationToken)
    {
        var warehouses = await _unitOfWork.WarehouseRepository.GetAllAsync(cancellationToken);
        
        var filtered = warehouses.Where(w => w.IsActive);
        
        if (!string.IsNullOrEmpty(request.Query))
            filtered = filtered.Where(w => w.Name.Contains(request.Query) || w.NameArabic.Contains(request.Query) || w.Code.Contains(request.Query));

        if (request.FactoryId.HasValue)
            filtered = filtered.Where(w => w.FactoryId == request.FactoryId);

        return filtered.Take(50).Select(w => new WarehouseDto
        {
            Id = w.Id,
            Name = w.Name,
            NameAr = w.NameArabic,
            Code = w.Code,
            FactoryId = w.FactoryId,
            FactoryName = w.Factory?.Name ?? "",
            FactoryNameAr = w.Factory?.NameArabic ?? "",
            Type = w.Type,
            Location = w.Location ?? "",
            IsActive = w.IsActive
        });
    }
}

public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, IEnumerable<ItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetItemsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ItemDto>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.ItemRepository.GetAllAsync(cancellationToken);

        return items.Select(i => new ItemDto
        {
            Id = i.Id,
            Code = i.ItemCode,
            Name = i.Name,
            NameAr = i.NameArabic,
            Description = i.Description ?? "",
            CategoryId = 0,
            CategoryName = i.Category ?? "",
            Unit = i.Unit,
            MinimumStock = i.MinimumStock,
            ReorderLevel = i.ReorderPoint,
            IsActive = i.IsActive
        });
    }
}

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetItemByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ItemDto?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var i = await _unitOfWork.ItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (i == null) return null;

        return new ItemDto
        {
            Id = i.Id,
            Code = i.ItemCode,
            Name = i.Name,
            NameAr = i.NameArabic,
            Description = i.Description ?? "",
            CategoryId = 0,
            CategoryName = i.Category ?? "",
            Unit = i.Unit,
            MinimumStock = i.MinimumStock,
            ReorderLevel = i.ReorderPoint,
            IsActive = i.IsActive
        };
    }
}

public class SearchItemsQueryHandler : IRequestHandler<SearchItemsQuery, IEnumerable<ItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchItemsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ItemDto>> Handle(SearchItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.ItemRepository.GetAllAsync(cancellationToken);
        var filtered = items.Where(i => i.IsActive);

        if (!string.IsNullOrEmpty(request.Query))
            filtered = filtered.Where(i => i.Name.Contains(request.Query) || i.NameArabic.Contains(request.Query) || i.ItemCode.Contains(request.Query));

        var result = filtered.Take(50).Select(i => new ItemDto
        {
            Id = i.Id,
            Code = i.ItemCode,
            Name = i.Name,
            NameAr = i.NameArabic,
            Description = i.Description ?? "",
            CategoryId = 0,
            CategoryName = i.Category ?? "",
            CategoryNameAr = i.CategoryArabic ?? "",
            Unit = i.Unit,
            UnitAr = i.UnitOfMeasureArabic ?? "",
            MinimumStock = i.MinimumStock,
            ReorderLevel = i.ReorderPoint,
            IsActive = i.IsActive,
            IsCritical = i.IsCritical,
            ReservePercentage = i.ReservePercentage,
            UnitPrice = i.StandardCost
        });

        // If warehouseId is specified, include inventory information
        if (request.WarehouseId.HasValue)
        {
            var inventoryRecords = await _unitOfWork.Repository<EICInventorySystem.Domain.Entities.InventoryRecord>()
                .FindAsync(ir => ir.WarehouseId == request.WarehouseId.Value, cancellationToken);

            var inventoryDict = inventoryRecords.ToDictionary(ir => ir.ItemId, ir => ir);

            return result.Select(i => {
                if (inventoryDict.TryGetValue(i.Id, out var inv))
                {
                    i.AvailableQuantity = inv.AvailableQuantity;
                    i.TotalQuantity = inv.TotalQuantity;
                    i.ReservedQuantity = inv.ReserveAllocated;
                    i.CommanderReserveQuantity = inv.CommanderReserveQuantity;
                }
                return i;
            });
        }

        return result;
    }
}

public class GetItemCategoriesQueryHandler : IRequestHandler<GetItemCategoriesQuery, IEnumerable<ItemCategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetItemCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ItemCategoryDto>> Handle(GetItemCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Get unique categories from items since ItemCategory entity doesn't exist
        var items = await _unitOfWork.ItemRepository.GetAllAsync(cancellationToken);
        var categories = items
            .Where(i => !string.IsNullOrEmpty(i.Category))
            .Select(i => new { Category = i.Category, CategoryArabic = i.CategoryArabic })
            .Distinct()
            .Select((c, index) => new ItemCategoryDto
            {
                Id = index + 1,
                Name = c.Category ?? "",
                NameAr = c.CategoryArabic ?? "",
                Code = c.Category ?? ""
            });

        return categories;
    }
}

public class GetItemsWithInventoryQueryHandler : IRequestHandler<GetItemsWithInventoryQuery, IEnumerable<ItemWithInventoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetItemsWithInventoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ItemWithInventoryDto>> Handle(GetItemsWithInventoryQuery request, CancellationToken cancellationToken)
    {
        // Get inventory records for the warehouse
        var inventoryRecords = await _unitOfWork.Repository<EICInventorySystem.Domain.Entities.InventoryRecord>()
            .FindAsync(ir => ir.WarehouseId == request.WarehouseId && ir.TotalQuantity > 0, cancellationToken);

        return inventoryRecords.Select(ir => new ItemWithInventoryDto
        {
            Id = ir.Item?.Id ?? 0,
            Code = ir.Item?.ItemCode ?? "",
            Name = ir.Item?.Name ?? "",
            NameAr = ir.Item?.NameArabic ?? "",
            CategoryName = ir.Item?.Category ?? "",
            Unit = ir.Item?.Unit ?? "",
            CurrentQuantity = ir.TotalQuantity,
            AvailableQuantity = ir.AvailableQuantity,
            ReservedQuantity = ir.ReserveAllocated,
            CommanderReserve = ir.CommanderReserveQuantity
        });
    }
}

public class GetPendingCommanderReserveRequestsQueryHandler : IRequestHandler<GetPendingCommanderReserveRequestsQuery, IEnumerable<CommanderReserveRequestDto>>
{
    public Task<IEnumerable<CommanderReserveRequestDto>> Handle(GetPendingCommanderReserveRequestsQuery request, CancellationToken cancellationToken)
    {
        // Return empty list for now as CommanderReserveRequest entity might not exist yet
        return Task.FromResult<IEnumerable<CommanderReserveRequestDto>>(new List<CommanderReserveRequestDto>());
    }
}
