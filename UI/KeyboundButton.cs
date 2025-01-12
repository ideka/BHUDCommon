using Blish_HUD.Controls;
using Blish_HUD.Input;

namespace Ideka.BHUDCommon;

public class KeyboundButton : StandardButton
{
    private readonly KeyBinding _keybind;

    public bool KeybindEnabled
    {
        set => _keybind.Enabled = _keybind.BlockSequenceFromGw2 = value;
    }

    public KeyboundButton(KeyBinding keybind)
    {
        _keybind = keybind;
        _keybind.Activated += delegate
        {
            if (Enabled)
                OnClick(new MouseEventArgs(MouseEventType.LeftMouseButtonReleased, false));
        };
        BasicTooltipText = _keybind.GetBindingDisplayText();
        KeybindEnabled = true;
    }

    protected override void DisposeControl()
    {
        KeybindEnabled = false;
        base.DisposeControl();
    }
}
