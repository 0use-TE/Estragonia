# 快速开始

## 环境

- Godot **4.7+**（.NET），渲染器 **Forward+** 或 **Mobile**（Vulkan）
- .NET SDK **10**（见 `global.json`）
- Avalonia **12**（版本见 `Directory.Packages.props`）

## 最小步骤

1. 用 Godot 打开 `samples/HelloWorld`。
2. Autoload `AvaloniaLoader` 只初始化一次 Avalonia。
3. 宿主继承 `UiHost`，实现 `CreateRoot()`。
4. 编译运行。

示例工程：`samples/HelloWorld`。

## 声明

本仓库含 AI 辅助改动，请自行验证后再用于正式环境。
