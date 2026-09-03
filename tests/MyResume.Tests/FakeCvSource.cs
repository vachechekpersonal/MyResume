using MyResume.Core.Data;
using MyResume.Core.Models;

namespace MyResume.Tests;

internal sealed class FakeCvSource : ICvSource
{
    private readonly Task<Cv> _result;

    private FakeCvSource(Task<Cv> result) => _result = result;

    public static FakeCvSource Returning(Cv cv) => new(Task.FromResult(cv));

    public static FakeCvSource Failing(Exception exception) => new(Task.FromException<Cv>(exception));

    /// <summary>Never completes – used to observe the loading state.</summary>
    public static FakeCvSource Pending() => new(new TaskCompletionSource<Cv>().Task);

    public Task<Cv> LoadAsync(CancellationToken cancellationToken = default) => _result;
}
