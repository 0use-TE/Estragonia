using Avalonia;
using Avalonia.Markup.Xaml;

namespace GodotGame;

public class App : Application {

	public override void Initialize()
		=> AvaloniaXamlLoader.Load(this);

}
