# HelloWorld

Game-oriented Avalonia UI sample for Estragonia (single project).

## What it shows

| Area | Controls / features |
|------|---------------------|
| Top HUD | HP/MP/EXP `ProgressBar`, gold, wave, **FPS** from Godot |
| 战场 | `TextBox`, `ComboBox`, `RadioButton`, `NumericUpDown`, `Button`, `Expander`, `CheckBox`, `Flyout` + image |
| 背包 | `ListBox` + `DataTemplate`, selection, chat `TextBox` |
| 设置 | `ToggleSwitch`, `Slider`, `CalendarDatePicker` |
| Godot | Avalonia → tint Sprite; empty area pass-through |
| 右侧 | Combat log `ListBox` |

## Run

1. Open this folder in **Godot 4.7+ (.NET)**.
2. Run the main scene.
3. Drag the Godot sprite on the right to test input vs Avalonia.

FPS is written from `UserInterface._Process` → `HelloWorldViewModel.ReportFrame`.
