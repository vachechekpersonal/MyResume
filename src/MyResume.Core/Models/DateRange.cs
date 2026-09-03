using System.Globalization;

namespace MyResume.Core.Models;

/// <summary>A period of employment. <see cref="End"/> is null while the role is current.</summary>
public readonly record struct DateRange(DateOnly Start, DateOnly? End)
{
    private const string Dash = " – ";

    public bool IsCurrent => End is null;

    /// <summary>"Apr 2021 – Present" or "Oct 2019 – Mar 2021".</summary>
    public string Format() =>
        MonthYear(Start) + Dash + (End is { } end ? MonthYear(end) : "Present");

    /// <summary>Whole months, counting both the first and last month, e.g. "1 yr 6 mos".</summary>
    public string Duration(DateOnly today)
    {
        var end = End ?? today;
        var totalMonths = ((end.Year - Start.Year) * 12) + end.Month - Start.Month + 1;
        var years = totalMonths / 12;
        var months = totalMonths % 12;

        return (years, months) switch
        {
            (0, _) => Plural(months, "mo"),
            (_, 0) => Plural(years, "yr"),
            _ => $"{Plural(years, "yr")} {Plural(months, "mo")}",
        };
    }

    private static string MonthYear(DateOnly date) => date.ToString("MMM yyyy", CultureInfo.InvariantCulture);

    private static string Plural(int count, string unit) =>
        string.Create(CultureInfo.InvariantCulture, $"{count} {unit}{(count == 1 ? string.Empty : "s")}");
}
