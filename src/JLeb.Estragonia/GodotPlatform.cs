using System;
using System.Reflection;
using System.Threading;
using Avalonia;
using Avalonia.Controls.Platform;
using Avalonia.Dialogs;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Godot;
using JLeb.Estragonia.Input;
using AvCompositor = Avalonia.Rendering.Composition.Compositor;

namespace JLeb.Estragonia;

/// <summary>Contains Godot to Avalonia platform initialization.</summary>
/// <remarks>
/// Used by the Godot-project host scripts (<c>AvaloniaControl</c> / <c>UiHost</c>).
/// Those scripts must live in your Godot C# project, not in this NuGet assembly.
/// </remarks>
public static class GodotPlatform {

	private static AvCompositor? s_compositor;
	private static ManualRenderTimer? s_renderTimer;
	private static GodotVkPlatformGraphics? s_platformGraphics;
	private static GodotDispatcherImpl? s_dispatcherImpl;
	private static bool s_dispatcherInitialized;
	private static ulong s_lastProcessFrame = UInt64.MaxValue;

	public static AvCompositor Compositor
		=> s_compositor ?? throw new InvalidOperationException($"{nameof(GodotPlatform)} hasn't been initialized");

	public static void Initialize() {
		AvaloniaSynchronizationContext.AutoInstall = false; // Godot has its own sync context, don't replace it

		EnsureAssetLoader(null);

		// Dispatcher.InitializeUIThreadDispatcher can run only once; reuse the impl and only
		// recreate its thread-pool timer after a Shutdown.
		if (!s_dispatcherInitialized) {
			s_dispatcherImpl = new GodotDispatcherImpl(Thread.CurrentThread);
			Avalonia.Threading.Dispatcher.InitializeUIThreadDispatcher(s_dispatcherImpl);
			s_dispatcherInitialized = true;
		}
		else {
			s_dispatcherImpl?.Resume();
		}

		var platformGraphics = new GodotVkPlatformGraphics();
		var renderTimer = new ManualRenderTimer();
		var renderLoop = RenderLoop.FromTimer(renderTimer);

		AvaloniaLocator.CurrentMutable
			.Bind<IClipboard>().ToConstant(new GodotClipboard())
			.Bind<ICursorFactory>().ToConstant(new GodotCursorFactory())
			.Bind<IKeyboardDevice>().ToConstant(GodotDevices.Keyboard)
			.Bind<IPlatformGraphics>().ToConstant(platformGraphics)
			.Bind<IPlatformIconLoader>().ToConstant(new StubPlatformIconLoader())
			.Bind<IPlatformSettings>().ToConstant(new GodotPlatformSettings())
			.Bind<IRenderTimer>().ToConstant(renderTimer)
			.Bind<IRenderLoop>().ToConstant(renderLoop)
			.Bind<IWindowingPlatform>().ToConstant(new GodotWindowingPlatform())
			.Bind<IStorageProviderFactory>().ToConstant(new GodotStorageProviderFactory())
			.Bind<PlatformHotkeyConfiguration>().ToConstant(CreatePlatformHotKeyConfiguration())
			.Bind<ManagedFileDialogOptions>().ToConstant(new ManagedFileDialogOptions { AllowDirectorySelection = true });

		s_platformGraphics = platformGraphics;
		s_renderTimer = renderTimer;
		s_compositor = new AvCompositor(platformGraphics);
	}

	/// <summary>
	/// Drops Godot/Avalonia platform services so <see cref="Initialize"/> can run again.
	/// Does not reset the UI dispatcher (Avalonia forbids that).
	/// </summary>
	public static void Reset() {
		s_compositor = null;
		s_renderTimer = null;
		s_lastProcessFrame = UInt64.MaxValue;

		if (s_platformGraphics is not null) {
			s_platformGraphics.Dispose();
			s_platformGraphics = null;
		}

		s_dispatcherImpl?.Pause();

		// Drop Application.Current and every Godot/Avalonia service that could pin the game ALC.
		AvaloniaLocator.Current = AvaloniaLocator.CurrentMutable = new AvaloniaLocator();
	}

	/// <summary>
	/// Ensures <see cref="IAssetLoader"/> is registered for XAML <c>avares</c> / image sources.
	/// Godot's assembly load context can leave the standard runtime registration missing.
	/// </summary>
	public static void EnsureAssetLoader(Assembly? defaultAssembly) {
		AssetLoader.RegisterResUriParsers();

		if (AvaloniaLocator.Current.GetService<IAssetLoader>() is not { } assetLoader) {
			assetLoader = new StandardAssetLoader(defaultAssembly);
			AvaloniaLocator.CurrentMutable.Bind<IAssetLoader>().ToConstant(assetLoader);
		}
		else if (defaultAssembly is not null) {
			assetLoader.SetDefaultAssembly(defaultAssembly);
		}

		if (AvaloniaLocator.Current.GetService<IRuntimePlatform>() is null) {
			AvaloniaLocator.CurrentMutable.Bind<IRuntimePlatform>().ToSingleton<StandardRuntimePlatform>();
		}
	}

	private static PlatformHotkeyConfiguration CreatePlatformHotKeyConfiguration()
		=> OperatingSystem.IsMacOS()
			? new PlatformHotkeyConfiguration(commandModifiers: KeyModifiers.Meta, wholeWordTextActionModifiers: KeyModifiers.Alt)
			: new PlatformHotkeyConfiguration(commandModifiers: KeyModifiers.Control);

	public static void TriggerRenderTick() {
		if (s_renderTimer is null)
			return;

		// if we have several AvaloniaControls, ensure we tick the timer only once each frame
		var processFrame = Engine.GetProcessFrames();
		if (processFrame == s_lastProcessFrame)
			return;

		s_lastProcessFrame = processFrame;
		s_renderTimer.TriggerTick(new TimeSpan((long) (Time.GetTicksUsec() * 10UL)));
	}

}
