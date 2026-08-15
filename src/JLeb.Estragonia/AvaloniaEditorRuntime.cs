namespace JLeb.Estragonia;

/// <summary>
/// Editor-wide Avalonia lifetime. First caller starts the process-wide
/// <see cref="Avalonia.Application"/>; later callers are a no-op.
/// </summary>
public static class AvaloniaEditorRuntime {

	/// <inheritdoc cref="GodotAvalonia.IsStarted"/>
	public static bool IsStarted
		=> GodotAvalonia.IsStarted;

	/// <summary>Starts Avalonia with <see cref="EditorAvaloniaApp"/> if needed.</summary>
	public static void EnsureStarted()
		=> GodotAvalonia.EnsureStarted();

}
