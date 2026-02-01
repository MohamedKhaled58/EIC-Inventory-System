namespace EICInventorySystem.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a supplier credit limit is exceeded
/// </summary>
public class SupplierCreditLimitExceededEvent : DomainEvent
{
    public int SupplierId { get; }
    public string SupplierName { get; }
    public decimal CreditLimit { get; }
    public decimal CurrentBalance { get; }
    public decimal ExceededAmount { get; }
    public DateTime ExceededDate { get; }

    public SupplierCreditLimitExceededEvent(
        int supplierId,
        string supplierName,
        decimal creditLimit,
        decimal currentBalance,
        decimal exceededAmount,
        DateTime exceededDate)
    {
        SupplierId = supplierId;
        SupplierName = supplierName;
        CreditLimit = creditLimit;
        CurrentBalance = currentBalance;
        ExceededAmount = exceededAmount;
        ExceededDate = exceededDate;
    }
}
