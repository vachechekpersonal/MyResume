using System.Net;
using System.Text.Json;
using MyResume.Web.Services;

namespace MyResume.Tests.Web;

public sealed class HttpCvSourceTests
{
    [Fact]
    public async Task Loads_cv_from_data_cv_json_relative_to_base_address()
    {
        var handler = new StubHandler(HttpStatusCode.OK, File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "data", "cv.json")));
        var source = new HttpCvSource(new HttpClient(handler) { BaseAddress = new Uri("https://example.test/app/") });

        var cv = await source.LoadAsync(Xunit.TestContext.Current.CancellationToken);

        Assert.Equal("Vache Chek", cv.Profile.Name);
        Assert.Equal("https://example.test/app/data/cv.json", handler.RequestedUri?.ToString());
    }

    [Fact]
    public async Task Http_failure_surfaces_as_HttpRequestException()
    {
        var source = new HttpCvSource(new HttpClient(new StubHandler(HttpStatusCode.NotFound, "")) { BaseAddress = new Uri("https://example.test/") });

        await Assert.ThrowsAsync<HttpRequestException>(() => source.LoadAsync(Xunit.TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Malformed_json_surfaces_as_JsonException()
    {
        var source = new HttpCvSource(new HttpClient(new StubHandler(HttpStatusCode.OK, "{ not json")) { BaseAddress = new Uri("https://example.test/") });

        await Assert.ThrowsAsync<JsonException>(() => source.LoadAsync(Xunit.TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Null_document_surfaces_as_InvalidOperationException()
    {
        var source = new HttpCvSource(new HttpClient(new StubHandler(HttpStatusCode.OK, "null")) { BaseAddress = new Uri("https://example.test/") });

        await Assert.ThrowsAsync<InvalidOperationException>(() => source.LoadAsync(Xunit.TestContext.Current.CancellationToken));
    }

    private sealed class StubHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        public Uri? RequestedUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestedUri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"),
            });
        }
    }
}
