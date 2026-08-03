using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloWorld.ViewModels;

public sealed partial class HelloWorldViewModel : ObservableObject {

	public event Action? PaintOuseRequested;
	public event Action? ResetOuseRequested;

	[ObservableProperty]
	private bool _isFadePanelHidden;

	[ObservableProperty]
	private bool _isPulseVisible = true;

	[ObservableProperty]
	private double _progress;

	[ObservableProperty]
	private double _sliderValue = 35;

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(PlayProgressCommand))]
	private bool _isProgressRunning;

	[RelayCommand]
	private void PaintOuse()
		=> PaintOuseRequested?.Invoke();

	[RelayCommand]
	private void ResetOuse()
		=> ResetOuseRequested?.Invoke();

	[RelayCommand]
	private void ToggleFadePanel()
		=> IsFadePanelHidden = !IsFadePanelHidden;

	[RelayCommand(CanExecute = nameof(CanPlayProgress))]
	private async Task PlayProgressAsync() {
		IsProgressRunning = true;
		Progress = 0;
		try {
			for (var i = 0; i <= 100; i += 2) {
				Progress = i;
				await Task.Delay(28);
			}
		}
		finally {
			IsProgressRunning = false;
		}
	}

	private bool CanPlayProgress()
		=> !IsProgressRunning;

}
