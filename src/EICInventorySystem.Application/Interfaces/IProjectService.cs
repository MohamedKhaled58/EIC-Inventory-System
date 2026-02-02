using EICInventorySystem.Application.Common.DTOs;
using EICInventorySystem.Domain.Enums;

namespace EICInventorySystem.Application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetProjectsAsync(
        int? factoryId = null,
        ProjectStatus? status = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<ProjectDto?> GetProjectByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto project, CancellationToken cancellationToken = default);
    Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto project, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectAsync(int id, CancellationToken cancellationToken = default);
}
