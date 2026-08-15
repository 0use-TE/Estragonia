# Release notes

## 1.0.1

- **Breaking for package consumers:** `AvaloniaControl` / `UiHost` are no longer Godot node types inside `Ouse.Estragonia`. Copy them from the template/sample into your Godot project (or use `dotnet new estragonia`). Logic lives in `AvaloniaControlEngine` in the NuGet package.
- Avoids Godot ScriptTypeBiMap duplicate-key errors when hot-reloading with host types in an external assembly.
- Custom cursor support: Avalonia `CreateCursor(Bitmap)` → Godot `Input.SetCustomMouseCursor`.
- Template references `Ouse.Estragonia` via NuGet only (`Version="1.*"` float on 1.x).
- HelloWorld sample: left scene list (controls, cursor, HUD, pass-through, binding).
- Editor plugins: process-wide `AvaloniaEditorRuntime` / `GodotAvalonia.EnsureStarted`; HelloWorld ships two sample addons (left dock + bottom panel).
- C# hot-reload: `GodotAvalonia.PrepareForUnload` on GodotTools `BuildStarted` and ALC `Unloading` so Avalonia no longer pins the collectible ALC after every code change.

## 1.0.0

Initial release of this maintained fork:

- Avalonia 12 / Godot 4.7 / .NET 10
- Central Package Management (`Directory.Packages.props`)
- Input pass-through, Avalonia 12 dispatcher / asset-loader fixes
- DocFX documentation + GitHub Pages workflow
- `dotnet new estragonia` project template
- NuGet: [Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/) + [Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/)
- Source: [0use-TE/Estragonia](https://github.com/0use-TE/Estragonia)

Based on [MrJul/Estragonia](https://github.com/MrJul/Estragonia) (MIT). Upstream package id remains `JLeb.Estragonia`.
