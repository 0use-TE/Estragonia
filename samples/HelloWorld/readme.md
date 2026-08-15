# HelloWorld

Estragonia 场景演示（左侧列表切换）。

## 场景

| 场景 | 内容 |
|------|------|
| 总览 | 简介与推荐体验顺序 |
| 基础控件 | TextBox / ComboBox / Slider / Toggle / Expander 等 |
| 自定义光标 | 标准 CursorShape + `CreateCursor(Bitmap)` 位图光标 |
| 游戏 HUD | HP/MP/EXP、金币、波次、战斗日志 |
| 输入穿透 | 透明区把鼠标交给背后 Godot Sprite |
| 数据绑定 | MVVM 列表增删 |

## 宿主脚本

本示例的 Godot 宿主类型在工程内（不在 NuGet）：

- `AvaloniaControl.cs`
- `UiHost.cs`
- `UserInterface.cs` → `CreateRoot()`

## 编辑器插件

打开本工程后，编辑器会启用两个 Avalonia 插件（共享一份 `Application`）：

| 插件 | 位置 |
|------|------|
| `addons/estragonia_editor` | 左侧 Dock |
| `addons/estragonia_editor_log` | 底栏「Estragonia Log」 |

插件入口是 GDScript（`plugin.gd`）。C# 只提供 Avalonia 页和静态 `EstragoniaEditorBridge`，不要把 `EditorPlugin` 写成 C#。

## 运行

1. 用 **Godot 4.7+（.NET）** 打开本目录（Forward+）。
2. 编译 C# 后查看左侧 Dock / 底栏插件。
3. 运行主场景；切到「输入穿透」或「自定义光标」验证运行时能力。
