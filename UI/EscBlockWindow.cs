using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ideka.BHUDCommon
{
    public class EscBlockWindow : WindowBase2
    {
        public EscBlockWindow(Container parent)
        {
            TopMost = true;
            CanClose = false;
            Visible = false;
            Parent = parent;
        }

        public void BlockStart()
        {
            Visible = true;
            FocusedControl = this;
        }

        public void BlockEnd()
        {
            Visible = false;
            FocusedControl = Parent;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle drawBounds, Rectangle scissor) { }
    }
}
