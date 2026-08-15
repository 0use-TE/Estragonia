using Avalonia;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;

namespace JLeb.Estragonia;

/// <summary>
/// Process-wide Avalonia <see cref="Application"/> for the Godot editor.
/// Lives in this assembly so editor plugins do not pin the game project assembly.
/// </summary>
public sealed class EditorAvaloniaApp : Application {

	public override void Initialize() {
		Styles.Add(new FluentTheme());
		RequestedThemeVariant = ThemeVariant.Dark;
	}

}
