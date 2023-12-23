using Blish_HUD.Controls;
using Blish_HUD.Input;

namespace Ideka.BHUDCommon;

// TODO: Delete this when the relevant Blish-HUD bug is fixed
public class TextBoxFix : TextBox
{
    protected override void OnLeftMouseButtonPressed(MouseEventArgs e)
    {
        if (Enabled)
            base.OnLeftMouseButtonPressed(e);
    }

    protected override void OnClick(MouseEventArgs e)
    {
        if (Enabled)
            base.OnClick(e);
    }
}
