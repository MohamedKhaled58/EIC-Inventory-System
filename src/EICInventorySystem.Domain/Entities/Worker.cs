namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Worker entity - represents a worker who can receive operational custody
/// Workers DO NOT have system access - they are data records only
/// Items are issued TO workers as operational custody
/// </summary>
public class Worker : BaseEntity
{
    public string WorkerCode { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NameArabic { get; private set; } = null!;
    public string? MilitaryRank { get; private set; }
    public string? MilitaryRankArabic { get; private set; }
    public string? NationalId { get; private set; }
    public string? Phone { get; private set; }
    public int FactoryId { get; private set; }
    public int DepartmentId { get; private set; }
    public DateTime JoinDate { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public Factory Factory { get; private set; } = null!;
    public Department Department { get; private set; } = null!;
    public ICollection<OperationalCustody> Custodies { get; private set; } = new List<OperationalCustody>();

    private Worker() { }

    public Worker(
        string workerCode,
        string name,
        string nameArabic,
        int factoryId,
        int departmentId,
        int createdBy,
        string? militaryRank = null,
        string? militaryRankArabic = null,
        string? nationalId = null,
        string? phone = null,
        DateTime? joinDate = null) : base(createdBy)
    {
        WorkerCode = workerCode;
        Name = name;
        NameArabic = nameArabic;
        MilitaryRank = militaryRank;
        MilitaryRankArabic = militaryRankArabic;
        NationalId = nationalId;
        Phone = phone;
        FactoryId = factoryId;
        DepartmentId = departmentId;
        JoinDate = joinDate ?? DateTime.UtcNow;
        IsActive = true;
    }

    public void UpdateDetails(
        string name,
        string nameArabic,
        int departmentId,
        int updatedBy,
        string? militaryRank = null,
        string? militaryRankArabic = null,
        string? nationalId = null,
        string? phone = null)
    {
        Name = name;
        NameArabic = nameArabic;
        DepartmentId = departmentId;
        MilitaryRank = militaryRank;
        MilitaryRankArabic = militaryRankArabic;
        NationalId = nationalId;
        Phone = phone;
        Update(updatedBy);
    }

    public void TransferToDepartment(int newDepartmentId, int updatedBy)
    {
        DepartmentId = newDepartmentId;
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

    /// <summary>
    /// Gets the total count of active custodies for this worker
    /// </summary>
    public int GetActiveCustodyCount()
    {
        return Custodies?.Count(c => c.Status == Enums.CustodyStatus.Active) ?? 0;
    }
}

