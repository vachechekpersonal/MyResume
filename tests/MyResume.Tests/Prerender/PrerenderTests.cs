using System.Text.Json.Nodes;
using MyResume.Prerender;

namespace MyResume.Tests.Prerender;

public sealed class PrerenderTests
{
    private static readonly Uri Site = new("https://example.test/cv/");

    private static string RealCvPath => Path.Combine(AppContext.BaseDirectory, "data", "cv.json");

    [Fact]
    public async Task Renders_the_real_cv_to_static_markup_with_every_section()
    {
        var html = await PageRenderer.RenderHomeAsync(new FileCvSource(RealCvPath), FixedTimeProvider.September2026, Site);

        Assert.Matches("<h1[^>]*>Vache Chek</h1>", html);
        foreach (var id in new[] { "about", "skills", "experience", "education", "languages" })
        {
            Assert.Contains($"id=\"{id}\"", html, StringComparison.Ordinal);
        }

        Assert.Contains("Fiscal Technologies", html, StringComparison.Ordinal);
        Assert.DoesNotContain("Loading CV", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Rendered_markup_is_wrapped_in_the_main_layout()
    {
        var html = await PageRenderer.RenderHomeAsync(FakeCvSource.Returning(TestData.Cv()), FixedTimeProvider.September2026, Site);

        // Without the layout's .page wrapper the static page sits unpadded at the left edge until Blazor starts.
        Assert.Matches("""<div class="page"[^>]* b-[a-z0-9]+>""", html);
    }

    [Fact]
    public async Task Rendered_markup_keeps_css_isolation_scopes()
    {
        var html = await PageRenderer.RenderHomeAsync(FakeCvSource.Returning(TestData.Cv()), FixedTimeProvider.September2026, Site);

        // Scope attributes look like b-abc123xyz and are what MyResume.Web.styles.css targets.
        Assert.Matches("""<section id="skills"[^>]* b-[a-z0-9]+>""", html);
    }

    [Fact]
    public void Description_is_title_location_and_first_sentence()
    {
        var profile = TestData.Cv().Profile with { Summary = "First sentence. Second sentence." };

        Assert.Equal("Engineer in Reading, UK. First sentence.", ShareMetadata.Description(profile));
    }

    [Fact]
    public void Open_graph_tags_are_html_encoded()
    {
        var profile = TestData.Cv().Profile with { Name = "Ann \"Quote\" <Tag>" };

        var tags = ShareMetadata.OpenGraphTags(profile, Site);

        Assert.Contains("""property="og:title" content="Ann &quot;Quote&quot; &lt;Tag&gt; &#x2013; Engineer""", tags, StringComparison.Ordinal);
        Assert.Contains("""content="https://example.test/cv/""", tags, StringComparison.Ordinal);
        Assert.Contains("twitter:card", tags, StringComparison.Ordinal);
    }

    [Fact]
    public void Json_ld_is_a_schema_org_person_built_from_the_cv()
    {
        var block = ShareMetadata.JsonLd(TestData.Cv(), Site);

        var json = block["""<script type="application/ld+json">""".Length..^"</script>".Length];
        var person = JsonNode.Parse(json)!.AsObject();
        Assert.Equal("Person", (string?)person["@type"]);
        Assert.Equal("Test Person", (string?)person["name"]);
        Assert.Equal(["https://example.com/in/test"], person["sameAs"]!.AsArray().Select(n => (string?)n));
        Assert.Equal(["C#", "React", "Azure"], person["knowsAbout"]!.AsArray().Select(n => (string?)n));
    }

    [Fact]
    public void Json_ld_cannot_break_out_of_its_script_element()
    {
        var cv = TestData.Cv();
        cv = cv with { Profile = cv.Profile with { Summary = "</script><script>alert(1)</script>" } };

        var block = ShareMetadata.JsonLd(cv, Site);

        Assert.Equal(1, block.Split("</script>").Length - 1);
    }

    [Fact]
    public void Inject_fills_the_app_div_and_adds_head_markup()
    {
        const string index = """
            <html><head><title>x</title></head>
            <body><div id="app"><p class="status">Loading…</p></div><script src="a.js"></script></body></html>
            """;

        var result = IndexHtml.Inject(index, "<main>CV</main>", """<meta property="og:type" content="profile" />""");

        Assert.Contains("""<div id="app"><main>CV</main></div><script src="a.js">""", result, StringComparison.Ordinal);
        Assert.DoesNotContain("Loading…", result, StringComparison.Ordinal);
        Assert.Contains("og:type", result[..result.IndexOf("</head>", StringComparison.Ordinal)], StringComparison.Ordinal);
    }

    [Fact]
    public void Inject_rejects_a_page_without_an_app_div() =>
        Assert.Throws<InvalidOperationException>(() => IndexHtml.Inject("<html><head></head><body></body></html>", "x", "y"));

    [Fact]
    public async Task Real_index_html_accepts_the_prerendered_page()
    {
        var index = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "wwwroot", "index.html"), Xunit.TestContext.Current.CancellationToken);
        var source = new FileCvSource(RealCvPath);
        var cv = await source.LoadAsync(Xunit.TestContext.Current.CancellationToken);
        var app = await PageRenderer.RenderHomeAsync(source, FixedTimeProvider.September2026, Site);

        var html = IndexHtml.Inject(index, app, ShareMetadata.JsonLd(cv, Site));

        Assert.Matches("<h1[^>]*>Vache Chek</h1>", html);
        Assert.Contains("application/ld+json", html, StringComparison.Ordinal);
    }
}
