namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Warehouse entity (Central or Factory)
/// </summary>
public class Warehouse : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NameArabic { get; private set; } = null!;
    public string Type { get; private set; } = null!; // "Central", "Factory"
    public string Location { get; private set; } = null!;
    public string LocationArabic { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int? FactoryId { get; private set; }
    public int? KeeperId { get; private set; }

    // Navigation properties
    public Factory? Factory { get; private set; }
    public User? Keeper { get; private set; }
    public ICollection<InventoryRecord> InventoryRecords { get; private set; } = new List<InventoryRecord>();
    public ICollection<Transfer> OutgoingTransfers { get; private set; } = new List<Transfer>();
    public ICollection<Transfer> IncomingTransfers { get; private set; } = new List<Transfer>();
    public ICollection<Receipt> Receipts { get; private set; } = new List<Receipt>();
    public ICollection<User> AssignedUsers { get; private set; } = new List<User>();

    private Warehouse() { }

    public Warehouse(
        string code,
        string name,
        string nameArabic,
        string type,
        string location,
        string locationArabic,
        int createdBy,
        int? factoryId = null,
        int? keeperId = null) : base(createdBy)
    {
        Code = code;
        Name = name;
        NameArabic = nameArabic;
        Type = type;
        Location = location;
        LocationArabic = locationArabic;
        IsActive = true;
        FactoryId = factoryId;
        KeeperId = keeperId;
    }

    public void UpdateDetails(
        string name,
        string nameArabic,
        string location,
        string locationArabic,
        int updatedBy)
    {
        Name = name;
        NameArabic = nameArabic;
        Location = location;
        LocationArabic = locationArabic;
        Update(updatedBy);
    }

    public void AssignKeeper(int keeperId, int updatedBy)
    {
        KeeperId = keeperId;
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

    public bool IsCentralWarehouse()
    {
        return Type == "Central";
    }

    public bool IsFactoryWarehouse()
    {
        return Type == "Factory";
    }
}
