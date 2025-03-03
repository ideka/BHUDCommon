using Blish_HUD.Controls;
using Blish_HUD.Input;
using System;

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
        BasicTooltipText = _keybind.GetBindingDisplayText();
        KeybindEnabled = true;

        _keybind.Activated += KeybindActivated;
        _keybind.BindingChanged += BindingChanged;
    }

    private void KeybindActivated(object o, EventArgs e)
    {
        if (Enabled)
            OnClick(new MouseEventArgs(MouseEventType.LeftMouseButtonReleased, false));
    }

    private void BindingChanged(object o, EventArgs e)
    {
        BasicTooltipText = _keybind.GetBindingDisplayText();
    }

    protected override void DisposeControl()
    {
        _keybind.Activated -= KeybindActivated;
        _keybind.BindingChanged -= BindingChanged;
        KeybindEnabled = false;
        base.DisposeControl();
    }
}
