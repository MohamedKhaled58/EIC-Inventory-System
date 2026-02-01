namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a receipt number for material receipts
/// Format: RCP-YYYY-XXXXX (e.g., RCP-2025-00001)
/// </summary>
public class ReceiptNumber : ValueObject
{
    public string Value { get; private set; }

    public ReceiptNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Receipt number is required", nameof(value));

        if (!value.StartsWith("RCP-"))
            throw new ArgumentException("Receipt number must start with 'RCP-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static ReceiptNumber Generate(int year, int sequence)
    {
        return new ReceiptNumber($"RCP-{year}-{sequence:D5}");
    }

    public static implicit operator string(ReceiptNumber receiptNumber) => receiptNumber.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
