namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a consumption number
/// Format: CON-YYYY-XXXXX (e.g., CON-2025-00001)
/// </summary>
public class ConsumptionNumber : ValueObject
{
    public string Value { get; private set; }

    public ConsumptionNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Consumption number is required", nameof(value));

        if (!value.StartsWith("CON-"))
            throw new ArgumentException("Consumption number must start with 'CON-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static ConsumptionNumber Generate(int year, int sequence)
    {
        return new ConsumptionNumber($"CON-{year}-{sequence:D5}");
    }

    public static implicit operator string(ConsumptionNumber consumptionNumber) => consumptionNumber.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
