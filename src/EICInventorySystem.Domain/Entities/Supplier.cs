namespace EICInventorySystem.Domain.Entities;

/// <summary>
/// Supplier entity for external material suppliers
/// </summary>
public class Supplier : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string NameArabic { get; private set; } = null!;
    public string ContactPerson { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public string AddressArabic { get; private set; } = null!;
    public string TaxNumber { get; private set; } = null!;
    public string CommercialRegister { get; private set; } = null!;
    public string PaymentTerms { get; private set; } = null!;
    public int DeliveryDays { get; private set; }
    public decimal CreditLimit { get; private set; }
    public decimal CurrentBalance { get; private set; }
    public string Rating { get; private set; } = null!; // "A", "B", "C", "D"
    public bool IsActive { get; private set; }

    // Navigation properties
    public ICollection<PurchaseOrder> PurchaseOrders { get; private set; } = new List<PurchaseOrder>();
    public ICollection<Receipt> Receipts { get; private set; } = new List<Receipt>();

    private Supplier() { }

    public Supplier(
        string code,
        string name,
        string nameArabic,
        string contactPerson,
        string phoneNumber,
        string email,
        string address,
        string addressArabic,
        string taxNumber,
        string commercialRegister,
        string paymentTerms,
        int deliveryDays,
        decimal creditLimit,
        string rating,
        int createdBy) : base(createdBy)
    {
        Code = code;
        Name = name;
        NameArabic = nameArabic;
        ContactPerson = contactPerson;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        AddressArabic = addressArabic;
        TaxNumber = taxNumber;
        CommercialRegister = commercialRegister;
        PaymentTerms = paymentTerms;
        DeliveryDays = deliveryDays;
        CreditLimit = creditLimit;
        CurrentBalance = 0;
        Rating = rating;
        IsActive = true;
    }

    public void UpdateDetails(
        string name,
        string nameArabic,
        string contactPerson,
        string phoneNumber,
        string email,
        string address,
        string addressArabic,
        string taxNumber,
        string commercialRegister,
        string paymentTerms,
        int deliveryDays,
        decimal creditLimit,
        string rating,
        int updatedBy)
    {
        Name = name;
        NameArabic = nameArabic;
        ContactPerson = contactPerson;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        AddressArabic = addressArabic;
        TaxNumber = taxNumber;
        CommercialRegister = commercialRegister;
        PaymentTerms = paymentTerms;
        DeliveryDays = deliveryDays;
        CreditLimit = creditLimit;
        Rating = rating;
        Update(updatedBy);
    }

    public void UpdateBalance(decimal amount, int updatedBy)
    {
        CurrentBalance += amount;
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

    public bool IsOverCreditLimit()
    {
        return CurrentBalance > CreditLimit;
    }
}
