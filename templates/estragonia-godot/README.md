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
| `EstragoniaApp.sln` / `EstragoniaApp.slnx` | Solution (same folder as `GodotGame`) |
| `GodotGame/project.godot` | Open this in Godot |
| `GodotGame/GodotGame.csproj` | Single Godot + Avalonia assembly |
| `GodotGame/UI/` | Avalonia UI + `Estragonia/` host scripts |
| `Directory.Packages.props` | NuGet versions (solution-level) |
| `global.json` | .NET SDK pin |

Edit `GodotGame/UI/Views/MainView.axaml` and `GodotGame/UI/ViewModels/MainViewModel.cs` to build your UI.

Runtime is Godot via `GodotGame/project.godot` — do not start the C# project as a console app.
