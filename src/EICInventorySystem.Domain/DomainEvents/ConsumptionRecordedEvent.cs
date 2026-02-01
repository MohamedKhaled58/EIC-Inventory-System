namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when material consumption is recorded
/// </summary>
public class ConsumptionRecordedEvent : DomainEvent
{
    public int ConsumptionId { get; }
    public string ConsumptionNumber { get; }
    public int ProjectId { get; }
    public int DepartmentId { get; }
    public DateTime ConsumptionDate { get; }

    public ConsumptionRecordedEvent(
        int consumptionId,
        string consumptionNumber,
        int projectId,
        int departmentId,
        DateTime consumptionDate)
    {
        ConsumptionId = consumptionId;
        ConsumptionNumber = consumptionNumber;
        ProjectId = projectId;
        DepartmentId = departmentId;
        ConsumptionDate = consumptionDate;
    }
}
