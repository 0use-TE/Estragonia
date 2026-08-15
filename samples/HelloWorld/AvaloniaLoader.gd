extends Node

func _ready() -> void:
	var win := get_window()
	if win:
		win.set_ime_active(true)
	if Engine.is_editor_hint():
		return
	var game := AvaloniaGame.new()
	game.start()
	game.free()
