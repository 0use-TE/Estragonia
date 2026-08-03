using Godot;
using HelloWorld.ViewModels;
using HelloWorld.Views;
using JLeb.Estragonia;
using AvControl = Avalonia.Controls.Control;

namespace HelloWorld;

public partial class UserInterface : UiHost {

	protected override AvControl CreateRoot() {
		var vm = new HelloWorldViewModel();
		vm.PaintOuseRequested += () => SetOuseTint(Colors.Red);
		vm.ResetOuseRequested += () => SetOuseTint(Colors.White);
		return new HelloWorldView { DataContext = vm };
	}

	private void SetOuseTint(Color tint) {
		var ouse = GetNodeOrNull<Ouse>("../Ouse");
		if (ouse is null) {
			GD.PrintErr("[HelloWorld] Ouse node not found");
			return;
		}

		ouse.Tint = tint;
		GD.Print($"[HelloWorld] Ouse tint -> {tint}");
	}

}
