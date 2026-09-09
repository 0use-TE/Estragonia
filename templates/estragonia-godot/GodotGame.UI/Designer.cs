using System;
using Avalonia;

namespace GodotGame.UI;

/// <summary>
/// Avalonia designer / previewer host for the UI project.
/// Provides Main (Debug Exe entry point) and BuildAvaloniaApp.
/// Do not start this project as a console app — open project.godot in Godot to run the game.
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
