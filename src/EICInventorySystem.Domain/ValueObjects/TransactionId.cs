namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a transaction identifier
/// </summary>
public class TransactionId : ValueObject
{
    public Guid Value { get; private set; }

    public TransactionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Transaction ID cannot be empty", nameof(value));

        Value = value;
    }

    public static TransactionId New() => new TransactionId(Guid.NewGuid());

    public static implicit operator Guid(TransactionId transactionId) => transactionId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
