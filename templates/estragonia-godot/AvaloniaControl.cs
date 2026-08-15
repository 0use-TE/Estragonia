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
		get => LiveEngine?.Control;
		set {
			EnsureEngine();
			if (LiveEngine is { } engine)
				engine.Control = value;
		}
	}

	/// <summary>Gets or sets the render scaling for the Avalonia control. Defaults to 1.0.</summary>
	public double RenderScaling {
		get => LiveEngine?.RenderScaling ?? 1.0;
		set {
			EnsureEngine();
			if (LiveEngine is { } engine)
				engine.RenderScaling = value;
		}
	}

	/// <summary>
	/// Gets or sets whether some Godot UI actions will be automatically mapped to Avalonia key events.
	/// Defaults to true.
	/// </summary>
	public bool AutoConvertUIActionToKeyDown {
		get => LiveEngine?.AutoConvertUIActionToKeyDown ?? true;
		set {
			EnsureEngine();
			if (LiveEngine is { } engine)
				engine.AutoConvertUIActionToKeyDown = value;
		}
	}

	/// <summary>
	/// When false (default), only Avalonia-hittable pixels capture the mouse; empty areas pass through to Godot.
	/// </summary>
	public bool CaptureEmptyHits {
		get => LiveEngine?.CaptureEmptyHits ?? false;
		set {
			EnsureEngine();
			if (LiveEngine is { } engine)
				engine.CaptureEmptyHits = value;
		}
	}

	/// <summary>Gets the underlying Avalonia top-level element.</summary>
	public GodotTopLevel GetTopLevel()
		=> EngineOrThrow.GetTopLevel();

	/// <summary>Gets the underlying Godot texture where <see cref="Control"/> is rendered.</summary>
	public Texture2D GetTexture()
		=> EngineOrThrow.GetTexture();

	/// <summary>Live engine, or <c>null</c> after <see cref="GodotAvalonia.PrepareForUnload"/> disposed it.</summary>
	protected AvaloniaControlEngine? LiveEngine
		=> _engine is { IsDisposed: false } engine ? engine : ClearDeadEngine();

	/// <summary>False while Godot is compiling / unloading the collectible ALC.</summary>
	protected static bool CanCreateEngine
		=> GodotAvalonia.IsStarted && !GodotAvalonia.IsUnloading;

	private AvaloniaControlEngine EngineOrThrow
		=> LiveEngine ?? throw new System.InvalidOperationException($"{nameof(AvaloniaControl)} isn't ready yet.");

	private AvaloniaControlEngine? ClearDeadEngine() {
		_engine = null;
		return null;
	}

	private void EnsureEngine() {
		if (LiveEngine is not null)
			return;
		if (!CanCreateEngine)
			return;

		_engine = new AvaloniaControlEngine(this);
	}

	/// <summary>Drops the Avalonia root and GPU top-level so this host no longer pins view types.</summary>
	public void Detach() {
		if (_engine is null)
			return;

		_engine.Control = null;
		_engine.Dispose();
		_engine = null;
	}

	public override void _Ready() {
		if (!CanCreateEngine)
			return;

		EnsureEngine();
		LiveEngine?.Ready();
	}

	public override void _Notification(int what) {
		// Use notifications instead of Control.Resized / MouseExited C# events.
		// Those become dead ManagedCallables after editor assembly reload.
		var engine = LiveEngine;
		switch ((long) what) {
			case NotificationResized:
				engine?.NotifyResized();
				break;
			case NotificationFocusEnter:
				engine?.NotifyFocusEntered();
				break;
			case NotificationFocusExit:
				engine?.NotifyFocusExited();
				break;
			case NotificationMouseExit:
				engine?.NotifyMouseExited();
				break;
		}

		base._Notification(what);
	}

	public override void _Process(double delta)
		=> LiveEngine?.Process();

	public override void _Draw()
		=> LiveEngine?.Draw();

	public override void _GuiInput(InputEvent @event)
		=> LiveEngine?.GuiInput(@event);

	public override bool _HasPoint(Vector2 point)
		=> LiveEngine?.HasPoint(point) ?? false;

	protected override void Dispose(bool disposing) {
		if (disposing) {
			_engine?.Dispose();
			_engine = null;
		}

		base.Dispose(disposing);
	}

}
