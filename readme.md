# Estragonia

在 **Godot 4** 中嵌入 **Avalonia** UI 的桥接库（Vulkan / Skia 共享纹理）。

## 重要声明

- 本仓库大量代码由 **AI 辅助编写与改写**（含 Avalonia 12 / Godot 4.7 / .NET 10 适配）。
- **不保证稳定性**，请勿在未验证的情况下直接用于生产。
- **维护者会审查**关键改动；欢迎 Issue / PR，但请自行充分测试。

## 署名与许可

基于 [Julien Lebosquain](https://github.com/MrJul) 的开源项目 [Estragonia](https://github.com/MrJul/Estragonia)（**MIT**）。  
原作者版权与许可声明见 [`license.txt`](license.txt)；修改与再分发须保留 MIT 要求的版权与许可文本。

## 环境

| 项 | 版本 |
|----|------|
| .NET SDK | 10.x（见 `global.json`） |
| Godot | 4.7.x（.NET / Forward+ 或 Mobile） |
| Avalonia | 12.x |

包版本由 **CPM** 统一管理：`Directory.Packages.props`。

## 快速开始

### 用模板新建项目（推荐）

```bash
# 打包库 Ouse.Estragonia + 模板
dotnet pack src/JLeb.Estragonia -c Release -o nupkgs
dotnet pack templates/Ouse.Estragonia.Templates.csproj -c Release -o nupkgs
dotnet new install ./nupkgs/Ouse.Estragonia.Templates.1.0.0.nupkg

# -n = 解决方案名；--GodotProjectName = Godot/C# 项目名
dotnet new estragonia -n MySolution --GodotProjectName MyGame -o MySolution
cd MySolution
dotnet restore
```

NuGet 包名：**`Ouse.Estragonia`**（代码命名空间仍为 `JLeb.Estragonia`）。

用 **Godot 4.7+（.NET）打开根目录的 `project.godot`**（不是 `.godot/` 缓存目录）。  
Autoload `AvaloniaLoader` 与默认 `UserInterface`（`UiHost`）已配好。详见 [`templates/README.md`](templates/README.md)。

### 或运行示例

1. 用 Godot（.NET）打开 `samples/HelloWorld`。
2. 编译并运行；Autoload `AvaloniaLoader` 负责 `UseGodot()`。
3. UI 宿主继承 `JLeb.Estragonia.UiHost`，实现 `CreateRoot()` 即可。

## 仓库结构

```
src/JLeb.Estragonia/   # 桥接库
samples/HelloWorld/    # 示例
docs/v1.0.0/           # 手写文档（英 / 中）
docfx.json             # DocFX 配置
DOCFX-AI-PROMPT.md     # 文档维护提示词（给 AI）
```

## 文档 / GitHub Pages

推送到 `main` 后，Actions 工作流 `.github/workflows/docs.yml` 会生成 DocFX 并部署到 **GitHub Pages**。  
仓库 Settings → Pages → Source 选 **GitHub Actions**。

本地预览（需已安装 [DocFX](https://dotnet.github.io/docfx/)）：

```bash
docfx docfx.json
docfx serve _site --port 8080
```

> `api/` 与 `_site/` 为生成物，**已 gitignore，不要提交到远端**。

文档维护规则见 [`DOCFX-AI-PROMPT.md`](DOCFX-AI-PROMPT.md)。
