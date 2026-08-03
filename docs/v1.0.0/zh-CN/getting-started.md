# 快速开始

## 链接

| | URL |
|--|-----|
| 库 | [Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/) |
| 模板 | [Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/) |
| 源码 | [0use-TE/Estragonia](https://github.com/0use-TE/Estragonia) |

NuGet 包名是 **`Ouse.Estragonia`**；代码命名空间仍是 **`JLeb.Estragonia`**。

## 环境

- Godot **4.7+**（.NET），渲染器 **Forward+** 或 **Mobile**（Vulkan）
- .NET SDK **10**
- Avalonia **12**

---

## 教程 A — 用模板创建（推荐）

### 1. 安装模板

```bash
dotnet new install Ouse.Estragonia.Templates
```

### 2. 创建项目

```bash
dotnet new estragonia -n MySolution --GodotProjectName MyGame -o MySolution
cd MySolution
dotnet restore
```

| 参数 | 含义 |
|------|------|
| `-n` / `--name` | 解决方案名 |
| `--GodotProjectName` | Godot / C# 项目名（合法 C# 标识符） |
| `-o` | 输出目录 |

Visual Studio：新建项目 → 搜 **Estragonia Godot App**（装完模板后若没有，请重启 VS）。

### 3. 用 Godot 打开

用 Godot 4.7+（.NET）打开根目录的 **`project.godot`**。  
不要打开 `.godot/` 缓存目录。

模板已配置：

- Autoload `AvaloniaLoader` → 只初始化一次 `UseGodot()`
- `UserInterface` : `UiHost` → `CreateRoot()`
- `Designer.cs` 供 Avalonia 预览（`Main` + `BuildAvaloniaApp`）

### 4. 改 UI

- 视图：`Views/MainView.axaml`
- 视图模型：`ViewModels/MainViewModel.cs`
- 主题：`App.axaml`

---

## 教程 B — 给已有 Godot C# 工程加包

```bash
dotnet add package Ouse.Estragonia
dotnet add package Semi.Avalonia
```

1. 准备 Avalonia `Application` 与主题（如 Semi）。
2. Autoload（整个运行只调用一次）：

```csharp
using Avalonia;
using Godot;
using JLeb.Estragonia;

public partial class AvaloniaLoader : Node
{
    public override void _Ready()
    {
        AppBuilder.Configure<App>()
            .UseGodot()
            .SetupWithoutStarting();

        GodotAvalonia.EnsureAssetLoader(typeof(App).Assembly);
        GetWindow()?.SetImeActive(true);
    }
}
```

3. 场景里挂 `Control`，脚本继承 `UiHost`：

```csharp
using Avalonia.Controls;
using JLeb.Estragonia;

public partial class UserInterface : UiHost
{
    protected override Control CreateRoot()
        => new MainView { DataContext = new MainViewModel() };
}
```

详见 [宿主与 UI](hosting.md)。

---

## 本仓库示例

用 Godot 打开 `samples/HelloWorld`（通过工程引用本地库源码）。

## 热重载

若出现 `Failed to unload assemblies`，或  
`An item with the same key has already been added`（`AvaloniaControl` / `UiHost`），请 **完全重启 Godot** 再运行。

## 声明

本仓库含 AI 辅助改动，请自行验证后再用于正式环境。
