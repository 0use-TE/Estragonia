# EstragoniaApp

Godot 4 + Avalonia starter from **[Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/)**.

- Library: [Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/)
- Docs / source: [0use-TE/Estragonia](https://github.com/0use-TE/Estragonia)

## Open in Godot

1. Install [.NET 10 SDK](https://dotnet.microsoft.com/download) and **Godot 4.7+ (.NET)**.
2. Run `dotnet restore` in this folder.
3. Open **`project.godot`** here with Godot (not the `.godot/` cache directory).

Autoload `AvaloniaLoader` and the default `UserInterface` (`UiHost`) are already configured.

**`AvaloniaControl.cs` and `UiHost.cs` must stay in this Godot project.**  
They are not shipped as Godot scripts inside the `Ouse.Estragonia` NuGet package (Godot cannot hot-reload node types from external assemblies).

## Layout

| Path | Role |
|------|------|
| `project.godot` | Open this in Godot |
| `Views/` / `ViewModels/` | Avalonia MVVM UI |
| `AvaloniaControl.cs` | Godot host — renders Avalonia (required, in-project) |
| `UiHost.cs` | Godot host — focus + `CreateRoot()` (required, in-project) |
| `AvaloniaLoader.cs` | Autoload — `UseGodot()` once |
| `UserInterface.cs` | Your `UiHost` — implement `CreateRoot()` |
| `Directory.Packages.props` | NuGet versions (solution-level) |
| `global.json` | .NET SDK pin |

Edit `Views/MainView.axaml` and `ViewModels/MainViewModel.cs` to build your UI.

## Visual Studio / previewer

- Install the **Avalonia for Visual Studio** extension.
- `Designer.cs` provides `Main` + `BuildAvaloniaApp` (Debug `OutputType=Exe`) so AXAML preview works.
  (Keep that file free of C# `#if …` directives — `dotnet new` would strip them and leave an empty file.)
- Runtime still runs inside Godot via `project.godot` — do not start the C# project as a normal console app.
