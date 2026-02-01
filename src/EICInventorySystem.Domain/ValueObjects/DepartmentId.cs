namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a department identifier
/// </summary>
public class DepartmentId : ValueObject
{
    public Guid Value { get; private set; }

    public DepartmentId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Department ID cannot be empty", nameof(value));

        Value = value;
    }

    public static DepartmentId New() => new DepartmentId(Guid.NewGuid());

    public static implicit operator Guid(DepartmentId departmentId) => departmentId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
