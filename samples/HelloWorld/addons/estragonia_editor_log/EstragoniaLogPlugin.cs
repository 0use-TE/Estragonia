#if TOOLS
using Godot;
using HelloWorld.Editor;
using JLeb.Estragonia;

namespace HelloWorld.EditorLog;

[Tool]
public partial class EstragoniaLogPlugin : EditorPlugin {

	private EditorDock? _dock;
	private AvaloniaEditorHost? _host;

	public override void _EnterTree() {
		AvaloniaEditorRuntime.EnsureStarted();

		_host = new AvaloniaEditorHost {
			Name = "EstragoniaLogHost",
			CustomMinimumSize = new Vector2(420, 160),
			CreateRoot = static () => new LogView()
		};

		_dock = new EditorDock {
			Title = "Estragonia Log",
			ClipContents = true,
			DefaultSlot = EditorDock.DockSlot.Bottom,
			AvailableLayouts = EditorDock.DockLayout.Horizontal | EditorDock.DockLayout.Floating
		};
		_dock.AddChild(_host);
		AddDock(_dock);
	}

	public override void _ExitTree() {
		_host?.Detach();
		_host = null;

		if (_dock is not null) {
			RemoveDock(_dock);
			_dock.QueueFree();
			_dock = null;
		}
	}

}
#endif
