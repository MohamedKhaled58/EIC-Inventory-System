namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a return is created
/// </summary>
public class ReturnCreatedEvent : DomainEvent
{
    public int ReturnId { get; }
    public string ReturnNumber { get; }
    public int ProjectId { get; }
    public int DepartmentId { get; }

    public ReturnCreatedEvent(
        int returnId,
        string returnNumber,
        int projectId,
        int departmentId)
    {
        ReturnId = returnId;
        ReturnNumber = returnNumber;
        ProjectId = projectId;
        DepartmentId = departmentId;
    }
}
