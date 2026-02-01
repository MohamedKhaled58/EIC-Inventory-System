namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a project budget is exceeded
/// </summary>
public class ProjectBudgetExceededEvent : DomainEvent
{
    public int ProjectId { get; }
    public string ProjectCode { get; }
    public decimal Budget { get; }
    public decimal SpentAmount { get; }
    public decimal ExceededAmount { get; }
    public DateTime ExceededDate { get; }
    public int UserId { get; }

    public ProjectBudgetExceededEvent(
        int projectId,
        string projectCode,
        decimal budget,
        decimal spentAmount,
        decimal exceededAmount,
        DateTime exceededDate,
        int userId)
    {
        ProjectId = projectId;
        ProjectCode = projectCode;
        Budget = budget;
        SpentAmount = spentAmount;
        ExceededAmount = exceededAmount;
        ExceededDate = exceededDate;
        UserId = userId;
    }
}
