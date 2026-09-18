using MyResume.Prerender;

if (args.Length != 2 || !Uri.TryCreate(args[1], UriKind.Absolute, out var siteUrl))
{
    Console.Error.WriteLine("Usage: MyResume.Prerender <published wwwroot directory> <absolute site URL>");
    return 2;
}

var wwwroot = args[0];
var indexPath = Path.Combine(wwwroot, "index.html");
var cvSource = new FileCvSource(Path.Combine(wwwroot, "data", "cv.json"));

var cv = await cvSource.LoadAsync();
var app = await PageRenderer.RenderHomeAsync(cvSource, TimeProvider.System, siteUrl);
var head = ShareMetadata.OpenGraphTags(cv.Profile, siteUrl) + Environment.NewLine + "    " + ShareMetadata.JsonLd(cv, siteUrl);

var html = IndexHtml.Inject(await File.ReadAllTextAsync(indexPath), app, head);
await File.WriteAllTextAsync(indexPath, html);

Console.WriteLine($"Prerendered the CV of {cv.Profile.Name} into {indexPath} ({app.Length:N0} characters of markup).");
return 0;
