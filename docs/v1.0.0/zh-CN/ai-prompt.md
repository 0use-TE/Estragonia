# 业务 AI 提示词

给 AI 写 Estragonia 上的 Avalonia UI 时可用：

```
你在 Godot 4 + Estragonia 上写 Avalonia 12 UI。
- Autoload 里 UseGodot + EnsureAssetLoader 只做一次。
- 宿主用 UiHost / CreateRoot。
- 复杂界面用 CommunityToolkit.Mvvm。
- 素材按钮用 PNG，不要指望 Godot ShaderMaterial 挂在 Avalonia 控件上。
- CaptureEmptyHits 为 false 时空白可穿透到 Godot。
- 代码可能由 AI 生成，保持改动小，并在 Godot Forward+ 下自测。
```
