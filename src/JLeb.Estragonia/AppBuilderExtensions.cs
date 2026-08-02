using Avalonia;

namespace JLeb.Estragonia;

/// <summary>Contains extensions methods for <see cref="AppBuilder"/> related to Godot.</summary>
public static class AppBuilderExtensions {

	public static AppBuilder UseGodot(this AppBuilder builder)
		=> builder
			.UseStandardRuntimePlatformSubsystem()
			.UseSkia()
			.UseHarfBuzz()
			.UseWindowingSubsystem(GodotPlatform.Initialize)
			.AfterPlatformServicesSetup(b => GodotPlatform.EnsureAssetLoader(b.ApplicationType?.Assembly))
			.AfterSetup(b => GodotPlatform.EnsureAssetLoader(b.ApplicationType?.Assembly));

}
