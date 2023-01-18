using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ideka.BHUDCommon
{
    public class EscBlockWindow : WindowBase2
    {
        private readonly Container _owner;

        public EscBlockWindow(Container owner)
        {
            TopMost = true;
            CanClose = false;
            Visible = false;
            Parent = Graphics.SpriteScreen;
            _owner = owner;
            _owner.Disposed += OwnerDisposed;
        }

        public void BlockStart()
        {
            Visible = true;
            FocusedControl = this;
        }

        public void BlockEnd()
        {
            Visible = false;
            FocusedControl = _owner;
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle drawBounds, Rectangle scissor) { }

        private void OwnerDisposed(object sender, EventArgs e) => Dispose();

        protected override void DisposeControl()
        {
            _owner.Disposed -= OwnerDisposed;

            base.DisposeControl();
        }
    }
}
