namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Department entity within a factory
/// </summary>
public class Department : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NameArabic { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string DescriptionArabic { get; private set; } = null!;
    public int FactoryId { get; private set; }
    public int? HeadId { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public Factory Factory { get; private set; } = null!;
    public User? Head { get; private set; }
    public ICollection<User> Users { get; private set; } = new List<User>();
    public ICollection<Requisition> Requisitions { get; private set; } = new List<Requisition>();
    public ICollection<Item> Items { get; private set; } = new List<Item>();
    public ICollection<ItemTransactionHistory> TransactionHistory { get; private set; } = new List<ItemTransactionHistory>();
    public ICollection<Worker> Workers { get; private set; } = new List<Worker>();
    public ICollection<OperationalCustody> Custodies { get; private set; } = new List<OperationalCustody>();

    private Department() { }

    public Department(
        string code,
        string name,
        string nameArabic,
        string description,
        string descriptionArabic,
        int factoryId,
        int createdBy,
        int? headId = null) : base(createdBy)
    {
        Code = code;
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        FactoryId = factoryId;
        HeadId = headId;
        IsActive = true;
    }

    public void UpdateDetails(
        string name,
        string nameArabic,
        string description,
        string descriptionArabic,
        int updatedBy)
    {
        Name = name;
        NameArabic = nameArabic;
        Description = description;
        DescriptionArabic = descriptionArabic;
        Update(updatedBy);
    }

    public void AssignHead(int headId, int updatedBy)
    {
        HeadId = headId;
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
