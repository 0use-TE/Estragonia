using Godot;
using JLeb.Estragonia;

namespace GodotGame;

/// <summary>Autoload: initializes Avalonia once per run (platform + assets + IME).</summary>
public partial class AvaloniaLoader : Node {

	public override void _Ready() {
		if (!Engine.IsEditorHint())
			GodotAvalonia.EnsureStarted<App>();

		GetWindow()?.SetImeActive(true);
	}

}
