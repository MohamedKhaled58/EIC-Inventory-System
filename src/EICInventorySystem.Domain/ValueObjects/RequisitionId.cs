namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a requisition identifier
/// </summary>
public class RequisitionId : ValueObject
{
    public Guid Value { get; private set; }

    public RequisitionId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Requisition ID cannot be empty", nameof(value));

        Value = value;
    }

    public static RequisitionId New() => new RequisitionId(Guid.NewGuid());

    public static implicit operator Guid(RequisitionId requisitionId) => requisitionId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
