using Blish_HUD.Controls;
using System.Collections.Generic;

namespace Ideka.BHUDCommon;

public class ValueControlPanel : FlowPanel
{
    private readonly List<ValueControl> _controls;

    public ValueControlPanel() : base()
    {
        _controls = new List<ValueControl>();
    }

    public T Add<T>(T control) where T : ValueControl
    {
        _controls.Add(control);
        control.Parent = this;
        return control;
    }

    public T Add<T>(string label) where T : ValueControl, new() => Add(new T()
    {
        Label = label,
        Parent = this,
    });

    protected override void OnResized(ResizedEventArgs e)
    {
        base.OnResized(e);
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (_controls == null)
            return;

        foreach (var control in _controls)
            control.Width = (int)(ContentRegion.Width - OuterControlPadding.X * 2);

        ValueControl.AlignLabels(_controls);
    }
}
