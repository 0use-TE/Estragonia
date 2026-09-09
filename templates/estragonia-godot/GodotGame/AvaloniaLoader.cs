using Avalonia;
using Godot;
using JLeb.Estragonia;

namespace GodotGame;

/// <summary>Autoload: initializes Avalonia once per run (platform + assets + IME).</summary>
public partial class AvaloniaLoader : Node {

	public override void _Ready() {
		AppBuilder
			.Configure<App>()
			.UseGodot()
			.SetupWithoutStarting();

		GodotAvalonia.EnsureAssetLoader(typeof(App).Assembly);
		GetWindow()?.SetImeActive(true);
	}

}
