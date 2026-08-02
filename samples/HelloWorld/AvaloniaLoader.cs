using Avalonia;
using Godot;
using JLeb.Estragonia;

namespace HelloWorld;

public partial class AvaloniaLoader : Node {

	public override void _Ready() {
		AppBuilder
			.Configure<App>()
			.UseGodot()
			.SetupWithoutStarting();

		// Make sure XAML image / avares loading works under Godot's assembly load context.
		GodotAvalonia.EnsureAssetLoader(typeof(App).Assembly);
	}

}
