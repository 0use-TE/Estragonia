using Godot;
using JLeb.Estragonia;

namespace HelloWorld;

/// <summary>Autoload: Avalonia platform init + IME. Once per process.</summary>
public partial class AvaloniaLoader : Node {

	public override void _Ready() {
		if (Engine.IsEditorHint())
			GodotAvalonia.EnsureStarted();
		else
			GodotAvalonia.EnsureStarted<App>();

		GetWindow()?.SetImeActive(true);
	}

}
