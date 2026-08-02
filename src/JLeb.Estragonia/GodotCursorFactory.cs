using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace JLeb.Estragonia;

internal sealed class GodotCursorFactory : ICursorFactory {

	public ICursorImpl GetCursor(StandardCursorType cursorType)
		=> new GodotStandardCursorImpl(cursorType.ToGodotCursorShape());

	public ICursorImpl CreateCursor(Bitmap cursor, PixelPoint hotSpot)
		=> throw new NotSupportedException("Custom cursors aren't supported");

}
