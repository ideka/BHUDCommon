using Blish_HUD;
using Blish_HUD.Controls;
using System;
using System.Collections.Generic;

namespace Ideka.BHUDCommon;

public class EnumDropdown<TEnum> : ValueControl<TEnum, string, Dropdown>
    where TEnum : struct, Enum
{
    private readonly Func<TEnum, string?> _describe;
    private readonly Dictionary<string, TEnum> _descValue = new();

    public EnumDropdown() : this(null, default)
    {
    }

    public EnumDropdown(Func<TEnum, string?>? describe = null, TEnum start = default) : base(start)
    {
        _describe = describe ?? (v => Enum.GetName(typeof(TEnum), v));

        foreach (TEnum value in EnumUtil.GetCachedValues<TEnum>())
        {
            if (_describe(value) is string desc)
            {
                _descValue[desc] = value;
                Control.Items.Add(desc);
            }
        }

        Control.ValueChanged += delegate
        {
            if (!TryMakeValue(Control.SelectedItem, out var newValue))
            {
                ResetValue();
                return;
            }

            if (!Value.Equals(newValue))
                CommitValue(newValue);
        };
    }

    protected override bool TryMakeValue(string innerValue, out TEnum value)
        => _descValue.TryGetValue(innerValue, out value);

    protected override bool TryReflectValue(ref TEnum value)
    {
        var item = _describe(value);
        if (item == null)
            return false;
        Control.SelectedItem = item;
        return true;
    }
}
