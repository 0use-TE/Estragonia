using Godot;

namespace HelloWorld;

/// <summary>
/// Draggable Sprite2D. Avalonia can change <see cref="Tint"/>; drag highlight is temporary.
/// </summary>
public partial class Ouse : Sprite2D {

	private bool _dragging;
	private Vector2 _grabOffset;
	private Color _tint = Colors.White;

	/// <summary>Persistent color applied when not dragging.</summary>
	public Color Tint {
		get => _tint;
		set {
			_tint = value;
			if (!_dragging)
				Modulate = value;
		}
	}

	public override void _UnhandledInput(InputEvent @event) {
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton)
			OnMouseButton(mouseButton);
	}

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
			Modulate = _tint * new Color(1.15f, 1.15f, 0.75f);
			GetViewport().SetInputAsHandled();
			return;
		}

		if (_dragging)
			EndDrag();
	}

	private void EndDrag() {
		_dragging = false;
		Modulate = _tint;
	}

	private bool IsMouseOverSprite() {
		if (Texture is null)
			return false;

		return GetRect().HasPoint(ToLocal(GetGlobalMousePosition()));
	}

}
