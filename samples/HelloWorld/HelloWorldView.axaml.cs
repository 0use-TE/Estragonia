using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HelloWorld;

public partial class HelloWorldView : UserControl {

	public event Action? PaintOuseRequested;
	public event Action? ResetOuseRequested;

	private bool _fadeHidden;
	private bool _progressRunning;

	public HelloWorldView() {
		InitializeComponent();
		PulseToggle.IsCheckedChanged += OnPulseToggleChanged;
	}

	private void Button_Click(object? sender, RoutedEventArgs e)
		=> PaintOuseRequested?.Invoke();

	private void OnResetOuseClick(object? sender, RoutedEventArgs e)
		=> ResetOuseRequested?.Invoke();

	private void OnToggleFadeClick(object? sender, RoutedEventArgs e) {
		_fadeHidden = !_fadeHidden;
		if (_fadeHidden)
			FadePanel.Classes.Add("hidden");
		else
			FadePanel.Classes.Remove("hidden");
	}

	private void OnPulseToggleChanged(object? sender, RoutedEventArgs e) {
		PulseBox.IsVisible = PulseToggle.IsChecked == true;
	}

	private async void OnPlayProgressClick(object? sender, RoutedEventArgs e) {
		if (_progressRunning)
			return;

		_progressRunning = true;
		DemoProgress.Value = 0;
		try {
			for (var i = 0; i <= 100; i += 2) {
				DemoProgress.Value = i;
				ProgressLabel.Text = $"{i}%";
				await Task.Delay(28);
			}
		}
		finally {
			_progressRunning = false;
		}
	}

	private void OnStartAssetClick(object? sender, RoutedEventArgs e)
		=> PaintOuseRequested?.Invoke();

}
