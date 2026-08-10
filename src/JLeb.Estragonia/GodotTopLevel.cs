using Avalonia.Controls;
using Avalonia.Controls.Embedding;
using Avalonia.Input;

namespace JLeb.Estragonia;

/// <summary>
/// A <see cref="TopLevel"/> used with Godot.
/// Created by the host <c>AvaloniaControl</c> script in your Godot project.
/// </summary>
public sealed class GodotTopLevel : EmbeddableControlRoot {

	public GodotTopLevelImpl Impl { get; }

	static GodotTopLevel()
		// TopLevel has Cycle navigation mode but we want the focus to be able to leave Avalonia to return back to godot: use Continue
		=> KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<GodotTopLevel>(KeyboardNavigationMode.Continue);

	public GodotTopLevel(GodotTopLevelImpl impl)
		: base(impl)
		=> Impl = impl;

}
