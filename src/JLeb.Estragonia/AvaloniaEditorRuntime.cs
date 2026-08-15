using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Godot;
using AvControl = Avalonia.Controls.Control;
using GdControl = Godot.Control;
using LinqExpression = System.Linq.Expressions.Expression;

namespace JLeb.Estragonia;

/// <summary>
/// Editor Avalonia lifetime used by GDScript plugin shells.
/// Host nodes must be plain Godot <see cref="GdControl"/>s (GDScript), not C# scripts,
/// so the game assembly is not pinned by live Godot C# objects.
/// </summary>
public static class AvaloniaEditorRuntime {

	private static int s_users;
	private static bool s_buildHooked;
	private static readonly Dictionary<ulong, HostState> s_hosts = [];

	/// <inheritdoc cref="GodotAvalonia.IsStarted"/>
	public static bool IsStarted
		=> GodotAvalonia.IsStarted;

	/// <summary>Starts Avalonia if needed and increments the editor host count.</summary>
	public static void EnsureStarted() {
		TryHookGodotBuild();
		GodotAvalonia.EnsureStarted();
		Interlocked.Increment(ref s_users);
	}

	/// <summary>
	/// Decrements the editor host count. When it reaches zero, Avalonia is shut down.
	/// </summary>
	public static void Release() {
		if (Interlocked.Decrement(ref s_users) > 0)
			return;

		Interlocked.Exchange(ref s_users, 0);
		GodotAvalonia.Shutdown();
	}

	/// <summary>
	/// Attaches an Avalonia view (created by type name) to a GDScript host <see cref="GdControl"/>.
	/// </summary>
	public static void Attach(GdControl host, string viewTypeName) {
		ArgumentNullException.ThrowIfNull(host);
		if (string.IsNullOrWhiteSpace(viewTypeName))
			throw new ArgumentException("View type name is required.", nameof(viewTypeName));

		var viewType = ResolveViewType(viewTypeName)
			?? throw new InvalidOperationException($"Avalonia view type not found: {viewTypeName}");
		if (!typeof(AvControl).IsAssignableFrom(viewType))
			throw new InvalidOperationException($"{viewType.FullName} is not an Avalonia Control.");

		var id = host.GetInstanceId();
		lock (s_hosts) {
			if (s_hosts.TryGetValue(id, out var existing)) {
				existing.Engine.Control = CreateView(viewType);
				return;
			}
		}

		EnsureStarted();
		var engine = new AvaloniaControlEngine(host) {
			CaptureEmptyHits = true,
			Control = CreateView(viewType)
		};

		lock (s_hosts)
			s_hosts[id] = new HostState(engine);
	}

	/// <summary>Drops the Avalonia tree for <paramref name="host"/> and releases the runtime refcount.</summary>
	public static void Detach(GdControl host) {
		ArgumentNullException.ThrowIfNull(host);

		HostState? state;
		lock (s_hosts) {
			if (!s_hosts.Remove(host.GetInstanceId(), out state))
				return;
		}

		DisposeState(state);
		Release();
	}

	public static void Process(GdControl host) {
		if (!TryGet(host, out var state))
			return;

		if (host.Size.X > 1f && host.Size.Y > 1f)
			state.Engine.Ready();

		state.Engine.Process();
		host.QueueRedraw();
	}

	public static void Draw(GdControl host) {
		if (TryGet(host, out var state))
			state.Engine.Draw();
	}

	public static void GuiInput(GdControl host, InputEvent @event) {
		if (TryGet(host, out var state))
			state.Engine.GuiInput(@event);
	}

	public static bool HasPoint(GdControl host, Vector2 point)
		=> TryGet(host, out var state) && state.Engine.HasPoint(point);

	public static void Notification(GdControl host, int what) {
		if (!TryGet(host, out var state))
			return;

		switch ((long) what) {
			case GdControl.NotificationResized:
				state.Engine.NotifyResized();
				break;
			case GdControl.NotificationFocusEnter:
				state.Engine.NotifyFocusEntered();
				break;
			case GdControl.NotificationFocusExit:
				state.Engine.NotifyFocusExited();
				break;
			case GdControl.NotificationMouseExit:
				state.Engine.NotifyMouseExited();
				break;
		}
	}

	/// <summary>
	/// Detaches every editor host and shuts Avalonia down. Called from ALC <c>Unloading</c>
	/// and from GodotTools build-start (plugin <c>_ExitTree</c> does not run on Build).
	/// </summary>
	public static void PrepareForUnload() {
		Interlocked.Exchange(ref s_users, 0);

		HostState[] states;
		lock (s_hosts) {
			states = new HostState[s_hosts.Count];
			s_hosts.Values.CopyTo(states, 0);
			s_hosts.Clear();
		}

		foreach (var state in states)
			DisposeState(state);

		GodotAvalonia.Shutdown();

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}

	private static bool TryGet(GdControl host, out HostState state) {
		lock (s_hosts)
			return s_hosts.TryGetValue(host.GetInstanceId(), out state!);
	}

	private static void DisposeState(HostState state) {
		try {
			state.Engine.Control = null;
			state.Engine.Dispose();
		}
		catch (Exception ex) {
			GD.PrintErr($"Estragonia: host detach failed: {ex.Message}");
		}
	}

	private static AvControl CreateView(Type viewType)
		=> (AvControl) Activator.CreateInstance(viewType)!;

	private static Type? ResolveViewType(string viewTypeName) {
		var type = Type.GetType(viewTypeName, throwOnError: false, ignoreCase: false);
		if (type is not null)
			return type;

		var simpleName = viewTypeName.Split(',')[0].Trim();
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
			type = assembly.GetType(simpleName);
			if (type is not null)
				return type;
		}

		return null;
	}

	private static void TryHookGodotBuild() {
		if (s_buildHooked)
			return;

		s_buildHooked = true;

		try {
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GetName().Name != "GodotTools")
					continue;

				var type = assembly.GetType("GodotTools.Build.BuildManager");
				if (type is null)
					return;

				foreach (var eventName in new[] { "BuildLaunching", "BuildStarted" }) {
					var evt = type.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static);
					if (evt?.EventHandlerType is null)
						continue;

					evt.AddEventHandler(null, BindUnloadHandler(evt.EventHandlerType));
					GD.Print($"Estragonia: hooked GodotTools.{eventName}");
					return;
				}

				return;
			}
		}
		catch (Exception ex) {
			GD.PrintErr($"Estragonia: could not hook GodotTools build: {ex.Message}");
		}
	}

	private static Delegate BindUnloadHandler(Type handlerType) {
		var invoke = handlerType.GetMethod("Invoke")
			?? throw new InvalidOperationException(handlerType.FullName);
		var parameters = Array.ConvertAll(
			invoke.GetParameters(),
			static p => LinqExpression.Parameter(p.ParameterType, p.Name));
		var body = LinqExpression.Call(
			typeof(AvaloniaEditorRuntime).GetMethod(nameof(PrepareForUnload), BindingFlags.Public | BindingFlags.Static)!);
		return LinqExpression.Lambda(handlerType, body, parameters).Compile();
	}

	private sealed class HostState(AvaloniaControlEngine engine) {

		public AvaloniaControlEngine Engine { get; } = engine;

	}

}
