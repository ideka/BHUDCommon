using Blish_HUD.Controls;
using Blish_HUD.Input;
using System;

namespace Ideka.BHUDCommon;

public class ButtonBox<TValue>(TValue start) : ValueControl<TValue, TValue, StandardButton>(start)
{
    public string ButtonText
    {
        get => Control.Text;
        set => Control.Text = value;
    }

    public event EventHandler<MouseEventArgs> ButtonClick
    {
        add => Control.Click += value;
        remove => Control.Click -= value;
    }

    protected override bool TryMakeValue(TValue innerValue, out TValue value)
    {
        value = innerValue;
        return true;
    }

    protected override bool TryReflectValue(ref TValue value) => true;
}
