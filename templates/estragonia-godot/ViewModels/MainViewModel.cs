using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GodotGame.ViewModels;

public sealed partial class MainViewModel : ObservableObject {

	[ObservableProperty]
	private string _title = "Estragonia";

	[ObservableProperty]
	private string _status = "Ready.";

	[RelayCommand]
	private void Greet()
		=> Status = "Hello from Avalonia + Godot!";

}
