using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using MyResume.Core.Models;

namespace MyResume.Prerender;

/// <summary>Open Graph / Twitter card tags and a JSON-LD <c>Person</c> block derived from the CV.</summary>
public static class ShareMetadata
{
    // The default encoder escapes '<' and '>', so the block can never terminate its own <script> element.
    private static readonly JsonSerializerOptions JsonLdOptions = new() { Encoder = JavaScriptEncoder.Default };

    /// <summary>"Senior Software Engineer in Cambridge, UK. First sentence of the summary."</summary>
    public static string Description(Profile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        var summary = profile.Summary.Trim();
        var stop = summary.IndexOf(". ", StringComparison.Ordinal);
        var firstSentence = stop < 0 ? summary : summary[..(stop + 1)];
        return $"{profile.Title} in {profile.Location}. {firstSentence}";
    }

    public static string OpenGraphTags(Profile profile, Uri siteUrl)
    {
        ArgumentNullException.ThrowIfNull(profile);
        ArgumentNullException.ThrowIfNull(siteUrl);

        var title = $"{profile.Name} – {profile.Title}";
        var description = Description(profile);
        return string.Join(Environment.NewLine,
            Meta("og:type", "profile"),
            Meta("og:title", title),
            Meta("og:description", description),
            Meta("og:url", siteUrl.ToString()),
            Meta("og:locale", "en_GB"),
            Meta("twitter:card", "summary"),
            Meta("twitter:title", title),
            Meta("twitter:description", description));

        static string Meta(string property, string content) =>
            $"""<meta property="{Attr(property)}" content="{Attr(content)}" />""";
    }

    public static string JsonLd(Cv cv, Uri siteUrl)
    {
        ArgumentNullException.ThrowIfNull(cv);
        ArgumentNullException.ThrowIfNull(siteUrl);

        var person = new JsonObject
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Person",
            ["name"] = cv.Profile.Name,
            ["jobTitle"] = cv.Profile.Title,
            ["description"] = cv.Profile.Summary,
            ["url"] = siteUrl.ToString(),
            ["homeLocation"] = new JsonObject { ["@type"] = "Place", ["name"] = cv.Profile.Location },
            ["sameAs"] = Strings(cv.Profile.Links.Select(link => link.Url)),
            ["knowsAbout"] = Strings(cv.SkillGroups.SelectMany(group => group.Skills)),
            ["knowsLanguage"] = Strings(cv.Languages),
        };

        return $"""<script type="application/ld+json">{person.ToJsonString(JsonLdOptions)}</script>""";
    }

    private static JsonArray Strings(IEnumerable<string> values) =>
        new([.. values.Select(value => (JsonNode?)JsonValue.Create(value))]);

    private static string Attr(string value) => HtmlEncoder.Default.Encode(value);
}
