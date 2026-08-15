#if TOOLS
using Godot;
using JLeb.Estragonia;

namespace HelloWorld.Editor;

/// <summary>
/// C# Control that hosts Avalonia in the editor. Godot only reliably calls
/// overridden engine methods (<c>_Draw</c>, <c>_Process</c>, …) on C# scripts;
/// GDScript cannot invoke custom C# methods on a preloaded CSharpScript.
/// </summary>
[Tool]
public partial class AvaloniaEditorHost : Control {

	public override void _Ready() {
		SizeFlagsHorizontal = SizeFlags.ExpandFill;
		SizeFlagsVertical = SizeFlags.ExpandFill;
		SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
		ClipContents = true;
		FocusMode = FocusModeEnum.All;
		MouseFilter = MouseFilterEnum.Stop;
		SetProcess(true);

		var viewType = GetMeta("estragonia_view_type", "").AsString();
		if (!string.IsNullOrEmpty(viewType))
			AvaloniaEditorRuntime.Attach(this, viewType);
	}

	public override void _ExitTree()
		=> AvaloniaEditorRuntime.Detach(this);

	public override void _Process(double delta)
		=> AvaloniaEditorRuntime.Process(this);

	public override void _Draw()
		=> AvaloniaEditorRuntime.Draw(this);

	public override void _GuiInput(InputEvent @event)
		=> AvaloniaEditorRuntime.GuiInput(this, @event);

	public override bool _HasPoint(Vector2 point)
		=> AvaloniaEditorRuntime.HasPoint(this, point);

	public override void _Notification(int what) {
		AvaloniaEditorRuntime.Notification(this, what);
		base._Notification(what);
	}

}
#endif
