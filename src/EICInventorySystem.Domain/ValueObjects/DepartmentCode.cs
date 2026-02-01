namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a department code
/// Format: DEPT-XXXXX (e.g., DEPT-00001)
/// </summary>
public class DepartmentCode : ValueObject
{
    public string Value { get; private set; }

    public DepartmentCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Department code is required", nameof(value));

        if (!value.StartsWith("DEPT-"))
            throw new ArgumentException("Department code must start with 'DEPT-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static DepartmentCode Generate(int sequence)
    {
        return new DepartmentCode($"DEPT-{sequence:D5}");
    }

    public static implicit operator string(DepartmentCode departmentCode) => departmentCode.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
