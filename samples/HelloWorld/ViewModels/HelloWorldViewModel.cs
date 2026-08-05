using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HelloWorld.ViewModels;

public sealed partial class HelloWorldViewModel : ObservableObject {

	public event Action? PaintOuseRequested;
	public event Action? ResetOuseRequested;

	public ObservableCollection<InventoryItem> Inventory { get; } = [
		new("铁剑", "武器", 1),
		new("皮甲", "防具", 1),
		new("生命药水", "消耗", 5),
		new("法力药水", "消耗", 3),
		new("传送卷轴", "消耗", 2),
		new("金币袋", "杂物", 1),
	];

	public ObservableCollection<string> LogLines { get; } = [
		"[系统] Estragonia HelloWorld — 游戏 UI 示例",
		"[提示] 右侧可拖拽 Godot Sprite（Ouse）测输入穿透",
	];

	public ObservableCollection<string> Weapons { get; } = ["剑", "弓", "法杖", "匕首"];

	public ObservableCollection<string> GraphicsOptions { get; } = ["低", "中", "高", "极致"];

	[ObservableProperty] private string _playerName = "旅人";
	[ObservableProperty] private double _hp = 72;
	[ObservableProperty] private double _mp = 40;
	[ObservableProperty] private double _exp = 35;
	[ObservableProperty] private int _gold = 1280;
	[ObservableProperty] private int _wave = 3;
	[ObservableProperty] private string _selectedWeapon = "剑";
	[ObservableProperty] private bool _musicEnabled = true;
	[ObservableProperty] private bool _sfxEnabled = true;
	[ObservableProperty] private double _masterVolume = 80;
	[ObservableProperty] private double _musicVolume = 65;
	[ObservableProperty] private string _graphicsQuality = "高";
	[ObservableProperty] private bool _vsync = true;
	[ObservableProperty] private DateTimeOffset? _saveSlotDate = DateTimeOffset.Now;
	[ObservableProperty] private InventoryItem? _selectedItem;
	[ObservableProperty] private string _chatDraft = "";
	[ObservableProperty] private string _statusMessage = "准备就绪";
	[ObservableProperty] private bool _isShopOpen;
	[ObservableProperty] private double _fps;
	[ObservableProperty] private double _frameTimeMs;

	public string FpsText => string.Create(CultureInfo.InvariantCulture, $"FPS {Fps:0.0}  |  {FrameTimeMs:0.00} ms");

	partial void OnFpsChanged(double value) => OnPropertyChanged(nameof(FpsText));
	partial void OnFrameTimeMsChanged(double value) => OnPropertyChanged(nameof(FpsText));

	/// <summary>Called from Godot each frame.</summary>
	public void ReportFrame(double fps, double frameTimeMs) {
		Fps = fps;
		FrameTimeMs = frameTimeMs;
	}

	[RelayCommand]
	private void PaintOuse() {
		PaintOuseRequested?.Invoke();
		PushLog("Avalonia → Godot：Ouse 染红");
	}

	[RelayCommand]
	private void ResetOuse() {
		ResetOuseRequested?.Invoke();
		PushLog("Avalonia → Godot：Ouse 恢复");
	}

	[RelayCommand]
	private void Heal() {
		Hp = Math.Min(100, Hp + 15);
		Gold = Math.Max(0, Gold - 20);
		StatusMessage = "喝下药水，HP +15";
		PushLog($"治疗 → HP {Hp:0}，金币 {Gold}");
	}

	[RelayCommand]
	private void NextWave() {
		Wave++;
		Exp = Math.Min(100, Exp + 12);
		StatusMessage = $"第 {Wave} 波来袭！";
		PushLog(StatusMessage);
	}

	[RelayCommand]
	private void BuySelected() {
		if (SelectedItem is null) {
			StatusMessage = "先在背包里选中一件物品";
			return;
		}

		Gold = Math.Max(0, Gold - 50);
		SelectedItem.Count++;
		OnPropertyChanged(nameof(Inventory));
		StatusMessage = $"购入 / 强化：{SelectedItem.Name}";
		PushLog($"{StatusMessage}（-50 金）");
	}

	[RelayCommand]
	private void SendChat() {
		var text = ChatDraft.Trim();
		if (text.Length == 0)
			return;
		PushLog($"[{PlayerName}] {text}");
		ChatDraft = "";
	}

	[RelayCommand]
	private void ToggleShop() {
		IsShopOpen = !IsShopOpen;
		StatusMessage = IsShopOpen ? "商店已打开" : "商店已关闭";
	}

	[RelayCommand]
	private async Task AutoSaveAsync() {
		StatusMessage = "存档中…";
		await Task.Delay(600);
		SaveSlotDate = DateTimeOffset.Now;
		StatusMessage = "自动存档完成";
		PushLog($"存档 @ {SaveSlotDate:HH:mm:ss}");
	}

	private void PushLog(string line) {
		LogLines.Insert(0, line);
		while (LogLines.Count > 40)
			LogLines.RemoveAt(LogLines.Count - 1);
	}

}

public sealed partial class InventoryItem : ObservableObject {

	public InventoryItem(string name, string category, int count) {
		Name = name;
		Category = category;
		Count = count;
	}

	public string Name { get; }
	public string Category { get; }

	[ObservableProperty]
	private int _count;

	public override string ToString() => $"{Name} ×{Count}";

}
