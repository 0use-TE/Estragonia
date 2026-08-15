# Editor plugins (Avalonia)

Embed Avalonia XAML in the Godot editor. One process-wide Avalonia `Application` is shared; each plugin adds its own Godot host `Control` (dock or bottom panel).

## How it works

```text
Godot Editor process
  └── .NET runtime (one CLR)
        ├── AvaloniaEditorRuntime.EnsureStarted<TApp>()   ← first caller only
        │     UseGodot() + SetupWithoutStarting()
        ├── Plugin A → AvaloniaEditorHost → TopLevel A → texture A
        └── Plugin B → AvaloniaEditorHost → TopLevel B → texture B
```

Shared: `Application`, theme, `UseGodot()` platform services (Vulkan/Skia, clipboard, cursor).  
Not shared: each dock’s `TopLevel` and GPU texture.

Disable a plugin with `_ExitTree` → remove the host. **Do not** shut Avalonia down; other plugins still need it.

## What you reference

| Lives in NuGet (`Ouse.Estragonia`) | Lives in your Godot project |
|------------------------------------|-----------------------------|
| `AvaloniaEditorRuntime` / `GodotAvalonia.EnsureStarted` | `AvaloniaEditorHost.cs` (Godot `Control`) |
| `AvaloniaControlEngine`, Vulkan/Skia bridge | `EditorPlugin` + `plugin.cfg` |
| | Your AXAML views |

Host scripts cannot live only in the NuGet assembly (same Godot limitation as game `AvaloniaControl`).

## Minimal plugin

```csharp
#if TOOLS
using Godot;
using JLeb.Estragonia;

[Tool]
public partial class MyEditorPlugin : EditorPlugin {

    private EditorDock? _dock;
    private AvaloniaEditorHost? _host;

    public override void _EnterTree() {
        AvaloniaEditorRuntime.EnsureStarted<MyEditorApp>();
        _host = new AvaloniaEditorHost {
            Name = "MyTool",
            CreateRoot = static () => new MyView()
        };
        _dock = new EditorDock();
        _dock.Title = "MyTool";
        _dock.DefaultSlot = EditorDock.DockSlot.LeftUl;
        _dock.AddChild(_host);
        AddDock(_dock);
    }

    public override void _ExitTree() {
        if (_dock is null) return;
        RemoveDock(_dock);
        _dock.QueueFree();
        _dock = null;
    }
}
#endif
```

Copy `AvaloniaEditorHost.cs` from `samples/HelloWorld/addons/estragonia_editor/`.

## Sample

`samples/HelloWorld` enables two plugins:

| Plugin | UI |
|--------|-----|
| `addons/estragonia_editor` | Left dock (`DemoView`) |
| `addons/estragonia_editor_log` | Bottom panel (`LogView`) |

Open the HelloWorld folder in Godot 4.7+ (.NET), **Forward+**. Build the C# project, then confirm both plugins are enabled under Project → Project Settings → Plugins.

## Requirements

- Godot .NET editor, renderer **Forward+** or **Mobile** (Vulkan)
- Same Estragonia stack as the game (`UseGodot`)
- `#if TOOLS` + `[Tool]` on the `EditorPlugin` and host

Game Autoload should also call `GodotAvalonia.EnsureStarted<App>()` so play-mode and the editor cannot double-initialize.
