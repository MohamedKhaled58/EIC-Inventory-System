namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents an item identifier
/// </summary>
public class ItemId : ValueObject
{
    public Guid Value { get; private set; }

    public ItemId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Item ID cannot be empty", nameof(value));

        Value = value;
    }

    public static ItemId New() => new ItemId(Guid.NewGuid());

    public static implicit operator Guid(ItemId itemId) => itemId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
