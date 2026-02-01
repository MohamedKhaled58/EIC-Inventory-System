namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a warehouse code
/// Format: WH-XXXXX (e.g., WH-00001)
/// </summary>
public class WarehouseCode : ValueObject
{
    public string Value { get; private set; }

    public WarehouseCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Warehouse code is required", nameof(value));

        if (!value.StartsWith("WH-"))
            throw new ArgumentException("Warehouse code must start with 'WH-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static WarehouseCode Generate(int sequence)
    {
        return new WarehouseCode($"WH-{sequence:D5}");
    }

    public static implicit operator string(WarehouseCode warehouseCode) => warehouseCode.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
