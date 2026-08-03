# 输入与渲染

## 渲染

Avalonia 画到与 Godot 共享的 Vulkan 纹理，由宿主 `Control._Draw` 贴出。层级跟 Godot 场景树 / `z_index` 走。

## 输入顺序

```text
_Input → _GuiInput（Avalonia 宿主）→ _UnhandledInput
```

宿主收到的指针事件通常会 `AcceptEvent()`。更早的 `_Input` 若 `SetInputAsHandled()`，Avalonia 可能收不到。
