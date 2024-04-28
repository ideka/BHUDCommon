using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ideka.BHUDCommon;

public class EnumSetBox<TEnum> : ValueControl<HashSet<TEnum>, HashSet<TEnum>, EnumSetBox<TEnum>.Inner>
    where TEnum : struct, Enum
{
    private readonly Func<TEnum, string?> _describe;
    private readonly Dictionary<string, TEnum> _descValue = [];
    private readonly ContextMenuStrip _menu;

    public class Inner : TextBox
    {
        public new event Action? LeftMouseButtonPressed;
        public new event Action? Click;

        protected override void OnLeftMouseButtonPressed(MouseEventArgs e) => LeftMouseButtonPressed?.Invoke();
        protected override void OnClick(MouseEventArgs e) => Click?.Invoke();
    }

    public EnumSetBox(Func<TEnum, string?>? describe = null, HashSet<TEnum>? start = null) : base(start ?? [])
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

        if (Program.OverlayVersion < new SemVer.Version(1, 1, 2, "0"))
        {
            // This more complex version breaks in Blish 1.1.2, but it's needed for Blish < 1.1.2
            TimeSpan hiddenAt = TimeSpan.Zero;
            bool invalidateNext = false;
            _menu.Hidden += delegate
            {
                hiddenAt = GameService.Overlay.CurrentGameTime.TotalGameTime;
            };

            Control.LeftMouseButtonPressed += delegate
            {
                // Hack so the button can act as a toggle.
                if (GameService.Overlay.CurrentGameTime.TotalGameTime == hiddenAt)
                    invalidateNext = true;
            };

            Control.Click += delegate
            {
                if (invalidateNext)
                {
                    invalidateNext = false;
                    return;
                }

                _menu.Show(Control);
            };
        }
        else
        {
            TimeSpan hiddenAt = TimeSpan.Zero;
            _menu.Hidden += delegate
            {
                hiddenAt = GameService.Overlay.CurrentGameTime.TotalGameTime;
            };

            Control.LeftMouseButtonPressed += delegate
            {
                // Hack so the button can act as a toggle.
                if (GameService.Overlay.CurrentGameTime.TotalGameTime != hiddenAt)
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
        var values = value.Select(_describe);
        Control.Text = string.Join(", ", values);
        Control.BasicTooltipText = string.Join("\n", values);
        return true;
    }

    protected override void DisposeControl()
    {
        _menu.Dispose();
        base.DisposeControl();
    }
}
