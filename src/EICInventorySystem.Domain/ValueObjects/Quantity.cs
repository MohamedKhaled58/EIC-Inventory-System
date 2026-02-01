namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a quantity with unit of measure
/// </summary>
public class Quantity : ValueObject
{
    public decimal Amount { get; private set; }
    public string Unit { get; private set; }

    public Quantity(decimal amount, string unit)
    {
        if (amount < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(amount));
        
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be empty", nameof(unit));

        Amount = amount;
        Unit = unit.ToUpperInvariant();
    }

    public static Quantity Zero(string unit) => new Quantity(0, unit);

    public Quantity Add(Quantity other)
    {
        if (Unit != other.Unit)
            throw new InvalidOperationException("Cannot add quantities with different units");
        
        return new Quantity(Amount + other.Amount, Unit);
    }

    public Quantity Subtract(Quantity other)
    {
        if (Unit != other.Unit)
            throw new InvalidOperationException("Cannot subtract quantities with different units");
        
        if (Amount < other.Amount)
            throw new InvalidOperationException("Resulting quantity would be negative");
        
        return new Quantity(Amount - other.Amount, Unit);
    }

    public bool IsGreaterThan(Quantity other)
    {
        if (Unit != other.Unit)
            throw new InvalidOperationException("Cannot compare quantities with different units");
        
        return Amount > other.Amount;
    }

    public bool IsLessThan(Quantity other)
    {
        if (Unit != other.Unit)
            throw new InvalidOperationException("Cannot compare quantities with different units");
        
        return Amount < other.Amount;
    }

    public bool IsGreaterThanOrEqual(Quantity other)
    {
        if (Unit != other.Unit)
            throw new InvalidOperationException("Cannot compare quantities with different units");
        
        return Amount >= other.Amount;
    }

    public bool IsLessThanOrEqual(Quantity other)
    {
        if (Unit != other.Unit)
            throw new InvalidOperationException("Cannot compare quantities with different units");
        
        return Amount <= other.Amount;
    }

    public override string ToString()
    {
        return $"{Amount:N3} {Unit}";
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Unit;
    }
}
