# Getting started

## Links

| | URL |
|--|-----|
| Library | [Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/) |
| Template | [Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/) |
| Source | [0use-TE/Estragonia](https://github.com/0use-TE/Estragonia) |

Package id is **`Ouse.Estragonia`**; C# namespaces remain **`JLeb.Estragonia`**.

## Requirements

- Godot **4.7+** (.NET build), renderer **Forward+** or **Mobile** (Vulkan)
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

Visual Studio: **Create a new project** → **Estragonia Godot App** (restart VS after installing the template).

### 3. Open in Godot

Open **`project.godot`** at the solution root with Godot 4.7+ (.NET).  
Do **not** open the `.godot/` cache folder.

Already wired:

- Autoload `AvaloniaLoader` → `UseGodot()` once
- `UserInterface` : `UiHost` → `CreateRoot()`
- `Designer.cs` for Avalonia XAML preview (`Main` + `BuildAvaloniaApp`)

### 4. Edit the UI

- View: `Views/MainView.axaml`
- ViewModel: `ViewModels/MainViewModel.cs`
- Theme: `App.axaml`

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

Open `samples/HelloWorld` in Godot (uses a project reference to the library source).

## Hot reload

If Godot reports `Failed to unload assemblies` or  
`An item with the same key has already been added` for `AvaloniaControl` / `UiHost`, **fully restart the editor**. Estragonia + Avalonia often prevent clean unload.

## Disclaimer

This tree contains AI-assisted changes. Validate before shipping.
