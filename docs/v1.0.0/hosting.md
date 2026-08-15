# Hosting UI

## Critical rule — host scripts stay in the Godot project

`AvaloniaControl` and `UiHost` are **Godot node scripts**. They **must** be `.cs` files inside your Godot C# project (same assembly as `project.godot`).

| Location | OK? |
|----------|-----|
| Your Godot project (`res://AvaloniaControl.cs`, `res://UiHost.cs`, …) | Yes |
| Copied from `templates/estragonia-godot` or `samples/HelloWorld` | Yes |
| Only referencing `Ouse.Estragonia` NuGet / library assembly | **No** |

Godot’s C# editor hot-reload **does not support** Godot node types that live only in an external assembly (NuGet / `ProjectReference`). That is an engine limitation ([godot#111881](https://github.com/godotengine/godot/issues/111881), [godot#98094](https://github.com/godotengine/godot/issues/98094)).

The NuGet package (`Ouse.Estragonia`) provides the platform bridge (`UseGodot`, Vulkan/Skia, `AvaloniaControlEngine`, …). It does **not** ship `AvaloniaControl` / `UiHost` as Godot node scripts — those two `.cs` files must be in your Godot project (template/sample already include them).

If you use the **template**, those two files are already included. If you add the package by hand, **copy them** from the template or sample into your project (class name = file name, one Godot class per file).

## Split responsibilities

| Concern | Where |
|---------|--------|
| `UseGodot` / asset loader / IME | Autoload (`AvaloniaLoader`) once |
| Rendering + input host | `AvaloniaControl.cs` **in your Godot project** |
| Focus / mouse filter / `CreateRoot` | `UiHost.cs` **in your Godot project** |
| Your scene script | `UserInterface : UiHost` → implement `CreateRoot()` |
| Theme / app resources | Avalonia `Application` |

Do **not** call `GrabFocus` / `GetWindow` from Avalonia `App`.

## Required project files

```
YourGodotProject/
├── AvaloniaControl.cs   ← copy from template/sample (do not omit)
├── UiHost.cs            ← copy from template/sample (do not omit)
├── AvaloniaLoader.cs    ← Autoload
├── UserInterface.cs     ← your host: CreateRoot()
├── App.axaml (+ .cs)
└── Views/ …
```

## UiHost

```csharp
// UiHost.cs — must be in the Godot project
namespace JLeb.Estragonia;

public abstract partial class UiHost : AvaloniaControl
{
    protected abstract Control CreateRoot();

    public override void _Ready()
    {
        FocusMode = FocusModeEnum.All;
        MouseFilter = MouseFilterEnum.Stop;
        Control = CreateRoot();
        base._Ready();
        GrabFocus();
    }
}
```

```csharp
// UserInterface.cs — your scene script
using JLeb.Estragonia;

public partial class UserInterface : UiHost
{
    protected override Control CreateRoot()
        => new MainView { DataContext = new MainViewModel() };
}
```

## Hit testing

By default `AvaloniaControl.CaptureEmptyHits` is `false`: only Avalonia-hittable pixels capture the mouse; empty areas pass through to Godot (e.g. a `Sprite2D` behind the host).

Set `CaptureEmptyHits = true` to capture the whole control rect.

## Editor plugins

To host Avalonia inside the Godot editor (shared `Application`, one host per dock), see [Editor plugins](editor-plugins.md).
