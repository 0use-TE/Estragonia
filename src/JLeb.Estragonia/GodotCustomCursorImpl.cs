using Avalonia.Platform;
using Godot;

namespace JLeb.Estragonia;

/// <summary>A custom bitmap cursor backed by a Godot <see cref="ImageTexture"/>.</summary>
internal sealed class GodotCustomCursorImpl : ICursorImpl {

	public ImageTexture Texture { get; }

	public Vector2 Hotspot { get; }

	public GodotCustomCursorImpl(ImageTexture texture, Vector2 hotspot) {
		Texture = texture;
		Hotspot = hotspot;
	}

	public override string ToString()
		=> $"Custom({Texture.GetWidth()}x{Texture.GetHeight()}@{Hotspot})";

	public void Dispose()
		=> Texture.Dispose();

}
