namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a return number
/// Format: RET-YYYY-XXXXX (e.g., RET-2025-00001)
/// </summary>
public class ReturnNumber : ValueObject
{
    public string Value { get; private set; }

    public ReturnNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Return number is required", nameof(value));

        if (!value.StartsWith("RET-"))
            throw new ArgumentException("Return number must start with 'RET-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static ReturnNumber Generate(int year, int sequence)
    {
        return new ReturnNumber($"RET-{year}-{sequence:D5}");
    }

    public static implicit operator string(ReturnNumber returnNumber) => returnNumber.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
