using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Avalonia;
using Godot;
using LinqExpression = System.Linq.Expressions.Expression;

namespace JLeb.Estragonia;

/// <summary>Public helpers for hosting Avalonia inside Godot.</summary>
public static class GodotAvalonia {

	private static bool s_started;
	private static bool s_alcHooked;
	private static bool s_buildHooked;
	private static int s_unloading;
	private static EventInfo? s_buildEvent;
	private static Delegate? s_buildHandler;

	static GodotAvalonia()
		=> TryHookAlcUnloading();

	/// <summary>
	/// Whether <see cref="AppBuilderExtensions.UseGodot"/> has completed for the current session.
	/// Becomes false after <see cref="Shutdown"/>.
	/// </summary>
	public static bool IsStarted
		=> s_started;

	/// <summary>
	/// True after <see cref="PrepareForUnload"/> has started tearing Avalonia down so Godot can
	/// unload the collectible ALC. Hosts must not recreate a TopLevel while this is set.
	/// </summary>
	public static bool IsUnloading
		=> Volatile.Read(ref s_unloading) != 0;

	/// <summary>
	/// Subscribes to Godot C# rebuild / ALC unload without starting Avalonia.
	/// Call from an editor Autoload so the first Build tears down any later <see cref="EnsureStarted"/>.
	/// </summary>
	public static void HookReload() {
		TryHookAlcUnloading();
		TryHookGodotBuild();
	}

	/// <summary>
	/// Starts Avalonia with <see cref="EditorAvaloniaApp"/> (this assembly). Prefer this in the editor
	/// so the game project assembly is not the <see cref="Application"/> type.
	/// </summary>
	public static void EnsureStarted()
		=> EnsureStarted<EditorAvaloniaApp>();

	/// <summary>
	/// Starts Avalonia on the Godot platform. No-op if already started.
	/// Call <see cref="Shutdown"/> before starting again in the same process.
	/// </summary>
	public static void EnsureStarted<TApp>()
		where TApp : Application, new() {
		if (s_started || IsUnloading)
			return;

		if (RenderingServer.GetRenderingDevice() is null)
			throw new NotSupportedException("Estragonia requires a Vulkan renderer (Forward+ or Mobile).");

		TryHookAlcUnloading();
		TryHookGodotBuild();

		AppBuilder
			.Configure<TApp>()
			.UseGodot()
			.SetupWithoutStarting();

		EnsureAssetLoader(typeof(TApp).Assembly);
		s_started = true;
		GD.Print("Estragonia: Avalonia started");
	}

	/// <summary>
	/// Tears down Avalonia platform services so a later <see cref="EnsureStarted{TApp}"/> can run again.
	/// Hosts must <c>Detach</c> first. The UI dispatcher object is kept (Avalonia cannot recreate it);
	/// its thread-pool timer is disposed so Godot can unload the collectible ALC.
	/// </summary>
	public static void Shutdown() {
		if (!s_started)
			return;

		try {
			GodotPlatform.Reset();
		}
		catch (Exception ex) {
			GD.PrintErr($"Estragonia: platform reset failed: {ex.Message}");
		}

		ResetAppBuilderSetupFlag();
		s_started = false;
		GD.Print("Estragonia: Avalonia shut down");
	}

	/// <summary>
	/// Drops every live <see cref="AvaloniaControlEngine"/> and shuts Avalonia down.
	/// Godot Build does not call plugin <c>_ExitTree</c>, so this must run from
	/// GodotTools <c>BuildStarted</c> and from the collectible ALC <c>Unloading</c> event.
	/// Without it, Avalonia's locator / GPU / thread-pool timer pin the old assembly and
	/// Godot reports a hot-reload failure after every XAML or C# rebuild.
	/// </summary>
	public static void PrepareForUnload() {
		if (Interlocked.Exchange(ref s_unloading, 1) != 0)
			return;

		AvaloniaControlEngine.DisposeAll();
		Shutdown();
		UnhookGodotBuild();

		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.Collect();
	}

	/// <summary>
	/// Avalonia allows <c>SetupWithoutStarting</c> only once per process unless this flag is cleared.
	/// </summary>
	private static void ResetAppBuilderSetupFlag() {
		var field = typeof(AppBuilder).GetField(
			"s_setupWasAlreadyCalled",
			BindingFlags.Static | BindingFlags.NonPublic);

		if (field is null) {
			GD.PrintErr("Estragonia: could not reset AppBuilder setup flag; a later EnsureStarted may fail.");
			return;
		}

		field.SetValue(null, false);
	}

	/// <summary>
	/// Godot Build does not call plugin <c>_ExitTree</c> before unloading the ALC.
	/// Hook the collectible context so we can drop thread-pool / GPU roots first.
	/// </summary>
	private static void TryHookAlcUnloading() {
		if (s_alcHooked)
			return;

		var alc = AssemblyLoadContext.GetLoadContext(typeof(GodotAvalonia).Assembly);
		if (alc is null || alc == AssemblyLoadContext.Default)
			return;

		alc.Unloading += static _ => {
			GD.Print("Estragonia: ALC unloading");
			PrepareForUnload();
		};
		s_alcHooked = true;
	}

	/// <summary>
	/// Subscribe to GodotTools build-start so Avalonia is torn down <i>before</i> MSBuild runs.
	/// The handler is removed in <see cref="PrepareForUnload"/>; leaving it subscribed would
	/// itself pin the collectible ALC (GodotTools lives outside that context).
	/// </summary>
	private static void TryHookGodotBuild() {
		if (s_buildHooked || !Engine.IsEditorHint())
			return;

		s_buildHooked = true;

		try {
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				if (assembly.GetName().Name != "GodotTools")
					continue;

				var type = assembly.GetType("GodotTools.Build.BuildManager");
				if (type is null)
					return;

				foreach (var eventName in new[] { "BuildStarted", "BuildLaunching" }) {
					var evt = type.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static);
					if (evt?.EventHandlerType is null)
						continue;

					var handler = BindUnloadHandler(evt.EventHandlerType);
					evt.AddEventHandler(null, handler);
					s_buildEvent = evt;
					s_buildHandler = handler;
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

	private static void UnhookGodotBuild() {
		try {
			if (s_buildEvent is not null && s_buildHandler is not null)
				s_buildEvent.RemoveEventHandler(null, s_buildHandler);
		}
		catch (Exception ex) {
			GD.PrintErr($"Estragonia: could not unhook GodotTools build: {ex.Message}");
		}

		s_buildEvent = null;
		s_buildHandler = null;
		s_buildHooked = false;
	}

	private static Delegate BindUnloadHandler(Type handlerType) {
		var invoke = handlerType.GetMethod("Invoke")
			?? throw new InvalidOperationException(handlerType.FullName);
		var parameters = Array.ConvertAll(
			invoke.GetParameters(),
			static p => LinqExpression.Parameter(p.ParameterType, p.Name));
		var body = LinqExpression.Call(
			typeof(GodotAvalonia).GetMethod(nameof(PrepareForUnload), BindingFlags.Public | BindingFlags.Static)!);
		return LinqExpression.Lambda(handlerType, body, parameters).Compile();
	}

	/// <summary>
	/// Ensures Avalonia can load <c>avares</c> assets and XAML image sources.
	/// Call this after <c>UseGodot().SetupWithoutStarting()</c> and before creating views that load assets.
	/// </summary>
	public static void EnsureAssetLoader(Assembly? defaultAssembly = null)
		=> GodotPlatform.EnsureAssetLoader(defaultAssembly);

}
