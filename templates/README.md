# Estragonia templates

## Install / update

```bash
# From repo root
dotnet pack src/JLeb.Estragonia -c Release -o nupkgs
dotnet pack templates/Ouse.Estragonia.Templates.csproj -c Release -o nupkgs

dotnet new uninstall Ouse.Estragonia.Templates
dotnet new uninstall JLeb.Estragonia.Templates
dotnet new install ./nupkgs/Ouse.Estragonia.Templates.1.0.0.nupkg
```

Then **restart Visual Studio** (required after template install/update).

## Create

```bash
dotnet new estragonia -n MySolution --GodotProjectName MyGame -o MySolution
```

Or in VS: **Create a new project** → search **Estragonia Godot App**.

| Parameter | CLI | Visual Studio | Meaning |
|-----------|-----|---------------|---------|
| Solution name | `-n` / `--name` | Project name | `*.sln` name |
| Output folder | `-o` | Location | Output directory |
| Godot project name | `--GodotProjectName` | Godot project name | C# project / assembly / namespace |

NuGet library package: **`Ouse.Estragonia`** (namespaces remain `JLeb.Estragonia`).

Open **`project.godot`** with Godot 4.7+ (.NET).

## What you should see in Solution Explorer

```
Solution 'MySolution'
├── Solution Items
│   ├── README.md
│   ├── global.json
│   ├── Directory.Build.props
│   ├── Directory.Packages.props
│   └── project.godot
└── MyGame
    ├── Views/
    ├── ViewModels/
    ├── App.axaml
    ├── AvaloniaLoader.cs
    ├── Designer.cs
    └── UserInterface.cs
```
