# 宿主与 UI

## 职责拆分

| 事情 | 放哪 |
|------|------|
| `UseGodot` / 资源 / IME | Autoload 一次 |
| 焦点 / `CreateRoot` | `UiHost` |
| 主题 / 全局样式 | Avalonia `Application` |

不要在 Avalonia `App` 里调用 `GrabFocus` / `GetWindow`。

## 命中测试

默认 `CaptureEmptyHits = false`：只有 Avalonia 命中到的像素吃鼠标，空白可穿透到 Godot。
