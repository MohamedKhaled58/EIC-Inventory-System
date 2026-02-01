namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a warehouse identifier
/// </summary>
public class WarehouseId : ValueObject
{
    public Guid Value { get; private set; }

    public WarehouseId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Warehouse ID cannot be empty", nameof(value));

        Value = value;
    }

    public static WarehouseId New() => new WarehouseId(Guid.NewGuid());

    public static implicit operator Guid(WarehouseId warehouseId) => warehouseId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
