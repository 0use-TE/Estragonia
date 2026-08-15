# 宿主与 UI

## 硬性规则 — 宿主脚本必须在 Godot 工程内

`AvaloniaControl` 和 `UiHost` 是 **Godot 节点脚本**，必须以 `.cs` 文件形式放在你的 **Godot C# 工程里**（和 `project.godot` 同一程序集）。

| 放哪 | 可以吗 |
|------|--------|
| 你的 Godot 工程（`res://AvaloniaControl.cs`、`res://UiHost.cs` …） | 可以 |
| 从 `templates/estragonia-godot` 或 `samples/HelloWorld` 复制 | 可以 |
| 只引用 `Ouse.Estragonia` NuGet / 库程序集 | **不行** |

Godot C# 编辑器热重载 **不支持** 仅存在于外部程序集（NuGet / `ProjectReference`）里的 Godot 节点类型。这是引擎限制（[godot#111881](https://github.com/godotengine/godot/issues/111881)、[godot#98094](https://github.com/godotengine/godot/issues/98094)）。

NuGet 包（`Ouse.Estragonia`）提供平台桥接（`UseGodot`、Vulkan/Skia、`AvaloniaControlEngine` 等），**不会**把 `AvaloniaControl` / `UiHost` 作为 Godot 节点脚本装进你的工程——这两个 `.cs` 必须在 Godot 工程里（模板/示例已带）。

用 **模板** 时这两个文件已经带好。手动加包时，请从模板或示例 **原样复制** 到你的工程（类名 = 文件名，一个文件一个 Godot 类）。

## 职责拆分

| 事情 | 放哪 |
|------|------|
| `UseGodot` / 资源 / IME | Autoload 一次（`AvaloniaLoader`） |
| 渲染 + 输入宿主 | 工程内的 `AvaloniaControl.cs` |
| 焦点 / `CreateRoot` | 工程内的 `UiHost.cs` |
| 场景脚本 | `UserInterface : UiHost` → 实现 `CreateRoot()` |
| 主题 / 全局样式 | Avalonia `Application` |

不要在 Avalonia `App` 里调用 `GrabFocus` / `GetWindow`。

## 工程里必须有的文件

```
YourGodotProject/
├── AvaloniaControl.cs   ← 从模板/示例复制（不能省）
├── UiHost.cs            ← 从模板/示例复制（不能省）
├── AvaloniaLoader.cs    ← Autoload
├── UserInterface.cs     ← 你的宿主：CreateRoot()
├── App.axaml (+ .cs)
└── Views/ …
```

## UiHost

```csharp
// UiHost.cs — 必须在 Godot 工程内
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
// UserInterface.cs — 挂到场景上的脚本
using JLeb.Estragonia;

public partial class UserInterface : UiHost
{
    protected override Control CreateRoot()
        => new MainView { DataContext = new MainViewModel() };
}
```

## 命中测试

默认 `CaptureEmptyHits = false`：只有 Avalonia 命中到的像素吃鼠标，空白可穿透到 Godot。

设 `CaptureEmptyHits = true` 则整块控件矩形都吃输入。

## 编辑器插件

要在 Godot 编辑器里嵌 Avalonia（进程内共享一份 `Application`，每个 Dock 一个宿主），见 [编辑器插件](editor-plugins.md)。
