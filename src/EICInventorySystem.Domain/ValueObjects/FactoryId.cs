namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a factory identifier
/// </summary>
public class FactoryId : ValueObject
{
    public Guid Value { get; private set; }

    public FactoryId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Factory ID cannot be empty", nameof(value));

        Value = value;
    }

    public static FactoryId New() => new FactoryId(Guid.NewGuid());

    public static implicit operator Guid(FactoryId factoryId) => factoryId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
