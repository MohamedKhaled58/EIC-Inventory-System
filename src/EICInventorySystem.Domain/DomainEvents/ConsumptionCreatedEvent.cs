namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a consumption record is created
/// </summary>
public class ConsumptionCreatedEvent : DomainEvent
{
    public int ConsumptionId { get; }
    public string ConsumptionNumber { get; }
    public int ProjectId { get; }
    public int DepartmentId { get; }

    public ConsumptionCreatedEvent(
        int consumptionId,
        string consumptionNumber,
        int projectId,
        int departmentId)
    {
        ConsumptionId = consumptionId;
        ConsumptionNumber = consumptionNumber;
        ProjectId = projectId;
        DepartmentId = departmentId;
    }
}
