namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Factory entity representing a manufacturing facility
/// </summary>
public class Factory : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NameArabic { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string DescriptionArabic { get; private set; } = null!;
    public string Location { get; private set; } = null!;
    public string LocationArabic { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int? CommanderId { get; private set; }

    // Navigation properties
    public User? Commander { get; private set; }
    public ICollection<Warehouse> Warehouses { get; private set; } = new List<Warehouse>();
    public ICollection<Department> Departments { get; private set; } = new List<Department>();
    public ICollection<Project> Projects { get; private set; } = new List<Project>();
    public ICollection<User> Users { get; private set; } = new List<User>();

    private Factory() { }

    public Factory(
        string code,
        string name,
        string nameArabic,
        string description,
        string descriptionArabic,
        string location,
        string locationArabic,
        int createdBy,
        int? commanderId = null) : base(createdBy)
    {
        Code = code;
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        Location = location;
        LocationArabic = locationArabic;
        IsActive = true;
        CommanderId = commanderId;
    }

    public void UpdateDetails(
        string name,
        string nameArabic,
        string description,
        string descriptionArabic,
        string location,
        string locationArabic,
        int updatedBy)
    {
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        Location = location;
        LocationArabic = locationArabic;
        Update(updatedBy);
    }

    public void AssignCommander(int commanderId, int updatedBy)
    {
        CommanderId = commanderId;
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
}
