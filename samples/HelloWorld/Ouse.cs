using Godot;

namespace HelloWorld;

/// <summary>
/// Draggable Sprite2D. Listens on <see cref="_UnhandledInput"/> so Avalonia/GUI can swallow first;
/// leftover clicks (no UI under the cursor) reach this node.
/// </summary>
public partial class Ouse : Sprite2D {

	private bool _dragging;
	private Vector2 _grabOffset;

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton)
			OnMouseButton(mouseButton);
	}

	// While dragging, keep receiving moves even if the cursor crosses the Avalonia host.
	public override void _Input(InputEvent @event) {
		if (!_dragging)
			return;

		switch (@event) {
			case InputEventMouseMotion mouseMotion:
				GlobalPosition = mouseMotion.Position + _grabOffset;
				GetViewport().SetInputAsHandled();
				break;
			case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }:
				EndDrag();
				GetViewport().SetInputAsHandled();
				break;
		}
	}

	private void OnMouseButton(InputEventMouseButton mouseButton) {
		if (mouseButton.Pressed) {
			if (!IsMouseOverSprite())
				return;

			_dragging = true;
			_grabOffset = GlobalPosition - mouseButton.Position;
			Modulate = new Color(1.15f, 1.15f, 0.75f);
			GetViewport().SetInputAsHandled();
			GD.Print($"[Ouse] drag start at {GlobalPosition}");
			return;
		}

		if (_dragging)
			EndDrag();
	}

	private void EndDrag() {
		_dragging = false;
		Modulate = Colors.White;
		GD.Print($"[Ouse] drag end at {GlobalPosition}");
	}

	private bool IsMouseOverSprite() {
		if (Texture is null)
			return false;

		return GetRect().HasPoint(ToLocal(GetGlobalMousePosition()));
	}

}
