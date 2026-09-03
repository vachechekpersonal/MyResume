using MyResume.Core.Models;

namespace MyResume.Core.Filtering;

/// <summary>Pure matching rule: which experiences should be highlighted for a skill selection.</summary>
public static class ExperienceFilter
{
    /// <summary>
    /// True when nothing is selected, or when <paramref name="experience"/> used at least one selected skill.
    /// Career breaks never match an active filter. String comparison follows the comparer of
    /// <paramref name="selectedSkills"/>; <see cref="SkillSelection"/> supplies a case-insensitive set.
    /// </summary>
    public static bool Matches(Experience experience, IReadOnlySet<string> selectedSkills)
    {
        ArgumentNullException.ThrowIfNull(experience);
        ArgumentNullException.ThrowIfNull(selectedSkills);

        if (selectedSkills.Count == 0)
        {
            return true;
        }

        return experience.Kind == ExperienceKind.Employment
            && experience.Skills.Any(selectedSkills.Contains);
    }
}
