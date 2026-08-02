using System;
using Avalonia.Platform;

namespace JLeb.Estragonia;

internal sealed class GodotWindowingPlatform : IWindowingPlatform {

	public IWindowImpl CreateWindow()
		=> throw CreateNotImplementedException();

	public IWindowImpl CreateEmbeddableWindow()
		=> throw CreateNotImplementedException();

	public ITopLevelImpl CreateEmbeddableTopLevel()
		=> throw CreateNotImplementedException();

	public void GetWindowsZOrder(ReadOnlySpan<IWindowImpl> windows, Span<long> zOrder) {
		for (var i = 0; i < zOrder.Length; i++)
			zOrder[i] = i;
	}

	private static NotImplementedException CreateNotImplementedException()
		=> new("Sub windows aren't implemented yet");

	public ITrayIconImpl? CreateTrayIcon()
		=> null;

}
