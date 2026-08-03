# Getting started

## Requirements

- Godot **4.7+** (.NET build), renderer **Forward+** or **Mobile** (Vulkan)
- .NET SDK **10** (see `global.json`)
- Avalonia **12** (versions pinned in `Directory.Packages.props`)

## Minimal setup

1. Open `samples/HelloWorld` in Godot.
2. Autoload `AvaloniaLoader` runs once:

```csharp
AppBuilder.Configure<App>()
    .UseGodot()
    .SetupWithoutStarting();

GodotAvalonia.EnsureAssetLoader(typeof(App).Assembly);
GetWindow()?.SetImeActive(true);
```

3. Host UI with `UiHost`:

```csharp
public partial class UserInterface : UiHost
{
    protected override Control CreateRoot() => new MainView();
}
```

4. Build & run the scene.

## Samples

| Sample | Purpose |
|--------|---------|
| HelloWorld | Loader + `UiHost` + input / animation demos |

## Disclaimer

This tree contains AI-assisted changes. Validate on your machines before shipping.
