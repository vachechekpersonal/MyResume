namespace MyResume.Core.Models;

/// <summary>A period of employment. <see cref="End"/> is null while the role is current.</summary>
public readonly record struct DateRange(DateOnly Start, DateOnly? End)
{
    public bool IsCurrent => End is null;
}
