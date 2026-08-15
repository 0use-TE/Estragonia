using Avalonia.Controls;
using Avalonia.Interactivity;
using JLeb.Estragonia;

namespace HelloWorld.Editor;

public partial class DemoView : UserControl {

	private int _pings;

	public DemoView()
		=> InitializeComponent();

	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);
		StatusText.Text = AvaloniaEditorRuntime.IsStarted
			? "Shared runtime is started. Application.Current is reused by every dock."
			: "Shared runtime is not started.";
	}

	private void OnPing(object? sender, RoutedEventArgs e) {
		_pings++;
		var name = string.IsNullOrWhiteSpace(NameBox.Text) ? "editor" : NameBox.Text.Trim();
		StatusText.Text = $"Ping #{_pings} from {name}. App = {Avalonia.Application.Current?.GetType().Name}";
	}

}
