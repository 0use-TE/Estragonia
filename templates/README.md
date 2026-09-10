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

Then **fully restart Visual Studio** (it caches templates). In the New Project dialog you should see **Create in new folder**, not an extra nested project folder + a parent `.slnx`.

## Create a project

```bash
dotnet new estragonia -n MySolution --GodotProjectName MyGame -o MySolution
cd MySolution
dotnet restore
```

Open **`MyGame/project.godot`** with Godot 4.7.2+ (.NET).

| Parameter | CLI | Visual Studio | Meaning |
|-----------|-----|---------------|---------|
| Solution name | `-n` / `--name` | Project name | `*.sln` name |
| Output folder | `-o` | Location | Output directory |
| Godot project name | `--GodotProjectName` | Godot project name | C# project / assembly / namespace |

The generated project references **`Ouse.Estragonia`** from nuget.org.  
C# namespaces remain **`JLeb.Estragonia`**.

## Solution layout

```
MySolution/                              ← 这一层就是解决方案根目录
├── MySolution.sln
├── README.md / global.json / Directory.*.props
└── MyGame/                              ← Godot + Avalonia（打开 project.godot）
    ├── project.godot
    └── UI/
        ├── Estragonia/
        ├── App.axaml
        ├── Views/
        └── ViewModels/
```

CLI：`-o` 指最终目录。Visual Studio：装新模板后完全重启，对话框应是 **Create in new folder**。

## Local pack (contributors)

```bash
dotnet pack src/JLeb.Estragonia -c Release -o nupkgs
dotnet pack templates/Ouse.Estragonia.Templates.csproj -c Release -o nupkgs
dotnet new uninstall Ouse.Estragonia.Templates
dotnet new install ./nupkgs/Ouse.Estragonia.Templates.1.0.6.nupkg
```
