using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Godot;
using JLeb.Estragonia.Input;
using AvControl = Avalonia.Controls.Control;
using GdControl = Godot.Control;
using GdInput = Godot.Input;
using GdKey = Godot.Key;

namespace JLeb.Estragonia;

/// <summary>
/// Avalonia render / input engine used by the Godot-project <c>AvaloniaControl</c> script.
/// Stays in this assembly so it can use Avalonia private platform APIs; the Godot node subclass itself must live in your Godot project.
/// </summary>
public sealed class AvaloniaControlEngine : IDisposable {

	private readonly GdControl _owner;
	private AvControl? _control;
	private double _renderScaling = 1.0;
	private GodotTopLevel? _topLevel;
	private bool _disposed;

	public AvaloniaControlEngine(GdControl owner)
		=> _owner = owner ?? throw new ArgumentNullException(nameof(owner));

	/// <summary>Gets or sets the underlying Avalonia control that will be rendered.</summary>
	public AvControl? Control {
		get => _control;
		set {
			if (ReferenceEquals(_control, value))
				return;

			if (value is not null)
				GodotPlatform.EnsureAssetLoader(value.GetType().Assembly);
			else
				GodotPlatform.EnsureAssetLoader(typeof(EditorAvaloniaApp).Assembly);

			_control = value;

			if (_topLevel is not null)
				_topLevel.Content = _control;
		}
	}

	/// <summary>Gets or sets the render scaling for the Avalonia control. Defaults to 1.0.</summary>
	[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator", Justification = "Doesn't affect correctness")]
	public double RenderScaling {
		get => _renderScaling;
		set {
			if (_renderScaling == value)
				return;

			_renderScaling = value;
			NotifyResized();
			_owner.QueueRedraw();
		}
	}

	/// <summary>
	/// Gets or sets whether some Godot UI actions will be automatically mapped to an <see cref="InputElement.KeyDownEvent"/> event.
	/// The mapped actions are ui_left, ui_right, ui_up, ui_down, ui_accept and ui_cancel.
	/// Defaults to true.
	/// </summary>
	public bool AutoConvertUIActionToKeyDown { get; set; } = true;

	/// <summary>
	/// When false (default), only Avalonia-hittable pixels capture the mouse and empty/transparent
	/// areas pass through to Godot nodes behind. When true, the whole control rect captures input.
	/// </summary>
	public bool CaptureEmptyHits { get; set; }

	/// <summary>Gets the underlying Avalonia top-level element.</summary>
	public GodotTopLevel GetTopLevel()
		=> _topLevel ?? throw new InvalidOperationException($"The {nameof(AvaloniaControlEngine)} isn't initialized");

	/// <summary>Gets the underlying Godot texture where <see cref="Control"/> is rendered.</summary>
	public Texture2D GetTexture()
		=> GetTopLevel().Impl.GetGdTexture();

	public bool IsInitialized
		=> _topLevel is not null && !_disposed;

	public void Ready() {
		if (_disposed)
			return;

		if (_topLevel is not null) {
			NotifyResized();
			return;
		}

		// Game hosts and editor docks share this path. A [Tool] host in the editor
		// must be allowed to create a TopLevel once UseGodot() has run.

		// Skia outputs a premultiplied alpha image, ensure we got the correct blend mode if the user didn't specify any
		_owner.Material ??= new CanvasItemMaterial {
			BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha,
			LightMode = CanvasItemMaterial.LightModeEnum.Unshaded
		};

		var locator = AvaloniaLocator.Current;

		if (locator.GetService<IPlatformGraphics>() is not GodotVkPlatformGraphics graphics) {
			GD.PrintErr("No Godot platform graphics found, did you forget to register your Avalonia app with UseGodot()?");
			return;
		}

		var topLevelImpl = new GodotTopLevelImpl(graphics, locator.GetRequiredService<IClipboard>(), GodotPlatform.Compositor) {
			CursorChanged = OnAvaloniaCursorChanged
		};

		topLevelImpl.SetRenderSize(GetFrameSize(), RenderScaling);

		_topLevel = new GodotTopLevel(topLevelImpl) {
			Background = null,
			Content = Control,
			TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent, WindowTransparencyLevel.None }
		};

		_topLevel.Prepare();
		_topLevel.StartRendering();

		if (_owner.HasFocus())
			NotifyFocusEntered();
	}

	public void NotifyResized() {
		if (_topLevel is null)
			return;

		_topLevel.Impl.SetRenderSize(GetFrameSize(), RenderScaling);
		RenderAvalonia();
	}

	public void NotifyFocusEntered()
		=> OnFocusEntered();

	public void NotifyFocusExited()
		=> _topLevel?.Impl.OnLostFocus();

	public void NotifyMouseExited()
		=> _topLevel?.Impl.OnMouseExited(Time.GetTicksMsec());

	public void Process() {
		GodotPlatform.TriggerRenderTick();

		// We might have cleared the texture after resize to prevent corruption on AMD GPU (see GodotSkiaGpuRenderSession),
		// force a re-render.
		if (_topLevel?.Impl.SurfaceDrawCount <= 2)
			RenderAvalonia();
	}

	public void Draw() {
		if (_topLevel is null)
			return;

		_owner.DrawTexture(_topLevel.Impl.GetGdTexture(), Vector2.Zero);
	}

	public void GuiInput(InputEvent @event) {
		if (_topLevel is null)
			return;

		var handled = TryHandleInput(_topLevel.Impl, @event) || TryHandleAction(@event);

		// Always consume pointer events while the cursor is over this control.
		// Avalonia often leaves RawPointerEventArgs.Handled == false; without AcceptEvent,
		// Godot can let the 3D viewport steal the mouse after Button press.
		if (handled
			|| @event is InputEventMouseButton
			|| @event is InputEventMouseMotion
			|| @event is InputEventScreenTouch
			|| @event is InputEventScreenDrag) {
			_owner.AcceptEvent();
		}
	}

	public bool HasPoint(Vector2 point) {
		// Godot may call _HasPoint with any local point (not pre-clipped to Size).
		// CaptureEmptyHits must mean "empty pixels inside this control", not the whole editor.
		var size = _owner.Size;
		if (point.X < 0f || point.Y < 0f || point.X > size.X || point.Y > size.Y)
			return false;

		if (_topLevel is null)
			return CaptureEmptyHits;

		var avaloniaPoint = point.ToAvaloniaPoint() / _topLevel.RenderScaling;
		if (_topLevel.InputHitTest(avaloniaPoint, false) is not null)
			return true;

		return CaptureEmptyHits;
	}

	private PixelSize GetFrameSize()
		=> PixelSize.FromSize(_owner.Size.ToAvaloniaSize(), 1.0);

	private void RenderAvalonia()
		=> _topLevel!.Impl.OnDraw(new Rect(_owner.Size.ToAvaloniaSize()));

	private bool _hasCustomMouseCursor;

	private void OnAvaloniaCursorChanged(ICursorImpl? cursor) {
		if (cursor is GodotCustomCursorImpl custom) {
			GdInput.SetCustomMouseCursor(custom.Texture, GdInput.CursorShape.Arrow, custom.Hotspot);
			_hasCustomMouseCursor = true;
			_owner.MouseDefaultCursorShape = GdControl.CursorShape.Arrow;
			return;
		}

		if (_hasCustomMouseCursor) {
			GdInput.SetCustomMouseCursor(null);
			_hasCustomMouseCursor = false;
		}

		_owner.MouseDefaultCursorShape =
			(cursor as GodotStandardCursorImpl)?.CursorShape ?? GdControl.CursorShape.Arrow;
	}

	private void OnFocusEntered() {
		if (_topLevel is null)
			return;

		_topLevel.Focus();

		if (KeyboardNavigationHandler.GetNext(_topLevel, NavigationDirection.Next) is not { } inputElement)
			return;

		NavigationMethod navigationMethod;

		if (GdInput.IsActionPressed(GodotBuiltInActions.UIFocusNext) || GdInput.IsActionPressed(GodotBuiltInActions.UIFocusPrev))
			navigationMethod = NavigationMethod.Tab;
		else if (GdInput.GetMouseButtonMask() != 0)
			navigationMethod = NavigationMethod.Pointer;
		else
			navigationMethod = NavigationMethod.Unspecified;

		inputElement.Focus(navigationMethod);
	}

	private bool TryHandleAction(InputEvent inputEvent) {
		if (!inputEvent.IsActionType())
			return false;

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIFocusNext, true, true))
			return TryMoveFocus(NavigationDirection.Next, inputEvent);

		if (inputEvent.IsActionPressed(GodotBuiltInActions.UIFocusPrev, true, true))
			return TryMoveFocus(NavigationDirection.Previous, inputEvent);

		if (AutoConvertUIActionToKeyDown) {

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UILeft, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Left);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UIRight, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Right);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UIUp, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Up);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UIDown, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Down);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UIAccept, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Enter);

			if (inputEvent.IsActionPressed(GodotBuiltInActions.UICancel, true, true))
				return SimulateKeyDownFromAction(inputEvent, GdKey.Escape);

		}

		return false;
	}

	private bool SimulateKeyDownFromAction(InputEvent inputEvent, GdKey key) {
		// if the action already matches the key we're going to simulate, abort: it already got through TryHandleInput and wasn't handled
		if (inputEvent is InputEventKey inputEventKey && inputEventKey.Keycode == key)
			return false;

		if (_topLevel?.FocusManager?.GetFocusedElement() is not { } currentElement)
			return false;

		var args = new KeyEventArgs {
			RoutedEvent = InputElement.KeyDownEvent,
			Key = key.ToAvaloniaKey(),
			KeyModifiers = inputEvent.GetKeyModifiers()
		};
		currentElement.RaiseEvent(args);
		return args.Handled;
	}

	private static bool TryHandleInput(GodotTopLevelImpl impl, InputEvent inputEvent)
		=> inputEvent switch {
			InputEventMouseMotion mouseMotion => impl.OnMouseMotion(mouseMotion, Time.GetTicksMsec()),
			InputEventMouseButton mouseButton => impl.OnMouseButton(mouseButton, Time.GetTicksMsec()),
			InputEventScreenTouch screenTouch => impl.OnScreenTouch(screenTouch, Time.GetTicksMsec()),
			InputEventScreenDrag screenDrag => impl.OnScreenDrag(screenDrag, Time.GetTicksMsec()),
			InputEventKey key => impl.OnKey(key, Time.GetTicksMsec()),
			InputEventJoypadButton joypadButton => impl.OnJoypadButton(joypadButton, Time.GetTicksMsec()),
			InputEventJoypadMotion joypadMotion => impl.OnJoypadMotion(joypadMotion, Time.GetTicksMsec()),
			_ => false
		};

	private bool TryMoveFocus(NavigationDirection direction, InputEvent inputEvent) {
		if (_topLevel?.FocusManager is not { } focusManager)
			return false;

		var currentElement = focusManager.GetFocusedElement() ?? _topLevel;

		// GodotTopLevel has a Continue tab navigation since we want to be able to focus the Godot controls
		// once we're done with the Avalonia ones. However, if there's no Godot control, we want to act as Cycle.
		var nextElement = GetNextTabElement(currentElement, direction);
		if (nextElement is null) {
			var nextGdControl = direction switch {
				NavigationDirection.Next => _owner.FindNextValidFocus(),
				NavigationDirection.Previous => _owner.FindPrevValidFocus(),
				_ => null
			};

			if ((nextGdControl is null || nextGdControl == _owner) && (object) currentElement != _topLevel)
				nextElement = GetNextTabElement(_topLevel, direction);
		}


		if (nextElement is null)
			return false;

		nextElement.Focus(NavigationMethod.Tab, inputEvent.GetKeyModifiers());
		return true;
	}

	private static IInputElement? GetNextTabElement(IInputElement element, NavigationDirection direction) {
		var previous = element;

		while (true) {
			// GetNext doesn't take IsEffectivelyEnabled into account, check it manually
			var next = KeyboardNavigationHandler.GetNext(previous, direction);
			if (next is null || next.IsEffectivelyEnabled)
				return next;

			// handle potential all-disabled cycle
			if (next == element)
				return null;

			previous = next;
		}
	}

	public void Dispose() {
		if (_disposed)
			return;

		_disposed = true;

		if (_topLevel is not null) {
			_topLevel.Dispose();
			_topLevel = null;
		}
	}

}
