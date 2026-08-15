# Estragonia Editor (sample addon)

Left-dock Avalonia UI. Shares one process-wide `Application` with `addons/estragonia_editor_log`.

1. `AvaloniaEditorRuntime.EnsureStarted()` — first caller starts Avalonia; later callers no-op.
2. `new AvaloniaEditorHost { CreateRoot = () => new YourView() }`
3. Wrap in `EditorDock`, then `AddDock`.

Godot Build does not run plugin `_ExitTree`. The runtime hooks the C# rebuild and tears Avalonia down so hot-reload can unload assemblies.
