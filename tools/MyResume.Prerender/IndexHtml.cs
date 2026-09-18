using System.Text.RegularExpressions;

namespace MyResume.Prerender;

/// <summary>Splices prerendered markup and metadata into the published <c>index.html</c>.</summary>
public static partial class IndexHtml
{
    private const string AppOpen = """<div id="app">""";
    private const string HeadClose = "</head>";

    public static string Inject(string html, string appMarkup, string headMarkup)
    {
        ArgumentNullException.ThrowIfNull(html);
        ArgumentNullException.ThrowIfNull(appMarkup);
        ArgumentNullException.ThrowIfNull(headMarkup);

        var app = AppDiv().Match(html);
        if (!app.Success)
        {
            throw new InvalidOperationException($"index.html has no `{AppOpen}` element to fill.");
        }

        if (!html.Contains(HeadClose, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("index.html has no </head>.");
        }

        var withApp = html[..app.Index] + AppOpen + appMarkup + "</div>" + html[(app.Index + app.Length)..];

        var headAt = withApp.IndexOf(HeadClose, StringComparison.OrdinalIgnoreCase);
        return withApp.Insert(headAt, "    " + headMarkup + Environment.NewLine);
    }

    // The app div holds only a loading placeholder, so it contains no nested <div>.
    [GeneratedRegex("""<div id="app">.*?</div>""", RegexOptions.Singleline)]
    private static partial Regex AppDiv();
}
