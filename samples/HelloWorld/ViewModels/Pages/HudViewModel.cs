using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloWorld.ViewModels.Pages;

public sealed partial class HudViewModel : ObservableObject {

	[ObservableProperty]
	private double _hp = 72;

	[ObservableProperty]
	private double _mp = 40;

	[ObservableProperty]
	private double _exp = 55;

	[ObservableProperty]
	private int _gold = 1280;

	[ObservableProperty]
	private int _wave = 3;

	public ObservableCollection<string> Log { get; } = [
		"进入战场。",
		"波次 3 开始。",
		"击杀史莱姆 +12 EXP。"
	];

	[RelayCommand]
	private void TakeDamage() {
		Hp = System.Math.Max(0, Hp - 8);
		Log.Insert(0, $"受到 8 点伤害（HP {Hp:0}）。");
	}

	[RelayCommand]
	private void Heal() {
		Hp = System.Math.Min(100, Hp + 15);
		Mp = System.Math.Max(0, Mp - 5);
		Log.Insert(0, $"恢复生命（HP {Hp:0} / MP {Mp:0}）。");
	}

}
