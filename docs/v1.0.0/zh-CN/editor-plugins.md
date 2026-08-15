# 编辑器插件（Avalonia）

在 Godot 编辑器里嵌 Avalonia XAML。整个编辑器进程只共享 **一份** Avalonia `Application`；每个插件各自挂自己的 Godot 宿主 `Control`（Dock 或底栏）。

## 原理

```text
Godot 编辑器进程
  └── .NET 运行时（一个 CLR）
        ├── AvaloniaEditorRuntime.EnsureStarted<TApp>()   ← 只有第一次会真正启动
        │     UseGodot() + SetupWithoutStarting()
        ├── 插件 A → AvaloniaEditorHost → TopLevel A → 纹理 A
        └── 插件 B → AvaloniaEditorHost → TopLevel B → 纹理 B
```

共享：`Application`、主题、`UseGodot()` 平台服务（Vulkan/Skia、剪贴板、光标）。  
不共享：每个 Dock 自己的 `TopLevel` 和 GPU 纹理。

禁用插件时在 `_ExitTree` 里卸掉宿主即可。**不要** Shutdown 整个 Avalonia，其它插件还在用。

## 怎么引用

| 在 NuGet（`Ouse.Estragonia`）里 | 必须放在你的 Godot 工程里 |
|----------------------------------|---------------------------|
| `AvaloniaEditorRuntime` / `GodotAvalonia.EnsureStarted` | `AvaloniaEditorHost.cs`（Godot `Control`） |
| `AvaloniaControlEngine`、Vulkan/Skia 桥 | `EditorPlugin` + `plugin.cfg` |
| | 你的 AXAML 界面 |

宿主脚本不能只放在 NuGet 程序集里（和游戏侧 `AvaloniaControl` 同一条 Godot 限制）。

## 最小插件

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

从 `samples/HelloWorld/addons/estragonia_editor/` 复制 `AvaloniaEditorHost.cs`。

## 示例

`samples/HelloWorld` 启用了两个插件：

| 插件 | 界面 |
|------|------|
| `addons/estragonia_editor` | 左侧 Dock（`DemoView`） |
| `addons/estragonia_editor_log` | 底栏（`LogView`） |

用 Godot 4.7+（.NET）打开 HelloWorld，渲染器 **Forward+**。先编译 C#，再在「项目 → 项目设置 → 插件」里确认两个插件已启用。

## 要求

- Godot .NET 编辑器，渲染器 **Forward+** 或 **Mobile**（Vulkan）
- 与游戏相同的 Estragonia 栈（`UseGodot`）
- `EditorPlugin` 和宿主加 `#if TOOLS` + `[Tool]`

游戏 Autoload 也应调用 `GodotAvalonia.EnsureStarted<App>()`，避免运行游戏与编辑器各 Setup 一次。
