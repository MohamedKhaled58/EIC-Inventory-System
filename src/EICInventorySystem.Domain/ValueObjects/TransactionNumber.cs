namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a transaction number
/// Format: TRN-YYYY-XXXXX (e.g., TRN-2025-00001)
/// </summary>
public class TransactionNumber : ValueObject
{
    public string Value { get; private set; }

    public TransactionNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Transaction number is required", nameof(value));

        if (!value.StartsWith("TRN-"))
            throw new ArgumentException("Transaction number must start with 'TRN-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static TransactionNumber Generate(int year, int sequence)
    {
        return new TransactionNumber($"TRN-{year}-{sequence:D5}");
    }

    public static implicit operator string(TransactionNumber transactionNumber) => transactionNumber.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
