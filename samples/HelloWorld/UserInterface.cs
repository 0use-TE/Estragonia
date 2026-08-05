using Godot;
using HelloWorld.ViewModels;
using HelloWorld.Views;
using JLeb.Estragonia;
using AvControl = Avalonia.Controls.Control;

namespace HelloWorld;

public partial class UserInterface : UiHost {

	private HelloWorldViewModel? _vm;

	protected override AvControl CreateRoot() {
		_vm = new HelloWorldViewModel();
		return new HelloWorldView { DataContext = _vm };
	}

	public override void _Process(double delta) {
		base._Process(delta);
		if (_vm is null)
			return;

		var fps = Engine.GetFramesPerSecond();
		var frameMs = delta > 0 ? delta * 1000.0 : 0;
		_vm.ReportFrame(fps, frameMs);
	}
}
