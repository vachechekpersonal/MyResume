using MyResume.Core.Data;
using MyResume.Core.Models;

namespace MyResume.Tests;

internal sealed class FakeCvSource : ICvSource
{
    private readonly TaskCompletionSource<Cv> _result = new();

    private FakeCvSource()
    {
    }

    public static FakeCvSource Returning(Cv cv)
    {
        var source = new FakeCvSource();
        source._result.SetResult(cv);
        return source;
    }

    public static FakeCvSource Failing(Exception exception)
    {
        var source = new FakeCvSource();
        source._result.SetException(exception);
        return source;
    }

    /// <summary>Never completes until <see cref="Complete"/> is called – used to observe the loading state.</summary>
    public static FakeCvSource Pending() => new();

    public void Complete(Cv cv) => _result.SetResult(cv);

    public Task<Cv> LoadAsync(CancellationToken cancellationToken = default) => _result.Task;
}
