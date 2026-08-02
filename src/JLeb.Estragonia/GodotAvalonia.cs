using System.Reflection;

namespace JLeb.Estragonia;

/// <summary>Public helpers for hosting Avalonia inside Godot.</summary>
public static class GodotAvalonia {

	/// <summary>
	/// Ensures Avalonia can load <c>avares</c> assets and XAML image sources.
	/// Call this after <c>UseGodot().SetupWithoutStarting()</c> and before creating views that load assets.
	/// </summary>
	public static void EnsureAssetLoader(Assembly? defaultAssembly = null)
		=> GodotPlatform.EnsureAssetLoader(defaultAssembly);

}
