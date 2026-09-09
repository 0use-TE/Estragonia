# HelloWorld

Game-oriented Avalonia UI sample for Estragonia.

Godot project: this folder (`project.godot`). Host scripts live in `Estragonia/`.  
Avalonia UI: sibling `../HelloWorld.UI` (open AXAML there for the previewer).  
Solution: `../HelloWorld.sln`.

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

1. Restore from the repo / `samples/HelloWorld.sln`.
2. Open **this folder** in **Godot 4.7.2+ (.NET)**.
3. Run the main scene.
4. Drag the Godot sprite on the right to test input vs Avalonia.

FPS is written from `UserInterface._Process` → `HelloWorldViewModel.ReportFrame`.
