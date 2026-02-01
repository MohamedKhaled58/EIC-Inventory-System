namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents an inventory adjustment number
/// Format: ADJ-YYYY-XXXXX (e.g., ADJ-2025-00001)
/// </summary>
public class AdjustmentNumber : ValueObject
{
    public string Value { get; private set; }

    public AdjustmentNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Adjustment number is required", nameof(value));

        if (!value.StartsWith("ADJ-"))
            throw new ArgumentException("Adjustment number must start with 'ADJ-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static AdjustmentNumber Generate(int year, int sequence)
    {
        return new AdjustmentNumber($"ADJ-{year}-{sequence:D5}");
    }

    public static implicit operator string(AdjustmentNumber adjustmentNumber) => adjustmentNumber.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
