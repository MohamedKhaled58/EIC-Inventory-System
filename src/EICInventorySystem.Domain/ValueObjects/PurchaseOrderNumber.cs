namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a purchase order number
/// Format: PO-YYYY-XXXXX (e.g., PO-2025-00001)
/// </summary>
public class PurchaseOrderNumber : ValueObject
{
    public string Value { get; private set; }

    public PurchaseOrderNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Purchase order number is required", nameof(value));

        if (!value.StartsWith("PO-"))
            throw new ArgumentException("Purchase order number must start with 'PO-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static PurchaseOrderNumber Generate(int year, int sequence)
    {
        return new PurchaseOrderNumber($"PO-{year}-{sequence:D5}");
    }

    public static implicit operator string(PurchaseOrderNumber poNumber) => poNumber.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
