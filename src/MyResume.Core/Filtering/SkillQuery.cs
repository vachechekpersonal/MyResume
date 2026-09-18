namespace MyResume.Core.Filtering;

/// <summary>
/// The <c>skills</c> query-string value that makes a skill selection shareable, e.g. <c>?skills=Azure,C%23</c>.
/// Parsing is lenient (blank entries and duplicates are dropped); formatting is canonical (sorted, case-insensitive)
/// so the same selection always produces the same URL.
/// </summary>
public static class SkillQuery
{
    public const string ParameterName = "skills";

    private const char Separator = ',';

    public static IReadOnlyList<string> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value
            .Split(Separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <summary>Null when nothing is selected, so the parameter can be removed from the URL entirely.</summary>
    public static string? Format(IEnumerable<string> skills)
    {
        ArgumentNullException.ThrowIfNull(skills);

        var ordered = skills.Order(StringComparer.OrdinalIgnoreCase).ToArray();
        return ordered.Length == 0 ? null : string.Join(Separator, ordered);
    }
}
