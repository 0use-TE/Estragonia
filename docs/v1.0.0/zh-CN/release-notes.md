# 更新说明

## 1.0.3

- Godot 宿主脚本（`AvaloniaControl`、`UiHost`、Autoload、`UserInterface`）放在工程内 `Estragonia/`；NuGet 库只带 `AvaloniaControlEngine`，便于编辑器热重载卸载游戏程序集
- 模板/示例仍把 Avalonia UI 拆成独立项目（`*.UI`），给预览器用
- 模板输出目录就是解决方案根目录（`.sln` 和 `GodotGame` / `GodotGame.UI` 同一层），不再多套一层（关闭 `preferNameDirectory`）

## 1.0.2

- Godot **4.7.2**（`Godot.NET.Sdk` / `GodotSharp`）
- 模板将 Avalonia 拆到 `GodotGame.UI`（预览器），Godot 脚本单独一个程序集；`.sln` 和两个项目同一层

## 1.0.0

本维护分支的首个版本：

- Avalonia 12 / Godot 4.7 / .NET 10
- CPM 统一包版本（`Directory.Packages.props`）
- `UiHost`、输入穿透、Avalonia 12 Dispatcher / 资源加载修复
- DocFX 文档 + GitHub Pages 工作流
- `dotnet new estragonia` 项目模板
- NuGet：[Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/) + [Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/)
- 源码：[0use-TE/Estragonia](https://github.com/0use-TE/Estragonia)

基于 [MrJul/Estragonia](https://github.com/MrJul/Estragonia)（MIT）。上游包名仍为 `JLeb.Estragonia`。
