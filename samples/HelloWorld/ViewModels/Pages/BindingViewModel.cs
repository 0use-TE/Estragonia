using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloWorld.ViewModels.Pages;

public sealed partial class BindingViewModel : ObservableObject {

	[ObservableProperty]
	private string _newItem = "";

	[ObservableProperty]
	private string? _selectedItem;

	public ObservableCollection<string> Items { get; } = ["药水", "铁剑", "皮甲", "传送卷轴"];

	[RelayCommand]
	private void Add() {
		var text = NewItem.Trim();
		if (text.Length == 0)
			return;
		Items.Add(text);
		NewItem = "";
		SelectedItem = text;
	}

	[RelayCommand]
	private void RemoveSelected() {
		if (SelectedItem is null)
			return;
		Items.Remove(SelectedItem);
		SelectedItem = null;
	}

}
