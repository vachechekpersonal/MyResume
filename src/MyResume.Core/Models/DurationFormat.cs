using System.Globalization;

namespace MyResume.Core.Models;

/// <summary>Human-readable spans of whole months.</summary>
public static class DurationFormat
{
    /// <summary>Exact: "1 yr 6 mos", "5 mos", "2 yrs".</summary>
    public static string Exact(int totalMonths)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(totalMonths);

        var years = totalMonths / 12;
        var months = totalMonths % 12;

        return (years, months) switch
        {
            (0, _) => Plural(months, "mo"),
            (_, 0) => Plural(years, "yr"),
            _ => $"{Plural(years, "yr")} {Plural(months, "mo")}",
        };
    }

    /// <summary>
    /// Compact, for chips: under a year shows months ("8 mos"); otherwise years rounded to the nearest whole
    /// year ("2 yrs" for 17 to 29 months). Rounding halves up, so 18 months reads "2 yrs".
    /// </summary>
    public static string Approximate(int totalMonths)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(totalMonths);

        if (totalMonths < 12)
        {
            return Plural(totalMonths, "mo");
        }

        var years = (int)Math.Round(totalMonths / 12d, MidpointRounding.AwayFromZero);
        return Plural(years, "yr");
    }

    private static string Plural(int count, string unit) =>
        string.Create(CultureInfo.InvariantCulture, $"{count} {unit}{(count == 1 ? string.Empty : "s")}");
}
