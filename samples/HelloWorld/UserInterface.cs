using Godot;
using JLeb.Estragonia;

namespace HelloWorld;

public partial class UserInterface : AvaloniaControl {

	public override void _Ready() {
		GetWindow().SetImeActive(true);
		MouseFilter = MouseFilterEnum.Stop;
		GrabFocus();

		GodotAvalonia.EnsureAssetLoader(typeof(App).Assembly);
		Control = new HelloWorldView();

		base._Ready();
	}

	public override void _Process(double delta) {

		base._Process(delta);
	}

}
