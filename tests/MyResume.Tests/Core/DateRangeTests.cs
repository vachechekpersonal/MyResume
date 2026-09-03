using MyResume.Core.Models;

namespace MyResume.Tests.Core;

public sealed class DateRangeTests
{
    private static readonly DateOnly Today = new(2026, 9, 3);

    [Fact]
    public void Format_shows_month_year_and_Present_for_current_role() =>
        Assert.Equal("Apr 2021 – Present", new DateRange(new DateOnly(2021, 4, 1), null).Format());

    [Fact]
    public void Format_shows_both_ends_for_finished_role() =>
        Assert.Equal("Oct 2019 – Mar 2021", new DateRange(new DateOnly(2019, 10, 1), new DateOnly(2021, 3, 1)).Format());

    [Theory]
    [InlineData(2021, 4, null, null, "5 yrs 6 mos")]   // Apr 2021 → Sep 2026 inclusive = 66 months
    [InlineData(2019, 10, 2021, 3, "1 yr 6 mos")]
    [InlineData(2013, 8, 2013, 12, "5 mos")]
    [InlineData(2025, 9, null, null, "1 yr 1 mo")]
    [InlineData(2026, 9, null, null, "1 mo")]
    [InlineData(2024, 9, 2025, 8, "1 yr")]
    public void Duration_counts_inclusive_months(int sy, int sm, int? ey, int? em, string expected)
    {
        var range = new DateRange(new DateOnly(sy, sm, 1), ey is null ? null : new DateOnly(ey.Value, em!.Value, 1));

        Assert.Equal(expected, range.Duration(Today));
    }
}
