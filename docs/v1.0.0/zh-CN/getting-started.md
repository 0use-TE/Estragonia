# 快速开始

## 链接

| | URL |
|--|-----|
| 库 | [Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/) |
| 模板 | [Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/) |
| 源码 | [0use-TE/Estragonia](https://github.com/0use-TE/Estragonia) |

NuGet 包名是 **`Ouse.Estragonia`**；桥接命名空间仍是 **`JLeb.Estragonia`**。

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
- **工程内已带 `AvaloniaControl.cs` + `UiHost.cs`**（必需宿主脚本）
- `UserInterface` : `UiHost` → `CreateRoot()`
- `Designer.cs` 供 Avalonia 预览（`Main` + `BuildAvaloniaApp`）

### 4. 改 UI

- 视图：`Views/MainView.axaml`
- 视图模型：`ViewModels/MainViewModel.cs`
- 主题：`App.axaml`

---

## 教程 B — 给已有 Godot C# 工程加包

**只装 NuGet 不够。** 还必须把宿主脚本复制进 Godot 工程。

```bash
dotnet add package Ouse.Estragonia
dotnet add package Semi.Avalonia
```

### 1. 把宿主脚本复制到 Godot 工程根目录

从本仓库（或已生成的模板工程）复制下面 **两个文件** 到你的 `.csproj` / `project.godot` 旁边：

| 文件 | 作用 |
|------|------|
| `AvaloniaControl.cs` | 渲染 Avalonia 的 Godot `Control` |
| `UiHost.cs` | 焦点 + `CreateRoot()` 样板 |

来源：

- `templates/estragonia-godot/AvaloniaControl.cs`
- `templates/estragonia-godot/UiHost.cs`
- 或 `samples/HelloWorld/` 下同名文件

文件内命名空间保持 `JLeb.Estragonia`（或同步改你的 `UserInterface`）。  
**类名必须等于文件名。** 不要把这些类型只放在类库里。

### 2. Avalonia `Application` + 主题

准备 `App.axaml` / `App.axaml.cs`（如 Semi）。

### 3. Autoload（整个运行只调用一次）

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

在 `project.godot` 里注册为 Autoload。

### 4. 场景宿主脚本

```csharp
using Avalonia.Controls;
using JLeb.Estragonia;

public partial class UserInterface : UiHost
{
    protected override Control CreateRoot()
        => new MainView { DataContext = new MainViewModel() };
}
```

把 `UserInterface.cs` 挂到主场景里铺满的 `Control` 上。

完整文件清单见 [宿主与 UI](hosting.md)。

---

## 本仓库示例

用 Godot 打开 `samples/HelloWorld`（通过工程引用本地库源码）。  
示例里已经包含 `AvaloniaControl.cs` 和 `UiHost.cs`。

## 热重载

`AvaloniaControl` / `UiHost` 已放在 Godot 工程内，可按普通脚本热重载。

若 Avalonia 等库拖住程序集，仍可能出现 **Failed to unload assemblies**。编辑器卡死时：完全重启 Godot；必要则删 `.godot` 再开。

## 声明

本仓库含 AI 辅助改动，请自行验证后再用于正式环境。
