using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using EICInventorySystem.Domain.Entities;
using MediatR;

namespace EICInventorySystem.Application.Commands;

// Factory Commands
public record CreateFactoryCommand(string Name, string NameAr, string Code, string? Location) : IRequest<FactoryDto>;
public record UpdateFactoryCommand(int Id, string Name, string NameAr, string Code, string? Location, bool IsActive) : IRequest<FactoryDto>;

// Department Commands
public record CreateDepartmentCommand(string Name, string NameAr, string Code, int FactoryId) : IRequest<DepartmentDto>;
public record UpdateDepartmentCommand(int Id, string Name, string NameAr, string Code, int FactoryId) : IRequest<DepartmentDto>;

// Handlers
public class CreateFactoryCommandHandler : IRequestHandler<CreateFactoryCommand, FactoryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateFactoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FactoryDto> Handle(CreateFactoryCommand request, CancellationToken cancellationToken)
    {
        var factory = new Factory(
            request.Code,
            request.Name,
            request.NameAr,
            "", // Description
            "", // DescriptionArabic
            request.Location ?? "",
            "", // LocationArabic
            0   // CreatedBy
        );

        await _unitOfWork.FactoryRepository.AddAsync(factory, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return new FactoryDto
        {
            Id = factory.Id,
            Name = factory.Name,
            NameAr = factory.NameArabic,
            Code = factory.Code,
            Location = factory.Location,
            IsActive = factory.IsActive
        };
    }
}

public class UpdateFactoryCommandHandler : IRequestHandler<UpdateFactoryCommand, FactoryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFactoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FactoryDto> Handle(UpdateFactoryCommand request, CancellationToken cancellationToken)
    {
        var factory = await _unitOfWork.FactoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (factory == null)
            throw new KeyNotFoundException($"Factory with ID {request.Id} not found");

        factory.UpdateDetails(
            request.Name,
            request.NameAr,
            factory.Description,
            factory.DescriptionArabic,
            request.Location ?? "",
            factory.LocationArabic,
            0 // UpdatedBy
        );

        if (request.IsActive)
            factory.Activate(0);
        else
            factory.Deactivate(0);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return new FactoryDto
        {
            Id = factory.Id,
            Name = factory.Name,
            NameAr = factory.NameArabic,
            Code = factory.Code,
            Location = factory.Location,
            IsActive = factory.IsActive
        };
    }
}

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = new Department(
            request.Code,
            request.Name,
            request.NameAr,
            "", // Description
            "", // DescriptionArabic
            request.FactoryId,
            0   // CreatedBy
        );

        await _unitOfWork.DepartmentRepository.AddAsync(department, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        var factory = await _unitOfWork.FactoryRepository.GetByIdAsync(request.FactoryId, cancellationToken);

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            NameAr = department.NameArabic,
            Code = department.Code,
            FactoryId = department.FactoryId,
            FactoryName = factory?.NameArabic ?? ""
        };
    }
}

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (department == null)
            throw new KeyNotFoundException($"Department with ID {request.Id} not found");

        department.UpdateDetails(
            request.Name,
            request.NameAr,
            department.Description,
            department.DescriptionArabic,
            0 // UpdatedBy
        );
        
        // Note: Department entity does not seem to support changing FactoryId via UpdateDetails or a dedicated method in the snippet provided.
        // Assuming FactoryId should not be changed or we would need a new method in the entity. 
        // For now, we only update details provided by UpdateDetails method.

        await _unitOfWork.CompleteAsync(cancellationToken);

        var factory = await _unitOfWork.FactoryRepository.GetByIdAsync(department.FactoryId, cancellationToken);

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            NameAr = department.NameArabic,
            Code = department.Code,
            FactoryId = department.FactoryId,
            FactoryName = factory?.NameArabic ?? ""
        };
    }
}
