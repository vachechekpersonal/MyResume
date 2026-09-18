using MyResume.Core.Models;

namespace MyResume.Core.Insights;

/// <summary>How much of the career used one skill: the number of roles and the months they cover.</summary>
public sealed record SkillExperience(string Skill, int RoleCount, int Months)
{
    /// <summary>"17 yrs" or "8 mos".</summary>
    public string Summary => DurationFormat.Approximate(Months);
}

public static class SkillExperienceCalculator
{
    /// <summary>
    /// Derives experience per skill from the roles that list it. Months are the union of those roles' periods,
    /// so two overlapping roles that both used a skill count that time once. Career breaks list no skills and
    /// therefore contribute nothing. Keys compare case-insensitively.
    /// </summary>
    public static IReadOnlyDictionary<string, SkillExperience> Compute(IEnumerable<Experience> experiences, DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(experiences);

        var periods = new Dictionary<string, List<(int First, int Last)>>(StringComparer.OrdinalIgnoreCase);
        foreach (var experience in experiences.Where(e => e.Kind == ExperienceKind.Employment))
        {
            var span = (DateRange.MonthIndex(experience.Period.Start), experience.Period.LastMonthIndex(today));
            foreach (var skill in experience.Skills.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!periods.TryGetValue(skill, out var list))
                {
                    periods[skill] = list = [];
                }

                list.Add(span);
            }
        }

        return periods.ToDictionary(
            pair => pair.Key,
            pair => new SkillExperience(pair.Key, pair.Value.Count, Math.Max(1, UnionLength(pair.Value))),
            StringComparer.OrdinalIgnoreCase);
    }

    private static int UnionLength(List<(int First, int Last)> spans)
    {
        spans.Sort((a, b) => a.First.CompareTo(b.First));

        var total = 0;
        var (first, last) = spans[0];
        foreach (var span in spans.Skip(1))
        {
            if (span.First <= last + 1)
            {
                last = Math.Max(last, span.Last);
            }
            else
            {
                total += last - first + 1;
                (first, last) = span;
            }
        }

        return total + last - first + 1;
    }
}
