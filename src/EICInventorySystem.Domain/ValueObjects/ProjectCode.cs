namespace EICInventorySystem.Domain.ValueObjects;

/// <summary>
/// Represents a project code
/// Format: PRJ-XXXXX (e.g., PRJ-00001)
/// </summary>
public class ProjectCode : ValueObject
{
    public string Value { get; private set; }

    public ProjectCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Project code is required", nameof(value));

        if (!value.StartsWith("PRJ-"))
            throw new ArgumentException("Project code must start with 'PRJ-'", nameof(value));

        Value = value.Trim().ToUpperInvariant();
    }

    public static ProjectCode Generate(int sequence)
    {
        return new ProjectCode($"PRJ-{sequence:D5}");
    }

    public static implicit operator string(ProjectCode projectCode) => projectCode.Value;

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
