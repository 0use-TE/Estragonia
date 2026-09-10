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
├── MyGame/                              ← Godot 工程（打开 project.godot）
│   ├── project.godot
│   └── Estragonia/
└── MyGame.UI/                           ← Avalonia（预览器）
    ├── App.axaml
    ├── Designer.cs
    ├── Views/
    └── ViewModels/
```

CLI 请用 `-o` 指到最终目录，不要再套一层同名文件夹。Visual Studio 勾选 **将解决方案和项目放在同一目录中**。

## Local pack (contributors)

```bash
dotnet pack src/JLeb.Estragonia -c Release -o nupkgs
dotnet pack templates/Ouse.Estragonia.Templates.csproj -c Release -o nupkgs
dotnet new uninstall Ouse.Estragonia.Templates
dotnet new install ./nupkgs/Ouse.Estragonia.Templates.1.0.3.nupkg
```
