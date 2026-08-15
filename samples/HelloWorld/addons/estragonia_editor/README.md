# Estragonia Editor (sample addon)

- `plugin.gd` — Godot `EditorPlugin` shell (not C#)
- `AvaloniaEditorHost.cs` — C# `Control` that overrides `_Draw` / `_Process` / input
- Your Avalonia view type name goes in `estragonia_view_type`

GDScript cannot call custom C# methods on a preloaded `.cs` script. The host must be a C# `Control` so Godot calls the engine virtuals.
