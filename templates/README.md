# Estragonia templates

NuGet: [Ouse.Estragonia.Templates](https://www.nuget.org/packages/Ouse.Estragonia.Templates/)  
Library: [Ouse.Estragonia](https://www.nuget.org/packages/Ouse.Estragonia/)  
Repo: [0use-TE/Estragonia](https://github.com/0use-TE/Estragonia)

## Install from NuGet

```bash
dotnet new install Ouse.Estragonia.Templates
```

Update:

```bash
dotnet new uninstall Ouse.Estragonia.Templates
dotnet new install Ouse.Estragonia.Templates
```

Then **restart Visual Studio** if you use the New Project dialog.

## Create a project

```bash
dotnet new estragonia -n MySolution --GodotProjectName MyGame -o MySolution
cd MySolution
dotnet restore
```

Open **`project.godot`** with Godot 4.7+ (.NET).

| Parameter | CLI | Visual Studio | Meaning |
|-----------|-----|---------------|---------|
| Solution name | `-n` / `--name` | Project name | `*.sln` name |
| Output folder | `-o` | Location | Output directory |
| Godot project name | `--GodotProjectName` | Godot project name | C# project / assembly / namespace |

The generated project references **`Ouse.Estragonia`** from nuget.org.  
C# namespaces remain **`JLeb.Estragonia`**.

## Solution layout

```
Solution 'MySolution'
├── Solution Items
│   ├── README.md
│   ├── global.json
│   ├── Directory.Build.props
│   ├── Directory.Packages.props
│   └── project.godot          ← open in Godot
└── MyGame
    ├── Views/ / ViewModels/
    ├── App.axaml
    ├── AvaloniaLoader.cs      ← Autoload
    ├── Designer.cs            ← Avalonia previewer (Main + BuildAvaloniaApp)
    └── UserInterface.cs       ← UiHost
```

## Local pack (contributors)

```bash
dotnet pack src/JLeb.Estragonia -c Release -o nupkgs
dotnet pack templates/Ouse.Estragonia.Templates.csproj -c Release -o nupkgs
dotnet new uninstall Ouse.Estragonia.Templates
dotnet new install ./nupkgs/Ouse.Estragonia.Templates.1.0.0.nupkg
```
