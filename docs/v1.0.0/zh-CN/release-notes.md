# 更新说明

## 1.0.1

- **对包消费者的破坏性变更：** `AvaloniaControl` / `UiHost` 不再作为 Godot 节点类型打进 `Ouse.Estragonia`。请从模板/示例复制进 Godot 工程（或用 `dotnet new estragonia`）。实现逻辑在 NuGet 内的 `AvaloniaControlEngine`。
- 规避宿主类型在外部程序集时 Godot ScriptTypeBiMap 重复 key 热重载错误。
- 自定义光标：Avalonia `CreateCursor(Bitmap)` → Godot `Input.SetCustomMouseCursor`。
- 模板仅通过 NuGet 引用 `Ouse.Estragonia`（`Version="1.*"` 浮动 1.x）。
- HelloWorld 示例：左侧场景列表（控件、光标、HUD、输入穿透、数据绑定）。
- 编辑器插件：进程内共享 `AvaloniaEditorRuntime` / `GodotAvalonia.EnsureStarted`；HelloWorld 带两个示例 addon（左侧 Dock + 底栏）。
- C# 热重载：在 GodotTools `BuildStarted` 和 ALC `Unloading` 上执行 `GodotAvalonia.PrepareForUnload`，避免每次改代码都被 Avalonia 钉住旧程序集。

## 1.0.0

本维护分支的首个版本：

- Avalonia 12 / Godot 4.7 / .NET 10
- CPM 统一包版本（`Directory.Packages.props`）
- 输入穿透、Avalonia 12 Dispatcher / 资源加载修复
- DocFX 文档 + GitHub Pages 工作流
- `dotnet new estragonia` 项目模板
- NuGet：[Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/) + [Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/)
- 源码：[0use-TE/Estragonia](https://github.com/0use-TE/Estragonia)

基于 [MrJul/Estragonia](https://github.com/MrJul/Estragonia)（MIT）。上游包名仍为 `JLeb.Estragonia`。
