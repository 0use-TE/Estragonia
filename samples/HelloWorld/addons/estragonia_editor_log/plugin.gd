@tool
extends EditorPlugin

const Host := preload("res://addons/estragonia_editor/AvaloniaEditorHost.cs")
const VIEW_TYPE := "HelloWorld.Editor.LogView"

var _dock: EditorDock


func _enter_tree() -> void:
	var host := Host.new()
	host.name = "EstragoniaLogHost"
	host.custom_minimum_size = Vector2(420, 160)
	host.set_meta("estragonia_view_type", VIEW_TYPE)

	_dock = EditorDock.new()
	_dock.title = "Estragonia Log"
	_dock.clip_contents = true
	_dock.default_slot = EditorDock.DOCK_SLOT_BOTTOM
	_dock.available_layouts = EditorDock.DOCK_LAYOUT_HORIZONTAL | EditorDock.DOCK_LAYOUT_FLOATING
	_dock.add_child(host)
	add_dock(_dock)


func _exit_tree() -> void:
	if _dock == null:
		return
	remove_dock(_dock)
	_dock.queue_free()
	_dock = null
