using CommunityToolkit.Mvvm.ComponentModel;

namespace HelloWorld.ViewModels.Pages;

public sealed partial class PassThroughViewModel : ObservableObject {

	public string Hint =>
		"本页大面积透明。只有中间卡片会捕获鼠标；空白处可拖动背后的 Godot Sprite。" +
		"这依赖 Estragonia 的 HasPoint / CaptureEmptyHits=false。";

}
