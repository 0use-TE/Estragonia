using JLeb.Estragonia;
using AvControl = Avalonia.Controls.Control;

namespace HelloWorld;

public partial class UserInterface : UiHost {

	protected override AvControl CreateRoot()
		=> new HelloWorldView();

}
