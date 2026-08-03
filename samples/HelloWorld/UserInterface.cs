using Godot;
using JLeb.Estragonia;
using AvControl = Avalonia.Controls.Control;

namespace HelloWorld;

public partial class UserInterface : UiHost {

	protected override AvControl CreateRoot() {
		var view = new HelloWorldView();
		view.PaintOuseRequested += () => SetOuseTint(Colors.Red);
		view.ResetOuseRequested += () => SetOuseTint(Colors.White);
		return view;
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
