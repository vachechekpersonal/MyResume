namespace MyResume.Core.Models;

public enum ExperienceKind
{
    Employment,
    CareerBreak,
}

public sealed record Experience(
    string Company,
    string Role,
    string Location,
    DateRange Period,
    IReadOnlyList<string> Highlights,
    IReadOnlyList<string> Skills,
    ExperienceKind Kind = ExperienceKind.Employment);
