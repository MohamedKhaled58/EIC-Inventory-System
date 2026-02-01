namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a user identifier
/// </summary>
public class UserId : ValueObject
{
    public Guid Value { get; private set; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(value));

        Value = value;
    }

    public static UserId New() => new UserId(Guid.NewGuid());

    public static implicit operator Guid(UserId userId) => userId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
