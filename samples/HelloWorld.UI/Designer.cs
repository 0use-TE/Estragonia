using System;
using Avalonia;

namespace HelloWorld.UI;

/// <summary>
/// Avalonia designer / previewer host for the UI project.
/// Do not start this project as a console app — open HelloWorld/project.godot in Godot.
/// </summary>
internal static class Designer {

	public static int Main(string[] args)
		=> throw new NotSupportedException(
			"This project runs inside Godot. Use the Avalonia previewer, or open HelloWorld/project.godot.");

	public static AppBuilder BuildAvaloniaApp()
		=> AppBuilder
			.Configure<App>()
			.UseSkia()
			.UseHarfBuzz();

}
