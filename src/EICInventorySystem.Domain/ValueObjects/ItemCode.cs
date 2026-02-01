namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a unique item code
/// </summary>
public class ItemCode : ValueObject
{
    public string Value { get; private set; }

    public ItemCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Item code is required", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Item code cannot exceed 50 characters", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(ItemCode itemCode) => itemCode.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
