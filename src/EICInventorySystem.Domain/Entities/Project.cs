namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Project entity for tracking production projects
/// </summary>
public class Project : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NameArabic { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string DescriptionArabic { get; private set; } = null!;
    public int FactoryId { get; private set; }
    public int? ManagerId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public decimal Budget { get; private set; }
    public decimal SpentAmount { get; private set; }
    public ProjectStatus Status { get; private set; }
    public int Priority { get; private set; } // 1=Low, 2=Medium, 3=High, 4=Critical
    public bool IsActive { get; private set; }

    // Navigation properties
    public Factory Factory { get; private set; } = null!;
    public User? Manager { get; private set; }
    public ICollection<User> TeamMembers { get; private set; } = new List<User>();
    public ICollection<ProjectAllocation> Allocations { get; private set; } = new List<ProjectAllocation>();
    public ICollection<Consumption> Consumptions { get; private set; } = new List<Consumption>();
    public ICollection<Item> Items { get; private set; } = new List<Item>();
    public ICollection<ItemTransactionHistory> TransactionHistory { get; private set; } = new List<ItemTransactionHistory>();

    private Project() { }

    public Project(
        string code,
        string name,
        string nameArabic,
        string description,
        string descriptionArabic,
        int factoryId,
        DateTime startDate,
        decimal budget,
        int priority,
        int createdBy,
        int? managerId = null,
        DateTime? endDate = null) : base(createdBy)
    {
        Code = code;
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        FactoryId = factoryId;
        ManagerId = managerId;
        StartDate = startDate;
        EndDate = endDate;
        Budget = budget;
        SpentAmount = 0;
        Status = ProjectStatus.Planning;
        Priority = priority;
        IsActive = true;
    }

    public void UpdateDetails(
        string name,
        string nameArabic,
        string description,
        string descriptionArabic,
        DateTime? endDate,
        decimal budget,
        int priority,
        int updatedBy)
    {
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        EndDate = endDate;
        Budget = budget;
        Priority = priority;
        Update(updatedBy);
    }

    public void AssignManager(int managerId, int updatedBy)
    {
        ManagerId = managerId;
        Update(updatedBy);
    }

    public void UpdateStatus(ProjectStatus status, int updatedBy)
    {
        Status = status;
        Update(updatedBy);
    }

    public void AddSpending(decimal amount, int updatedBy)
    {
        SpentAmount += amount;
        Update(updatedBy);
    }

    public void Activate(int updatedBy)
    {
        IsActive = true;
        Update(updatedBy);
    }

    public void Deactivate(int updatedBy)
    {
        IsActive = false;
        Update(updatedBy);
    }

    public decimal GetRemainingBudget()
    {
        return Budget - SpentAmount;
    }

    public bool IsOverBudget()
    {
        return SpentAmount > Budget;
    }
}
