using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HelloWorld.ViewModels.Pages;

namespace HelloWorld.ViewModels;

public sealed partial class HelloWorldViewModel : ObservableObject {

	public ObservableCollection<DemoScene> Scenes { get; }

	[ObservableProperty]
	private DemoScene? _selectedScene;

	public HelloWorldViewModel() {
		Scenes = [
			new DemoScene("总览", "Estragonia 示例导航", new OverviewViewModel()),
			new DemoScene("基础控件", "常用 Avalonia 控件一览", new ControlsViewModel()),
			new DemoScene("自定义光标", "标准光标 + 位图光标", new CursorViewModel()),
			new DemoScene("游戏 HUD", "血条 / 资源 / 战斗日志", new HudViewModel()),
			new DemoScene("输入穿透", "透明区域把输入交给 Godot", new PassThroughViewModel()),
			new DemoScene("数据绑定", "MVVM 列表与命令", new BindingViewModel()),
		];
		SelectedScene = Scenes[0];
	}

}

public sealed class DemoScene {

	public DemoScene(string title, string description, object content) {
		Title = title;
		Description = description;
		Content = content;
	}

	public string Title { get; }

	public string Description { get; }

	public object Content { get; }

}
