namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a supplier code
/// Format: SUP-XXXXX (e.g., SUP-00001)
/// </summary>
public class SupplierCode : ValueObject
{
    public string Value { get; private set; }

    public SupplierCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Supplier code is required", nameof(value));

        if (!value.StartsWith("SUP-"))
            throw new ArgumentException("Supplier code must start with 'SUP-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static SupplierCode Generate(int sequence)
    {
        return new SupplierCode($"SUP-{sequence:D5}");
    }

    public static implicit operator string(SupplierCode supplierCode) => supplierCode.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
