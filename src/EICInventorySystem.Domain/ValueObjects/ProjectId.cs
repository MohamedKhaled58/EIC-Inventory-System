namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a project identifier
/// </summary>
public class ProjectId : ValueObject
{
    public Guid Value { get; private set; }

    public ProjectId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty", nameof(value));

        Value = value;
    }

    public static ProjectId New() => new ProjectId(Guid.NewGuid());

    public static implicit operator Guid(ProjectId projectId) => projectId.Value;

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
