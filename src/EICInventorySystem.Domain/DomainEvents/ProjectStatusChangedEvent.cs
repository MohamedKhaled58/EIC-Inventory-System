namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a project status is changed
/// </summary>
public class ProjectStatusChangedEvent : DomainEvent
{
    public int ProjectId { get; }
    public string ProjectCode { get; }
    public string PreviousStatus { get; }
    public string NewStatus { get; }
    public DateTime StatusChangedDate { get; }
    public int ChangedBy { get; }

    public ProjectStatusChangedEvent(
        int projectId,
        string projectCode,
        string previousStatus,
        string newStatus,
        DateTime statusChangedDate,
        int changedBy)
    {
        ProjectId = projectId;
        ProjectCode = projectCode;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        StatusChangedDate = statusChangedDate;
        ChangedBy = changedBy;
    }
}
