namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// System user with role-based permissions
/// </summary>
public class User : BaseEntity
{
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string FullName { get; private set; } = null!;
    public string FullNameArabic { get; private set; } = null!;
    public string Role { get; private set; } = null!; // "ComplexCommander", "FactoryCommander", "CentralWarehouseKeeper", "FactoryWarehouseKeeper", "DepartmentHead", "ProjectManager", "Officer", "CivilEngineer", "Worker", "Auditor"
    public int? FactoryId { get; private set; }
    public int? DepartmentId { get; private set; }
    public int? ProjectId { get; private set; }
    public int? WarehouseId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? NationalId { get; private set; }
    public string? MilitaryRank { get; private set; }
    public string? EmployeeNumber { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    public DateTime? PasswordChangedAt { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiryTime { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    // Navigation properties
    public Factory? Factory { get; private set; }
    public Department? Department { get; private set; }
    public Project? Project { get; private set; }
    public Warehouse? Warehouse { get; private set; }
    public ICollection<Requisition> CreatedRequisitions { get; private set; } = new List<Requisition>();
    public ICollection<Requisition> ApprovedRequisitions { get; private set; } = new List<Requisition>();
    public ICollection<Transfer> CreatedTransfers { get; private set; } = new List<Transfer>();
    public ICollection<Transfer> ApprovedTransfers { get; private set; } = new List<Transfer>();
    public ICollection<InventoryTransaction> Transactions { get; private set; } = new List<InventoryTransaction>();
    public ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();

    private User() { }

    public User(
        string username,
        string email,
        string passwordHash,
        string fullName,
        string fullNameArabic,
        string role,
        int createdBy,
        int? factoryId = null,
        int? departmentId = null,
        int? projectId = null,
        int? warehouseId = null,
        string? phoneNumber = null,
        string? nationalId = null,
        string? militaryRank = null,
        string? employeeNumber = null) : base(createdBy)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        FullNameArabic = fullNameArabic;
        Role = role;
        FactoryId = factoryId;
        DepartmentId = departmentId;
        ProjectId = projectId;
        WarehouseId = warehouseId;
        IsActive = true;
        PhoneNumber = phoneNumber;
        NationalId = nationalId;
        MilitaryRank = militaryRank;
        EmployeeNumber = employeeNumber;
    }

    public void UpdateProfile(
        string fullName,
        string fullNameArabic,
        string? phoneNumber = null,
        int updatedBy = 0)
    {
        FullName = fullName;
        FullNameArabic = fullNameArabic;
        PhoneNumber = phoneNumber;
        Update(updatedBy);
    }

    public void UpdatePassword(string newPasswordHash, int updatedBy)
    {
        PasswordHash = newPasswordHash;
        PasswordChangedAt = DateTime.UtcNow;
        Update(updatedBy);
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }

    public void SetPasswordResetToken(string token, DateTime expiryTime)
    {
        PasswordResetToken = token;
        PasswordResetTokenExpiryTime = expiryTime;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetToken = null;
        PasswordResetTokenExpiryTime = null;
    }

    public void UpdateProfileImage(string imageUrl, int updatedBy)
    {
        ProfileImageUrl = imageUrl;
        Update(updatedBy);
    }

    public void AssignToFactory(int factoryId, int updatedBy)
    {
        FactoryId = factoryId;
        Update(updatedBy);
    }

    public void AssignToDepartment(int departmentId, int updatedBy)
    {
        DepartmentId = departmentId;
        Update(updatedBy);
    }

    public void AssignToProject(int projectId, int updatedBy)
    {
        ProjectId = projectId;
        Update(updatedBy);
    }

    public void AssignToWarehouse(int warehouseId, int updatedBy)
    {
        WarehouseId = warehouseId;
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

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public bool CanAccessCommanderReserve()
    {
        return Role == "ComplexCommander" || Role == "FactoryCommander";
    }

    public bool CanApproveRequisition()
    {
        return Role == "ComplexCommander" ||
               Role == "FactoryCommander" ||
               Role == "CentralWarehouseKeeper" ||
               Role == "FactoryWarehouseKeeper" ||
               Role == "Officer";
    }

    public bool CanApproveCommanderReserve()
    {
        return Role == "ComplexCommander" || Role == "FactoryCommander";
    }
}
