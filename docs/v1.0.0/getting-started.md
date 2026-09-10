# Getting started

## Links

| | URL |
|--|-----|
| Library | [Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/) |
| Template | [Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/) |
| Source | [0use-TE/Estragonia](https://github.com/0use-TE/Estragonia) |

Package id is **`Ouse.Estragonia`**; C# namespaces remain **`JLeb.Estragonia`**.

## Requirements

- Godot **4.7.2+** (.NET build), renderer **Forward+** or **Mobile** (Vulkan)
- .NET SDK **10**
- Avalonia **12**

---

## Tutorial A — `dotnet new` template (recommended)

### 1. Install the template

```bash
dotnet new install Ouse.Estragonia.Templates
```

### 2. Create a project

```bash
dotnet new estragonia -n MySolution --GodotProjectName MyGame -o MySolution
cd MySolution
dotnet restore
```

| Flag | Meaning |
|------|---------|
| `-n` / `--name` | Solution name |
| `--GodotProjectName` | Godot / C# project name (valid C# identifier) |
| `-o` | Output folder |

Visual Studio: **Create a new project** → **Estragonia Godot App** (fully restart VS after installing).  
The dialog should show **Create in new folder** (solution-template mode). That puts the solution file next to `GodotGame` and `GodotGame.UI`.

### 3. Open in Godot

Open **`MyGame/project.godot`** with Godot 4.7.2+ (.NET). The `.sln` is in the **same folder** as `MyGame` and `MyGame.UI`.  
Do **not** open the `.godot/` cache folder.

CLI: pass `-o` as the final directory (no extra nested name folder). Visual Studio: check **Place solution and project in the same directory**.

Already wired:

- Autoload `AvaloniaLoader` → `UseGodot()` once
- `UserInterface` : `UiHost` → `CreateRoot()`
- Separate `MyGame.UI` project for Avalonia (`App`, views, previewer)

### 4. Edit the UI

- View: `MyGame.UI/Views/MainView.axaml`
- ViewModel: `MyGame.UI/ViewModels/MainViewModel.cs`
- Theme: `MyGame.UI/App.axaml`
- Previewer: open AXAML in the **UI project** (not the Godot project)

---

## Tutorial B — add the package to an existing Godot C# project

```bash
dotnet add package Ouse.Estragonia
dotnet add package Semi.Avalonia
```

1. Create an Avalonia `Application` with a theme (e.g. Semi).
2. Autoload (once per run):

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

3. Scene `Control` script:

```csharp
using Avalonia.Controls;
using JLeb.Estragonia;

public partial class UserInterface : UiHost
{
    protected override Control CreateRoot()
        => new MainView { DataContext = new MainViewModel() };
}
```

See [Hosting UI](hosting.md).

---

## Sample in this repo

Open `samples/HelloWorld` in Godot (Avalonia lives in sibling `samples/HelloWorld.UI`; uses a project reference to the library source).

## Hot reload

If Godot reports `Failed to unload assemblies` or  
`An item with the same key has already been added` for `AvaloniaControl` / `UiHost`, **fully restart the editor**. Estragonia + Avalonia often prevent clean unload.

## Disclaimer

This tree contains AI-assisted changes. Validate before shipping.
