# Input & rendering

## Rendering

Avalonia draws into a Vulkan `VkImage` shared with Godot (`Texture2Drd`). The host `Control` blits that texture in `_Draw`. There is no separate Avalonia “window layer” — z-order follows the Godot scene tree / `z_index`.

## Input order (Godot)

```text
Node._Input
  → Control._GuiInput (Avalonia host, if _HasPoint)
  → Node._UnhandledInput
```

- Avalonia host typically `AcceptEvent()` for pointer events it receives, so later GUI / unhandled handlers do not see them.
- A node using `_Input` + `SetInputAsHandled()` can run **before** Avalonia and steal the event.

## Themes

Avalonia `RequestedThemeVariant` Light/Dark works. Platform “system” theme from Estragonia currently defaults to Dark via `GodotPlatformSettings`.
