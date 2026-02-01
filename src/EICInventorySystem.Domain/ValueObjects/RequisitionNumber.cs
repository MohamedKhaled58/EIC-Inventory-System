namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a requisition number
/// Format: REQ-YYYY-XXXXX (e.g., REQ-2025-00001)
/// </summary>
public class RequisitionNumber : ValueObject
{
    public string Value { get; private set; }

    public RequisitionNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Requisition number is required", nameof(value));

        if (!value.StartsWith("REQ-"))
            throw new ArgumentException("Requisition number must start with 'REQ-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static RequisitionNumber Generate(int year, int sequence)
    {
        return new RequisitionNumber($"REQ-{year}-{sequence:D5}");
    }

    public static implicit operator string(RequisitionNumber requisitionNumber) => requisitionNumber.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
