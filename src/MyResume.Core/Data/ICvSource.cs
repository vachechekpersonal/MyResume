using MyResume.Core.Models;

namespace MyResume.Core.Data;

/// <summary>Supplies the CV document. The Web project implements this over HTTP; tests use a fake.</summary>
public interface ICvSource
{
    Task<Cv> LoadAsync(CancellationToken cancellationToken = default);
}
