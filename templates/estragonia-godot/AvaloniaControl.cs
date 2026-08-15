using Godot;
using AvControl = Avalonia.Controls.Control;

namespace JLeb.Estragonia;

/// <summary>
/// Godot <see cref="Control"/> that hosts Avalonia UI.
/// <para>
/// <b>Must live in this Godot C# project</b> (class name = file name). Do not move this type into a class library / NuGet —
/// Godot cannot reliably hot-reload Godot node types from external assemblies.
/// </para>
/// Rendering/input implementation is <see cref="AvaloniaControlEngine"/> inside <c>Ouse.Estragonia</c>.
/// </summary>
public partial class AvaloniaControl : Control {

	private AvaloniaControlEngine? _engine;

	/// <summary>Gets or sets the underlying Avalonia control that will be rendered.</summary>
	public AvControl? Control {
		get => _engine?.Control;
		set {
			EnsureEngine();
			_engine!.Control = value;
		}
	}

	/// <summary>Gets or sets the render scaling for the Avalonia control. Defaults to 1.0.</summary>
	public double RenderScaling {
		get => _engine?.RenderScaling ?? 1.0;
		set {
			EnsureEngine();
			_engine!.RenderScaling = value;
		}
	}

	/// <summary>
	/// Gets or sets whether some Godot UI actions will be automatically mapped to Avalonia key events.
	/// Defaults to true.
	/// </summary>
	public bool AutoConvertUIActionToKeyDown {
		get => _engine?.AutoConvertUIActionToKeyDown ?? true;
		set {
			EnsureEngine();
			_engine!.AutoConvertUIActionToKeyDown = value;
		}
	}

	/// <summary>
	/// When false (default), only Avalonia-hittable pixels capture the mouse; empty areas pass through to Godot.
	/// </summary>
	public bool CaptureEmptyHits {
		get => _engine?.CaptureEmptyHits ?? false;
		set {
			EnsureEngine();
			_engine!.CaptureEmptyHits = value;
		}
	}

	/// <summary>Gets the underlying Avalonia top-level element.</summary>
	public GodotTopLevel GetTopLevel()
		=> EngineOrThrow.GetTopLevel();

	/// <summary>Gets the underlying Godot texture where <see cref="Control"/> is rendered.</summary>
	public Texture2D GetTexture()
		=> EngineOrThrow.GetTexture();

	private AvaloniaControlEngine EngineOrThrow
		=> _engine ?? throw new System.InvalidOperationException($"{nameof(AvaloniaControl)} isn't ready yet.");

	private void EnsureEngine()
		=> _engine ??= new AvaloniaControlEngine(this);

	/// <summary>Drops the Avalonia root and GPU top-level so this host no longer pins view types.</summary>
	public void Detach() {
		if (_engine is null)
			return;

		_engine.Control = null;
		_engine.Dispose();
		_engine = null;
	}

	public override void _Ready() {
		EnsureEngine();
		_engine!.Ready();
	}

	public override void _Notification(int what) {
		// Use notifications instead of Control.Resized / MouseExited C# events.
		// Those become dead ManagedCallables after editor assembly reload.
		switch ((long) what) {
			case NotificationResized:
				_engine?.NotifyResized();
				break;
			case NotificationFocusEnter:
				_engine?.NotifyFocusEntered();
				break;
			case NotificationFocusExit:
				_engine?.NotifyFocusExited();
				break;
			case NotificationMouseExit:
				_engine?.NotifyMouseExited();
				break;
		}

		base._Notification(what);
	}

	public override void _Process(double delta)
		=> _engine?.Process();

	public override void _Draw()
		=> _engine?.Draw();

	public override void _GuiInput(InputEvent @event)
		=> _engine?.GuiInput(@event);

	public override bool _HasPoint(Vector2 point)
		=> _engine?.HasPoint(point) ?? false;

	protected override void Dispose(bool disposing) {
		if (disposing) {
			_engine?.Dispose();
			_engine = null;
		}

		base.Dispose(disposing);
	}

}
