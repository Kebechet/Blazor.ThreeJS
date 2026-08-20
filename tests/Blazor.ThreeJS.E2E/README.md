# Real browser suite

Everything else in this repository is verified either in C# against a mocked interop module, or in
Node against the vendored three.js. Neither runs a WebGL context. This suite does: it starts a demo
storybook, drives it with Playwright, and asserts on what the browser actually did.

It runs the same stories against **both hosting models**, from two collections that each own their own
demo process and browser:

| collection | host | what only it can show |
|---|---|---|
| `DemoCollectionDefinition` | `demo/Blazor.ThreeJS.Demo.Wasm` (WebAssembly) | the deployed storybook, including the shell page and its sub-path assets |
| `ServerDemoCollectionDefinition` | `demo/Blazor.ThreeJS.Demo.Server` (Blazor Server) | every op as a SignalR message, every pointer callback as a round trip |

Both reference `demo/Blazor.ThreeJS.Stories`, so the stories are the same files and a story that behaves
differently in one of them is a hosting-model difference and nothing else. See
`demo/Blazor.ThreeJS.Demo.Server/README.md` for what the Server host does differently, and for the one
upstream BlazingStory bug that affects its shell page but not its stories.

Run it from the repository root:

```powershell
dotnet test tests/Blazor.ThreeJS.E2E/Blazor.ThreeJS.E2E.csproj -c Release
```

It is not part of `src/Blazor.ThreeJS.slnx`, so `dotnet test` on the solution does not pick it up.

## No screenshot baselines

There are no committed reference images and no pixel-exact comparison. GPU output differs by driver
and by vendor, so a baseline that passes here would fail on a CI runner for reasons nobody can act
on, and a suite people learn to ignore is worse than no suite.

What the tests do instead:

- **Structural facts** the browser is the only witness to — a WebGL context exists, the drawing
  buffer matches the CSS box times the device pixel ratio, every module chunk was served, the console
  is clean.
- **Perceptual comparison within one session, with a tolerance.** Two samples are always taken from
  the same browser seconds apart — before and after a click, one frame and the next — and compared as
  a mean absolute per-channel difference. A difference can then only come from the thing the test
  did.
- **Coverage rather than colour** for "did it render at all". The canvas is created with
  `alpha: true` and never cleared to a colour, so any pixel with opacity is one the scene drew.

Rendering is pinned to SwiftShader, ANGLE's software rasteriser, through the Chrome launch flags in
`DemoFixture`. The flag names are not trusted:
`Rendering_BrowserLaunched_ReportsSwiftShaderAsTheRenderer` reads the renderer string back out of a
live context and fails if Chrome quietly ignored them.

## Reading pixels back

The renderer runs without `preserveDrawingBuffer`, so the buffer is cleared as each frame is
composited. `StoryPage.CaptureScript` reads it inside a nested `requestAnimationFrame`, which lands
after the interop module's own render callback for that frame and before the frame is thrown away.

## The demo process

`DemoServer` starts `dotnet run` on one of the two demo hosts, on a port the OS picks, and stops the
whole process tree in `DisposeAsync` — `dotnet run` is a launcher, so killing it alone would leave the
app holding the port. A start that fails partway tears down what it managed to create before
rethrowing. Which host it runs is the project path the fixture hands it, `DemoServer.WebAssemblyHost`
or `DemoServer.ServerHost`.

## Browser

Playwright drives the installed Google Chrome through its `chrome` channel, which is present on this
machine and on the GitHub `ubuntu-latest` image, so no browser download is needed.
