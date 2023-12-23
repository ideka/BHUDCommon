using Blish_HUD.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ideka.BHUDCommon;

public abstract class ValueControl : Container
{
    public string Label
    {
        get => _label.Text;
        set
        {
            _label.Text = value;
            UpdateLayout();
        }
    }

    private int _spacing;
    public int Spacing
    {
        get => _spacing;
        set
        {
            _spacing = value;
            UpdateLayout();
        }
    }

    public int ControlWidth
    {
        get => Control.Width;
        set
        {
            Control.Width = value;
            UpdateLayout();
        }
    }

    public bool ControlEnabled
    {
        get => Control.Enabled;
        set => Control.Enabled = value;
    }

    public string? AllBasicTooltipText
    {
        set => Control.BasicTooltipText =
            _label.BasicTooltipText =
            BasicTooltipText = value;
    }

    protected readonly Label _label;
    protected Control Control { get; private set; }

    public ValueControl(Control control) : base()
    {
        Spacing = 10;

        _label = new Label()
        {
            Parent = this,
            AutoSizeWidth = true,
            VerticalAlignment = VerticalAlignment.Middle,
        };

        Control = control;
        control.Parent = this;

        UpdateLayout();
    }

    public static void AlignLabels(params ValueControl[] controls) => AlignLabels(controls.AsEnumerable());
    public static void AlignLabels(IEnumerable<ValueControl> controls)
    {
        var width = controls.Max(c => c._label.Width);
        foreach (var control in controls)
        {
            control._label.AutoSizeWidth = false;
            control._label.HorizontalAlignment = HorizontalAlignment.Right;
            control._label.Width = width;
            control.UpdateLayout();
        }
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        base.OnResized(e);
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (Control == null)
            return;

        _label.Height = Control.Height;
        Control.Left = _label.Right + Spacing;
        Control.Width = ContentRegion.Width - Control.Left;
        this.SetContentRegionHeight(Control.Bottom);
    }
}

public abstract class ValueControl<TValue, TInnerValue, TControl> : ValueControl
    where TControl : Control, new()
{
    private TValue _value;
    public TValue Value
    {
        get => _value;
        set
        {
            if (!TryReflectValue(ref value))
                return;
            _value = value;
        }
    }

    protected new TControl Control => (TControl)base.Control;

    public event Action<TValue>? TempValue;
    public event Action? TempClear;
    public event Action<TValue>? ValueCommitted;

    protected bool MouseOverControl => Control.MouseOver;

    public ValueControl(TValue start) : base(new TControl())
    {
        _value = start;
    }

    public void CommitValue(TValue value)
    {
        Value = value;
        ValueCommitted?.Invoke(value);
    }

    protected abstract bool TryReflectValue(ref TValue value);
    protected abstract bool TryMakeValue(TInnerValue innerValue, out TValue value);

    protected void ResetValue()
        => Value = Value;

    protected void SetTempValue(TValue value, bool reflect)
    {
        if (reflect && !TryReflectValue(ref value))
            return;
        TempValue?.Invoke(value);
    }

    protected void UnsetTempValue()
    {
        ResetValue();
        TempClear?.Invoke();
    }
}
