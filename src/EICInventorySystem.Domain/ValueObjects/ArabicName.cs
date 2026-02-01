namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents an Arabic name
/// </summary>
public class ArabicName : ValueObject
{
    public string Value { get; private set; }

    public ArabicName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Arabic name is required", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Arabic name cannot exceed 100 characters", nameof(value));

        Value = value.Trim();
    }

    public static implicit operator string(ArabicName arabicName) => arabicName.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
