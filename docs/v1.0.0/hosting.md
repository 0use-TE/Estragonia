# Hosting UI

## Split responsibilities

| Concern | Where |
|---------|--------|
| `UseGodot` / asset loader / IME | Autoload (`AvaloniaLoader`) once |
| Focus / mouse filter / `CreateRoot` | `UiHost` |
| Theme / app resources | Avalonia `Application` |

Do **not** call `GrabFocus` / `GetWindow` from Avalonia `App`.

## UiHost

```csharp
public abstract class UiHost : AvaloniaControl
{
    protected abstract Control CreateRoot();

    public override void _Ready()
    {
        FocusMode = FocusModeEnum.All;
        MouseFilter = MouseFilterEnum.Stop;
        Control = CreateRoot();
        base._Ready();
        GrabFocus();
    }
}
```

## Hit testing

By default `AvaloniaControl.CaptureEmptyHits` is `false`: only Avalonia-hittable pixels capture the mouse; empty areas pass through to Godot (e.g. a `Sprite2D` behind the host).

Set `CaptureEmptyHits = true` to capture the whole control rect.
