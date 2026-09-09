using GodotGame.ViewModels;
using GodotGame.Views;
using JLeb.Estragonia;
using AvControl = Avalonia.Controls.Control;

namespace GodotGame;

/// <summary>Default Avalonia host. Swap the root view / view-model as needed.</summary>
public partial class UserInterface : UiHost {

	protected override AvControl CreateRoot()
		=> new MainView { DataContext = new MainViewModel() };

}
