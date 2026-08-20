// The same storybook as the WebAssembly host, over a Blazor Server circuit.
//
// It exists to be a demonstration rather than a second product. Everything the package claims about
// Server support - that a frame costs one interop call however many properties it changed, that the
// batch survives a round trip over SignalR, that pointer picking calls back into C# across the
// circuit - is a claim about a place no other test in this repository runs. The stories are the same
// files, referenced from `Blazor.ThreeJS.Stories`, so nothing here is written twice and nothing here
// can drift from what the deployed storybook shows.
using Blazor.ThreeJS.Demo.Server;

var builder = WebApplication.CreateBuilder(args);

// ⚠️ Explicit, because the framework only does this for itself in Development. Without it a Release
// `dotnet run` routes `_content/…` to an endpoint that then looks for the file under this project's
// own `wwwroot`, where a referenced library's assets have never been copied - so every asset answers
// 500 rather than 404, and the storybook loads with no three.js at all. The E2E suite runs this host
// in Release, so Development-only behaviour would be behaviour the tests never see. A no-op after
// `dotnet publish`, which copies the same files into `wwwroot` for real.
builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

// MapStaticAssets rather than UseStaticFiles, because the assets that matter here come from
// referenced libraries - `_content/Kebechet.Blazor.ThreeJS/three-interop.js` and the stories' own
// models - and those are only on disk beside the app in Development. MapStaticAssets serves them from
// the manifest the build writes, which is what makes a Release run see the same files a developer
// does. ⚠️ The E2E suite runs this host in Release with no launch profile, so it is the Release
// behaviour that has to be right.
app.UseRouting();
app.MapStaticAssets();
app.MapBlazorHub();

// Two host pages rather than one, mirroring the WebAssembly host's `index.html` / `iframe.html`
// split. BlazingStory renders a story inside a transparent iframe and composites it over its own
// shell, and the two documents load different stylesheets - so serving both from one page would put
// the preview's own `body { color }` on the shell as well.
app.MapFallbackToPage("/iframe.html", "/_Iframe");
app.MapFallbackToPage("/_Host");

app.Run();

/// <summary>
/// Names the entry-point assembly for the Blazor root component, which the host pages reference.
/// </summary>
public partial class Program;
