namespace MyResume.Core.Models;

public sealed record Cv(
    Profile Profile,
    IReadOnlyList<SkillGroup> SkillGroups,
    IReadOnlyList<Experience> Experiences,
    IReadOnlyList<Qualification> Qualifications,
    IReadOnlyList<string> Languages)
{
    public IEnumerable<string> AllSkills => SkillGroups.SelectMany(group => group.Skills);
}
