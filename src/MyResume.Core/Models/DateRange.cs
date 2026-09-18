using System.Globalization;
using System.Text.Json.Serialization;

namespace MyResume.Core.Models;

/// <summary>A period of employment. <see cref="End"/> is null while the role is current.</summary>
public readonly record struct DateRange
{
    private const string Dash = " – ";

    [JsonConstructor]
    public DateRange(DateOnly start, DateOnly? end)
    {
        if (end is { } finish && finish < start)
        {
            throw new ArgumentOutOfRangeException(nameof(end), end, "End must not be before Start.");
        }

        Start = start;
        End = end;
    }

    public DateOnly Start { get; }

    public DateOnly? End { get; }

    public bool IsCurrent => End is null;

    /// <summary>"Apr 2021 – Present" or "Oct 2019 – Mar 2021".</summary>
    public string Format() =>
        MonthYear(Start) + Dash + (End is { } end ? MonthYear(end) : "Present");

    /// <summary>Whole months, counting both the first and last month, e.g. "1 yr 6 mos". Never less than one month.</summary>
    public string Duration(DateOnly today) => DurationFormat.Exact(MonthCount(today));

    /// <summary>Inclusive number of calendar months covered, clamped to at least one. Open ranges end at <paramref name="today"/>.</summary>
    public int MonthCount(DateOnly today) => Math.Max(1, LastMonthIndex(today) - MonthIndex(Start) + 1);

    /// <summary>Months since year 0, so ranges can be compared and merged arithmetically.</summary>
    internal static int MonthIndex(DateOnly date) => (date.Year * 12) + date.Month - 1;

    internal int LastMonthIndex(DateOnly today) => MonthIndex(End ?? today);

    private static string MonthYear(DateOnly date) => date.ToString("MMM yyyy", CultureInfo.InvariantCulture);
}
