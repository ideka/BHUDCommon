using Blish_HUD.Controls;

namespace Ideka.BHUDCommon;

public class BoolBox : ValueControl<bool, bool, Checkbox>
{
    public BoolBox(bool start) : base(start)
    {
        Control.CheckedChanged += delegate
        {
            CommitValue(Control.Checked);
        };
    }

    public BoolBox() : this(false)
    {
    }

    protected override bool TryMakeValue(bool innerValue, out bool value)
    {
        value = innerValue;
        return true;
    }

    protected override bool TryReflectValue(ref bool value)
    {
        Control.Checked = value;
        return true;
    }
}
