using System;
using System.Reflection;
using System.Runtime.Loader;
using Avalonia;
using Godot;

namespace JLeb.Estragonia;

/// <summary>Public helpers for hosting Avalonia inside Godot.</summary>
public static class GodotAvalonia {

	private static bool s_started;
	private static bool s_alcHooked;

	static GodotAvalonia()
		=> TryHookAlcUnloading();

	/// <summary>
	/// Whether <see cref="AppBuilderExtensions.UseGodot"/> has completed for the current session.
	/// Becomes false after <see cref="Shutdown"/>.
	/// </summary>
	public static bool IsStarted
		=> s_started;

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
		if (s_started)
			return;

		if (RenderingServer.GetRenderingDevice() is null)
			throw new NotSupportedException("Estragonia requires a Vulkan renderer (Forward+ or Mobile).");

		TryHookAlcUnloading();

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
			AvaloniaEditorRuntime.PrepareForUnload();
		};
		s_alcHooked = true;
	}

	/// <summary>
	/// Ensures Avalonia can load <c>avares</c> assets and XAML image sources.
	/// Call this after <c>UseGodot().SetupWithoutStarting()</c> and before creating views that load assets.
	/// </summary>
	public static void EnsureAssetLoader(Assembly? defaultAssembly = null)
		=> GodotPlatform.EnsureAssetLoader(defaultAssembly);

}
