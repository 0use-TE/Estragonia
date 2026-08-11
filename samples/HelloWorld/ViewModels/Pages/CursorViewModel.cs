using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HelloWorld.ViewModels.Pages;

public sealed partial class CursorViewModel : ObservableObject {

	public Cursor CrosshairCursor { get; }

	public Cursor TargetCursor { get; }

	public CursorViewModel() {
		CrosshairCursor = new Cursor(CreateCursorBitmap(CrosshairPixels), new PixelPoint(16, 16));
		TargetCursor = new Cursor(CreateCursorBitmap(TargetPixels), new PixelPoint(16, 16));
	}

	private static WriteableBitmap CreateCursorBitmap(System.Func<int, int, uint> colorAt) {
		const int size = 32;
		var bitmap = new WriteableBitmap(
			new PixelSize(size, size),
			new Vector(96, 96),
			PixelFormat.Rgba8888,
			AlphaFormat.Unpremul);

		using var fb = bitmap.Lock();
		unsafe {
			var ptr = (uint*)fb.Address;
			var stride = fb.RowBytes / 4;
			for (var y = 0; y < size; y++) {
				for (var x = 0; x < size; x++)
					ptr[y * stride + x] = colorAt(x, y);
			}
		}

		return bitmap;
	}

	/// <summary>RGBA packed as 0xAABBGGRR for Rgba8888 little-endian write as uint? 
	/// Actually for Rgba8888 memory layout is R,G,B,A bytes. As uint LE: 0xAABBGGRR.</summary>
	private static uint Rgba(byte r, byte g, byte b, byte a)
		=> (uint)(r | (g << 8) | (b << 16) | (a << 24));

	private static uint CrosshairPixels(int x, int y) {
		const int c = 16;
		var onCross = (x == c || y == c) && System.Math.Abs(x - c) <= 12 && System.Math.Abs(y - c) <= 12;
		var onGap = System.Math.Abs(x - c) < 3 && System.Math.Abs(y - c) < 3;
		if (onCross && !onGap)
			return Rgba(80, 220, 255, 255);
		return Rgba(0, 0, 0, 0);
	}

	private static uint TargetPixels(int x, int y) {
		const int c = 16;
		var dx = x - c;
		var dy = y - c;
		var d2 = dx * dx + dy * dy;
		if (d2 is >= 64 and <= 81 || d2 is >= 196 and <= 225)
			return Rgba(255, 90, 90, 255);
		if (System.Math.Abs(dx) <= 1 && System.Math.Abs(dy) <= 1)
			return Rgba(255, 220, 80, 255);
		return Rgba(0, 0, 0, 0);
	}

}
