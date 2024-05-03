using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ideka.BHUDCommon;

// Ugly, ugly hack
public class OnelineMenuItem(string text) : MenuItem(text)
{
    public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
    {
        var text = _text;
        _text = "";

        base.PaintBeforeChildren(spriteBatch, bounds);

        var num = _children.IsEmpty ? 10 : 26;
        if (_canCheck || _icon != null)
            num += 42;
        else if (!_children.IsEmpty)
            num += 10;

        _text = text;

        spriteBatch.DrawStringOnCtrl(this, _text, Content.DefaultFont16, new Rectangle(num, 0, Width - (num - 10), MenuItemHeight), Color.White, wrap: false, stroke: true);
    }
}
