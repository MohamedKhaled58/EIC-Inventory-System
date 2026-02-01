namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a factory code
/// Format: FC-XXXXX (e.g., FC-00001)
/// </summary>
public class FactoryCode : ValueObject
{
    public string Value { get; private set; }

    public FactoryCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Factory code is required", nameof(value));

        if (!value.StartsWith("FC-"))
            throw new ArgumentException("Factory code must start with 'FC-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static FactoryCode Generate(int sequence)
    {
        return new FactoryCode($"FC-{sequence:D5}");
    }

    public static implicit operator string(FactoryCode factoryCode) => factoryCode.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
