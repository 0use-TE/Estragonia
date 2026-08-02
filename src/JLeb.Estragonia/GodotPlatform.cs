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
internal static class GodotPlatform {

	private static AvCompositor? s_compositor;
	private static ManualRenderTimer? s_renderTimer;
	private static ulong s_lastProcessFrame = UInt64.MaxValue;

	public static AvCompositor Compositor
		=> s_compositor ?? throw new InvalidOperationException($"{nameof(GodotPlatform)} hasn't been initialized");

	public static void Initialize() {
		AvaloniaSynchronizationContext.AutoInstall = false; // Godot has its own sync context, don't replace it

		EnsureAssetLoader(null);

		// Avalonia 12 no longer reads IDispatcherImpl from AvaloniaLocator (that path is obsolete / DEBUG-throws).
		// Platforms must call InitializeUIThreadDispatcher, same as Win32/Browser/etc.
		var dispatcherImpl = new GodotDispatcherImpl(Thread.CurrentThread);
		Avalonia.Threading.Dispatcher.InitializeUIThreadDispatcher(dispatcherImpl);

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

		s_renderTimer = renderTimer;
		s_compositor = new AvCompositor(platformGraphics);
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
