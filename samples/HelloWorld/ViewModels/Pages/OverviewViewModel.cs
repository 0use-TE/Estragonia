using CommunityToolkit.Mvvm.ComponentModel;

namespace HelloWorld.ViewModels.Pages;

public sealed partial class OverviewViewModel : ObservableObject {

	public string Title => "Estragonia HelloWorld";

	public string Subtitle => "Avalonia 12 嵌入 Godot 4（Vulkan 共享纹理）";

	public string Body =>
		"左侧切换演示场景。自定义光标、HUD、输入穿透等都会在对应页验证。" +
		"场景右侧/透明区域可看到背后的 Godot Sprite；穿透页专门演示空像素不抢输入。";

}
