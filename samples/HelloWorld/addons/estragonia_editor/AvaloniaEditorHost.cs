#if TOOLS
using System;
using Godot;
using AvControl = Avalonia.Controls.Control;

namespace JLeb.Estragonia;

/// <summary>
/// Godot dock host for Avalonia. Must live in this Godot project (class name = file name).
/// Each dock has its own host / TopLevel; the Avalonia <c>Application</c> is process-wide.
/// </summary>
[Tool]
public partial class AvaloniaEditorHost : AvaloniaControl {

	/// <summary>Creates the Avalonia root when this host enters the editor tree.</summary>
	public Func<AvControl>? CreateRoot { get; set; }

	public override void _Ready() {
		SizeFlagsHorizontal = SizeFlags.ExpandFill;
		SizeFlagsVertical = SizeFlags.ExpandFill;
		SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
		ClipContents = true;
		CaptureEmptyHits = true;
		FocusMode = FocusModeEnum.All;
		MouseFilter = MouseFilterEnum.Stop;
		Control ??= CreateRoot?.Invoke();
		base._Ready();
	}

	public override void _Process(double delta) {
		if (Control is null)
			Control = CreateRoot?.Invoke();

		if (Size.X > 1f && Size.Y > 1f)
			base._Ready();

		base._Process(delta);
		QueueRedraw();
	}

}
#endif
