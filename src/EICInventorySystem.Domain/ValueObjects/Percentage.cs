namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a percentage value (0-100)
/// </summary>
public class Percentage : ValueObject
{
    public decimal Value { get; private set; }

    public Percentage(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentException("Percentage must be between 0 and 100", nameof(value));

        Value = value;
    }

    public static Percentage Zero => new Percentage(0);
    public static Percentage OneHundred => new Percentage(100);

    public static Percentage FromDecimal(decimal value) => new Percentage(value);

    public decimal AsDecimal() => Value;

    public decimal ApplyTo(decimal amount) => amount * (Value / 100m);

    public bool IsGreaterThan(Percentage other) => Value > other.Value;
    public bool IsLessThan(Percentage other) => Value < other.Value;
    public bool IsGreaterThanOrEqual(Percentage other) => Value >= other.Value;
    public bool IsLessThanOrEqual(Percentage other) => Value <= other.Value;

    public override string ToString() => $"{Value:N2}%";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
