namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a project is created
/// </summary>
public class ProjectCreatedEvent : DomainEvent
{
    public int ProjectId { get; }
    public string ProjectCode { get; }
    public string ProjectName { get; }
    public int FactoryId { get; }
    public decimal Budget { get; }
    public DateTime StartDate { get; }
    public int Priority { get; }
    public int CreatedBy { get; }

    public ProjectCreatedEvent(
        int projectId,
        string projectCode,
        string projectName,
        int factoryId,
        decimal budget,
        DateTime startDate,
        int priority,
        int createdBy)
    {
        ProjectId = projectId;
        ProjectCode = projectCode;
        ProjectName = projectName;
        FactoryId = factoryId;
        Budget = budget;
        StartDate = startDate;
        Priority = priority;
        CreatedBy = createdBy;
    }
}
