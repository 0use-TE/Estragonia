using System;
using System.IO;
using Avalonia;
using Avalonia.Input;
using Avalonia.Platform;
using Godot;
using AvBitmap = Avalonia.Media.Imaging.Bitmap;

namespace JLeb.Estragonia;

internal sealed class GodotCursorFactory : ICursorFactory {

	private const int MaxCursorSize = 256;

	public ICursorImpl GetCursor(StandardCursorType cursorType)
		=> new GodotStandardCursorImpl(cursorType.ToGodotCursorShape());

	public ICursorImpl CreateCursor(AvBitmap cursor, PixelPoint hotSpot) {
		var size = cursor.PixelSize;
		if (size.Width <= 0 || size.Height <= 0)
			throw new ArgumentException("Cursor bitmap must have a positive size.", nameof(cursor));

		if (size.Width > MaxCursorSize || size.Height > MaxCursorSize)
			throw new ArgumentException($"Godot custom cursors must be at most {MaxCursorSize}x{MaxCursorSize}.", nameof(cursor));

		if (hotSpot.X < 0 || hotSpot.Y < 0 || hotSpot.X >= size.Width || hotSpot.Y >= size.Height)
			throw new ArgumentOutOfRangeException(nameof(hotSpot), "Hotspot must lie within the cursor bitmap.");

		using var stream = new MemoryStream();
		cursor.Save(stream);

		var image = new Image();
		var error = image.LoadPngFromBuffer(stream.ToArray());
		if (error != Error.Ok) {
			image.Dispose();
			throw new InvalidOperationException($"Failed to decode cursor PNG for Godot: {error}");
		}

		var texture = ImageTexture.CreateFromImage(image);
		image.Dispose();

		return new GodotCustomCursorImpl(texture, new Vector2(hotSpot.X, hotSpot.Y));
	}

}
