using Godot;
using JLeb.Estragonia;

namespace HelloWorld;

/// <summary>GDScript-callable play-mode startup. Godot 4 cannot call C# static methods from GDScript.</summary>
[GlobalClass]
public partial class AvaloniaGame : GodotObject {

	public void Start()
		=> GodotAvalonia.EnsureStarted<App>();

}
