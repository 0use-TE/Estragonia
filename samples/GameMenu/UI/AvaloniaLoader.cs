using Avalonia;
using Godot;
using JLeb.Estragonia;

namespace GameMenu.UI;

/// <summary>Autoload: Avalonia platform init + asset loader + IME. Once per run.</summary>
public sealed partial class AvaloniaLoader : Node {

	public override void _Ready() {
		AppBuilder
			.Configure<App>()
			.UseGodot()
			.LogToTrace()
			.SetupWithoutStarting();

		GodotAvalonia.EnsureAssetLoader(typeof(App).Assembly);
		GetWindow()?.SetImeActive(true);
	}

}
