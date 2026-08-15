#if TOOLS
using Godot;
using JLeb.Estragonia;

namespace HelloWorld.Editor;

[Tool]
public partial class EstragoniaEditorPlugin : EditorPlugin {

	private EditorDock? _dock;
	private AvaloniaEditorHost? _host;

	public override void _EnterTree() {
		AvaloniaEditorRuntime.EnsureStarted();

		_host = new AvaloniaEditorHost {
			Name = "EstragoniaHost",
			CustomMinimumSize = new Vector2(320, 240),
			CreateRoot = static () => new DemoView()
		};

		_dock = new EditorDock {
			Title = "Estragonia",
			ClipContents = true,
			DefaultSlot = EditorDock.DockSlot.LeftUl,
			AvailableLayouts = EditorDock.DockLayout.Vertical | EditorDock.DockLayout.Floating
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
