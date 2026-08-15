using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using JLeb.Estragonia;

namespace HelloWorld.Editor;

public partial class LogView : UserControl {

	public LogView()
		=> InitializeComponent();

	protected override void OnLoaded(RoutedEventArgs e) {
		base.OnLoaded(e);
		LogList.Items.Add($"Runtime started = {AvaloniaEditorRuntime.IsStarted}");
		LogList.Items.Add($"App = {Avalonia.Application.Current?.GetType().Name}");
	}

	private void OnAppend(object? sender, RoutedEventArgs e)
		=> LogList.Items.Add($"{DateTime.Now:HH:mm:ss}  log from second plugin");

}
