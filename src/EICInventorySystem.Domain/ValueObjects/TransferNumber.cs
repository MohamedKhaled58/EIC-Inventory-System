namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a transfer number
/// Format: TRF-YYYY-XXXXX (e.g., TRF-2025-00001)
/// </summary>
public class TransferNumber : ValueObject
{
    public string Value { get; private set; }

    public TransferNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Transfer number is required", nameof(value));

        if (!value.StartsWith("TRF-"))
            throw new ArgumentException("Transfer number must start with 'TRF-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static TransferNumber Generate(int year, int sequence)
    {
        return new TransferNumber($"TRF-{year}-{sequence:D5}");
    }

    public static implicit operator string(TransferNumber transferNumber) => transferNumber.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
