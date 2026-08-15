using System;
using System.Reflection;
using Avalonia;
using Godot;

namespace JLeb.Estragonia;

/// <summary>Public helpers for hosting Avalonia inside Godot.</summary>
public static class GodotAvalonia {

	private static bool s_started;

	/// <summary>
	/// Whether <see cref="AppBuilderExtensions.UseGodot"/> has completed for this process.
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
	/// One process, one <see cref="Application"/>.
	/// </summary>
	public static void EnsureStarted<TApp>()
		where TApp : Application, new() {
		if (s_started)
			return;

		if (RenderingServer.GetRenderingDevice() is null)
			throw new NotSupportedException("Estragonia requires a Vulkan renderer (Forward+ or Mobile).");

		AppBuilder
			.Configure<TApp>()
			.UseGodot()
			.SetupWithoutStarting();

		EnsureAssetLoader(typeof(TApp).Assembly);
		s_started = true;
	}

	/// <summary>
	/// Ensures Avalonia can load <c>avares</c> assets and XAML image sources.
	/// Call this after <c>UseGodot().SetupWithoutStarting()</c> and before creating views that load assets.
	/// </summary>
	public static void EnsureAssetLoader(Assembly? defaultAssembly = null)
		=> GodotPlatform.EnsureAssetLoader(defaultAssembly);

}
