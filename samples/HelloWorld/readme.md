# HelloWorld

Minimal Estragonia sample: `AvaloniaLoader` + `UiHost` + Avalonia MVVM UI over Godot.

```
Views/          Avalonia views (.axaml)
ViewModels/     CommunityToolkit.Mvvm view-models
Assets/         avares:// resources
App.axaml       Avalonia application + theme
AvaloniaLoader  Godot Autoload (UseGodot once)
UserInterface   UiHost → creates View + ViewModel
```

Requires Godot 4.7+ (.NET) and .NET 10.
