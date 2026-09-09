using System;
using Avalonia;

namespace GodotGame;

/// <summary>
/// Avalonia designer / previewer host.
/// Provides Main (Debug Exe entry point) and BuildAvaloniaApp.
/// Do not run this assembly as a standalone app — open project.godot in Godot instead.
/// </summary>
internal static class Designer {

	public static int Main(string[] args)
		=> throw new NotSupportedException(
			"This project runs inside Godot. Use the Avalonia previewer, or open project.godot in Godot.");

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder
			.Configure<App>()
			.UseSkia()
			.UseHarfBuzz();

}
