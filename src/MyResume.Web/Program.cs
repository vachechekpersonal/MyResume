using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyResume.Core.Data;
using MyResume.Core.Filtering;
using MyResume.Web;
using MyResume.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ICvSource, HttpCvSource>();
builder.Services.AddScoped<SkillSelection>();
builder.Services.AddSingleton(TimeProvider.System);

await builder.Build().RunAsync().ConfigureAwait(false);
