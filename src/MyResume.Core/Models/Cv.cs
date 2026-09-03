namespace MyResume.Core.Models;

public sealed record Cv(
    Profile Profile,
    IReadOnlyList<SkillGroup> SkillGroups,
    IReadOnlyList<Experience> Experiences,
    IReadOnlyList<Qualification> Qualifications,
    IReadOnlyList<string> Languages);
