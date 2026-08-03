using Avalonia.Controls;
using Godot;
using AvControl = Avalonia.Controls.Control;

namespace JLeb.Estragonia;

/// <summary>
/// Godot-side Avalonia host that owns focus boilerplate.
/// Subclasses only implement <see cref="CreateRoot"/>.
/// </summary>
/// <remarks>
/// Asset loading and IME belong in an Autoload next to <c>UseGodot()</c>, not here.
/// </remarks>
public abstract class UiHost : AvaloniaControl {

	/// <summary>Creates the Avalonia root control for this host.</summary>
	protected abstract AvControl CreateRoot();

	public override void _Ready() {
		FocusMode = FocusModeEnum.All;
		MouseFilter = MouseFilterEnum.Stop;
		Control = CreateRoot();
		base._Ready();
		GrabFocus();
	}

}
