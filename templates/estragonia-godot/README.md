# EstragoniaApp

Godot 4 + Avalonia starter from **[Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/)**.

- Library: [Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/)
- Docs / source: [0use-TE/Estragonia](https://github.com/0use-TE/Estragonia)

## Open in Godot

1. Install [.NET 10 SDK](https://dotnet.microsoft.com/download) and **Godot 4.7.2+ (.NET)**.
2. Run `dotnet restore` in this folder (the solution root).
3. Open **`GodotGame/project.godot`** with Godot (not the `.godot/` cache directory).

Autoload `AvaloniaLoader` and the default `UserInterface` (`UiHost`) are already configured.

## Layout

| Path | Role |
|------|------|
| `EstragoniaApp.sln` | Solution (parent of the two projects) |
| `GodotGame/project.godot` | Open this in Godot |
| `GodotGame/GodotGame.csproj` | Godot assembly — Autoload + `UiHost` |
| `GodotGame.UI/GodotGame.UI.csproj` | Avalonia assembly — views / previewer |
| `GodotGame.UI/Views/` / `ViewModels/` | Avalonia MVVM UI |
| `GodotGame.UI/App.axaml` | Avalonia application + theme |
| `GodotGame.UI/Designer.cs` | Avalonia previewer (`Main` + `BuildAvaloniaApp`) |
| `GodotGame/Estragonia/` | Host scripts: Autoload, `AvaloniaControl`, `UiHost`, `UserInterface` |
| `Directory.Packages.props` | NuGet versions (solution-level) |
| `global.json` | .NET SDK pin |

Edit `GodotGame.UI/Views/MainView.axaml` and `GodotGame.UI/ViewModels/MainViewModel.cs` to build your UI.

## Visual Studio / previewer

- Install the **Avalonia for Visual Studio** extension.
- Open AXAML under **`GodotGame.UI`** (not the Godot project). That assembly has no Godot scripts, so the previewer can load it.
- `GodotGame.UI/Designer.cs` provides `Main` + `BuildAvaloniaApp` (Debug `OutputType=Exe`).
  (Keep that file free of C# `#if …` directives — `dotnet new` would strip them and leave an empty file.)
- Runtime still runs inside Godot via `GodotGame/project.godot` — do not start either C# project as a normal console app.
