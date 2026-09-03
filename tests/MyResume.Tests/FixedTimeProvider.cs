namespace MyResume.Tests;

internal sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public static readonly FixedTimeProvider September2026 = new(new DateTimeOffset(2026, 9, 3, 12, 0, 0, TimeSpan.Zero));

    public override DateTimeOffset GetUtcNow() => now;
}
