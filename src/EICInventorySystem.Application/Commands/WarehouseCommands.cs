using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using MediatR;

namespace EICInventorySystem.Application.Commands;

public record CreateWarehouseCommand(string Name, string NameAr, string Code, string Type, int? FactoryId, string? Location) : IRequest<WarehouseDto>;
public record UpdateWarehouseCommand(int Id, string Name, string NameAr, string Code, string Type, int? FactoryId, string? Location, bool IsActive) : IRequest<WarehouseDto>;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, WarehouseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateWarehouseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WarehouseDto> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        // For Type: "Central" or "Factory"
        var warehouse = new Warehouse(
            request.Code,
            request.Name,
            request.NameAr,
            request.Type,
            request.Location ?? "",
            "", // LocationArabic
            0, // CreatedBy
            request.FactoryId
        );

        await _unitOfWork.WarehouseRepository.AddAsync(warehouse, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        string factoryName = "";
        string factoryNameAr = "";

        if (request.FactoryId.HasValue)
        {
            var factory = await _unitOfWork.FactoryRepository.GetByIdAsync(request.FactoryId.Value, cancellationToken);
            factoryName = factory?.Name ?? "";
            factoryNameAr = factory?.NameArabic ?? "";
        }

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            NameAr = warehouse.NameArabic,
            Code = warehouse.Code,
            FactoryId = warehouse.FactoryId,
            FactoryName = factoryName,
            FactoryNameAr = factoryNameAr,
            Type = warehouse.Type,
            Location = warehouse.Location,
            IsActive = warehouse.IsActive
        };
    }
}

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, WarehouseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateWarehouseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WarehouseDto> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await _unitOfWork.WarehouseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (warehouse == null)
            throw new KeyNotFoundException($"Warehouse with ID {request.Id} not found");

         warehouse.UpdateDetails(
            request.Name,
            request.NameAr,
            request.Location ?? "",
            "", // LocationArabic
            0 // UpdatedBy
        );

        if (request.IsActive)
            warehouse.Activate(0);
        else
            warehouse.Deactivate(0);

        await _unitOfWork.CompleteAsync(cancellationToken);

        string factoryName = "";
        string factoryNameAr = "";

        if (warehouse.FactoryId.HasValue)
        {
            var factory = await _unitOfWork.FactoryRepository.GetByIdAsync(warehouse.FactoryId.Value, cancellationToken);
            factoryName = factory?.Name ?? "";
            factoryNameAr = factory?.NameArabic ?? "";
        }

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            NameAr = warehouse.NameArabic,
            Code = warehouse.Code,
            FactoryId = warehouse.FactoryId,
            FactoryName = factoryName,
            FactoryNameAr = factoryNameAr,
            Type = warehouse.Type,
            Location = warehouse.Location,
            IsActive = warehouse.IsActive
        };
    }
}
