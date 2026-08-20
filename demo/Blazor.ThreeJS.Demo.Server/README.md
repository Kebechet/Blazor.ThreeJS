# The storybook, over a Blazor Server circuit

The same stories as the deployed storybook, from the same `Blazor.ThreeJS.Stories` library, rendered
by a Blazor Server circuit instead of a WebAssembly runtime.

```
dotnet run --project demo/Blazor.ThreeJS.Demo.Server
```

It exists to make the package's Server claim checkable. On WebAssembly an interop call is a function
call in the same address space; over a circuit it is a SignalR message, a callback into C# is a round
trip, and a per-frame batch that is merely correct can still be too chatty to use. Nothing else in
this repository runs there. `tests/Blazor.ThreeJS.E2E/ServerCircuitTests.cs` drives this host and
sweeps every story the demo publishes, so "works on Blazor Server" is a test result rather than an
architecture argument.

Both hosts reference the same stories, so a story that behaves differently here is a hosting-model
difference and nothing else.

## ⚠️ The shell page hits an upstream bug; the stories do not

Opening a story through the storybook shell (`/?path=/story/…`) leaves the preview blank and
terminates the circuit. The exception is BlazingStory's own:

```
Microsoft.JSInterop.JSException: No target window found for selector: .preview-frame iframe
   at BlazingStory.ToolKit.JSInterop.Window.PostMessageAsync(String selector, String message)
   at BlazingStory.Addons.BuiltIns.Panel.Accessibility.AccessibilityPanel.OnAfterRenderAsync(Boolean firstRender)
```

The accessibility addon posts a message to the preview iframe from its first `OnAfterRenderAsync`.
Under WebAssembly the iframe is already in the DOM by then; over a circuit the ordering differs, the
iframe is not there yet, the `JSException` goes unhandled and Blazor tears the circuit down. There is
no public option to disable that addon, and an `ErrorBoundary` cannot help - it would replace the
whole shell rather than the panel.

**A story canvas opened directly is unaffected** - `/iframe.html?viewMode=story&id=<story id>` renders,
animates, and takes pointer input exactly as it does on WebAssembly. That is also how the E2E suite
opens every story, which is why the Server sweep is a real sweep.

Reported against BlazingStory at https://github.com/jsakamoto/BlazingStory. Nothing in
`Kebechet.Blazor.ThreeJS` is involved: the failing call is between two BlazingStory components, before
any three.js code runs.

## What is different from the WebAssembly host, and why

| | why |
|---|---|
| `builder.WebHost.UseStaticWebAssets()` | ⚠️ Explicit, because the framework only does it for itself in Development. Without it a Release `dotnet run` answers every `_content/…` asset with a 500 while the page itself still returns 200 - the storybook boots, looks alive, and has no three.js in it. The E2E suite runs this host in Release. |
| Two host pages | `_Host.cshtml` and `_Iframe.cshtml`, mirroring the WebAssembly host's `index.html` / `iframe.html`. BlazingStory composites a transparent story iframe over its shell and the two load different stylesheets; one page for both would put the preview's own `body { color }` on the shell. |
| `<component type="typeof(HeadOutlet)" />` | BlazingStory links its stylesheet and fonts through `<HeadContent>`. The WebAssembly host registers the outlet as a root component in `Program.cs`; a Razor Page has to render it. Without it the storybook works and has no styling at all. |
| `render-mode="Server"`, not `ServerPrerendered` | A prerender pass runs the component tree with no JavaScript available, so a canvas would be described once into HTML the circuit then replaces - twice the work for a document whose whole content is a WebGL surface. |

This host is **not** deployed. GitHub Pages serves static files, so the published storybook is the
WebAssembly one; this is for running locally and for the E2E suite.
