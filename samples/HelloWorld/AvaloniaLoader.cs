using Avalonia;
using Godot;
using JLeb.Estragonia;

namespace HelloWorld;

/// <summary>Autoload: Avalonia platform init + asset loader + IME. Once per run.</summary>
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
