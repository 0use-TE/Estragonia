using System;
using AvControl = Avalonia.Controls.Control;

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

	/// <summary>
	/// Creates an Avalonia view by type name. Used by editor hosts after a C# reload,
	/// when the original <c>CreateRoot</c> factory is gone but Godot metadata survived.
	/// </summary>
	public static AvControl CreateView(string viewTypeName) {
		ArgumentException.ThrowIfNullOrWhiteSpace(viewTypeName);

		var type = ResolveViewType(viewTypeName)
			?? throw new InvalidOperationException($"Avalonia view type not found: {viewTypeName}");
		if (!typeof(AvControl).IsAssignableFrom(type))
			throw new InvalidOperationException($"{type.FullName} is not an Avalonia Control.");

		return (AvControl) Activator.CreateInstance(type)!;
	}

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

}
