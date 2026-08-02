using Avalonia.Controls;
using Avalonia.Interactivity;

namespace HelloWorld;

public partial class HelloWorldView : UserControl {

	private int _clickCount;

	public HelloWorldView() {
		InitializeComponent();
		ClickMeButton.Click += OnClickMeButtonClick;
	}

	private void OnClickMeButtonClick(object? sender, RoutedEventArgs e) {
		_clickCount++;
		ClickStatus.Text = $"Clicked {_clickCount} time(s).";
	}

}
