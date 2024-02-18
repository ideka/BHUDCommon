using Blish_HUD;
using Blish_HUD.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ideka.BHUDCommon;

public class EnumSetButton<TEnum> : ValueControl<HashSet<TEnum>, HashSet<TEnum>, StandardButton>
    where TEnum : struct, Enum
{
    private readonly Func<TEnum, string?> _describe;
    private readonly Dictionary<string, TEnum> _descValue = [];
    private readonly ContextMenuStrip _menu;

    public EnumSetButton(Func<TEnum, string?>? describe = null, HashSet<TEnum>? start = null) : base(start ?? [])
    {
        _describe = describe ?? (v => Enum.GetName(typeof(TEnum), v));

        _menu = new ContextMenuStrip(menu);
        IEnumerable<ContextMenuStripItem> menu()
        {
            foreach (TEnum value in EnumUtil.GetCachedValues<TEnum>())
            {
                if (_describe(value) is not string desc)
                    continue;

                _descValue[desc] = value;
                var item = new ContextMenuStripItem(desc)
                {
                    CanCheck = true,
                    Checked = Value.Contains(value),
                };

                item.CheckedChanged += delegate
                {
                    var modified = item.Checked
                        ? Value.Add(value)
                        : Value.Remove(value);
                    if (modified)
                        CommitValue(Value);
                };

                yield return item;
            }
        }

        {
            TimeSpan hiddenAt = TimeSpan.Zero;
            _menu.Hidden += delegate
            {
                hiddenAt = GameService.Overlay.CurrentGameTime.TotalGameTime;
            };

            Control.LeftMouseButtonPressed += delegate
            {
                // Hack so the button can act as a toggle.
                if (GameService.Overlay.CurrentGameTime.TotalGameTime == hiddenAt)
                    return;
                _menu.Show(Control);
            };
        }
    }

    protected override bool TryMakeValue(HashSet<TEnum> innerValue, out HashSet<TEnum> value)
    {
        value = innerValue;
        return true;
    }

    protected override bool TryReflectValue(ref HashSet<TEnum> value)
    {
        Control.Text = string.Join(", ", value.Select(_describe));
        return true;
    }

    protected override void DisposeControl()
    {
        _menu.Dispose();
        base.DisposeControl();
    }
}
