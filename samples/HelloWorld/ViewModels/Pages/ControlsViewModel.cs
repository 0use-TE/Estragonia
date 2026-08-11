using CommunityToolkit.Mvvm.ComponentModel;

namespace HelloWorld.ViewModels.Pages;

public sealed partial class ControlsViewModel : ObservableObject {

	[ObservableProperty]
	private string _name = "Ava";

	[ObservableProperty]
	private double _volume = 0.65;

	[ObservableProperty]
	private bool _musicEnabled = true;

	[ObservableProperty]
	private string? _selectedClass = "法师";

	public string[] Classes { get; } = ["战士", "法师", "游侠", "牧师"];

}
