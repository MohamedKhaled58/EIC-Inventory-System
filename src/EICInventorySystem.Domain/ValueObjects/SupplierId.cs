namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a supplier identifier
/// </summary>
public class SupplierId : ValueObject
{
    public Guid Value { get; private set; }

    public SupplierId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Supplier ID cannot be empty", nameof(value));

        Value = value;
    }

    public static SupplierId New() => new SupplierId(Guid.NewGuid());

    public static implicit operator Guid(SupplierId supplierId) => supplierId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
