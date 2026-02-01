using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Application.Interfaces;
using MediatR;

namespace EICInventorySystem.Application.Queries;

// Factory Queries
public record GetFactoriesQuery(bool? IsActive = null) : IRequest<IEnumerable<FactoryDto>>;
public record GetFactoryByIdQuery(int Id) : IRequest<FactoryDto?>;

// Department Queries
public record GetDepartmentsQuery(int? FactoryId = null) : IRequest<IEnumerable<DepartmentDto>>;
public record GetDepartmentByIdQuery(int Id) : IRequest<DepartmentDto?>;

// Handlers
public class GetFactoriesQueryHandler : IRequestHandler<GetFactoriesQuery, IEnumerable<FactoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFactoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<FactoryDto>> Handle(GetFactoriesQuery request, CancellationToken cancellationToken)
    {
        var factories = await _unitOfWork.FactoryRepository.GetAllAsync(cancellationToken);

        if (request.IsActive.HasValue)
        {
            factories = factories.Where(f => f.IsActive == request.IsActive.Value);
        }

        return factories.Select(f => new FactoryDto
        {
            Id = f.Id,
            Name = f.Name,
            NameAr = f.NameArabic,
            Code = f.Code,
            Location = f.Location ?? "",
            IsActive = f.IsActive
        });
    }
}

public class GetFactoryByIdQueryHandler : IRequestHandler<GetFactoryByIdQuery, FactoryDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFactoryByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FactoryDto?> Handle(GetFactoryByIdQuery request, CancellationToken cancellationToken)
    {
        var f = await _unitOfWork.FactoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (f == null) return null;

        return new FactoryDto
        {
            Id = f.Id,
            Name = f.Name,
            NameAr = f.NameArabic,
            Code = f.Code,
            Location = f.Location ?? "",
            IsActive = f.IsActive
        };
    }
}

public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, IEnumerable<DepartmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDepartmentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await _unitOfWork.DepartmentRepository.GetAllAsync(cancellationToken);
        
        // Note: DepartmentRepository might not include Factory by default depending on implementation.
        // Assuming unitOfWork generic repository behavior, navigation properties are not auto included unless specified.
        // But for filtering by FactoryId, we can check property.
        
        if (request.FactoryId.HasValue)
        {
            departments = departments.Where(d => d.FactoryId == request.FactoryId.Value);
        }

        // We might need to fetch Factory names if not included.
        // For simplicity, we'll assume basic mapping. Ideally we should use a specification or custom repository method with Include.
        // Let's assume we need to join or fetch.
        // Since we don't have specifications readily visible, let's fetch all factories to map names efficiently if list is small.
        var factories = await _unitOfWork.FactoryRepository.GetAllAsync(cancellationToken);
        var factoryDict = factories.ToDictionary(f => f.Id, f => f);

        return departments.Select(d => new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            NameAr = d.NameArabic,
            Code = d.Code,
            FactoryId = d.FactoryId,
            FactoryName = factoryDict.ContainsKey(d.FactoryId) ? factoryDict[d.FactoryId].NameArabic : ""
        });
    }
}

public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDepartmentByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentDto?> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var d = await _unitOfWork.DepartmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (d == null) return null;

        var factory = await _unitOfWork.FactoryRepository.GetByIdAsync(d.FactoryId, cancellationToken);

        return new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            NameAr = d.NameArabic,
            Code = d.Code,
            FactoryId = d.FactoryId,
            FactoryName = factory?.NameArabic ?? ""
        };
    }
}
