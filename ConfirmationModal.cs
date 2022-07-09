using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ideka.BHUDCommon
{
    public class ConfirmationModal : Container
    {
        private static readonly Point Margin = new Point(20, 15);
        private const int Spacing = 10;

        private Point _modalSize;
        public int ModalWidth { get => ModalSize.X; set => ModalSize = new Point(value, ModalSize.Y); }
        public int ModalHeight { get => ModalSize.Y; set => ModalSize = new Point(ModalSize.X, value); }
        public Point ModalSize
        {
            get => _modalSize;
            set
            {
                _modalSize = value;
                ContentRegion = new Rectangle(
                    Width / 2 - _modalSize.X / 2,
                    Height / 2 - _modalSize.Y / 2,
                    _modalSize.X, _modalSize.Y);
            }
        }

        private readonly Image _background;
        private readonly Label _titleLabel;
        private readonly Label _textLabel;
        private readonly StandardButton _confirmButton;
        private readonly StandardButton _cancelButton;

        private Action _confirmed;
        private Action _cancelled;

        public ConfirmationModal(Texture2D background) : base()
        {
            Visible = false;
            ZIndex = int.MaxValue / 10;

            _background = new Image(background)
            {
                Parent = this,
                ClipsBounds = false,
            };

            _titleLabel = new Label()
            {
                Parent = this,
                Font = GameService.Content.DefaultFont32,
                AutoSizeHeight = true,
            };

            _textLabel = new Label()
            {
                Parent = this,
                AutoSizeHeight = true,
                WrapText = true,
            };

            _confirmButton = new StandardButton() { Parent = this };
            _cancelButton = new StandardButton() { Parent = this };

            _confirmButton.Click += delegate { Hide(); _confirmed?.Invoke(); };
            _cancelButton.Click += delegate { Hide(); _cancelled?.Invoke(); };
        }

        public void Show(string title, string text, string confirm, string cancel, Action confirmed,
            Action cancelled = null)
        {
            Parent = GameService.Graphics.SpriteScreen;
            Location = Point.Zero;
            Size = Parent.Size;

            _confirmed = confirmed;
            _cancelled = cancelled;

            ModalWidth = 300;

            _titleLabel.Location = Point.Zero;
            _titleLabel.Width = ModalWidth;
            _titleLabel.Text = title;

            _textLabel.Top = _titleLabel.Bottom + Spacing;
            _textLabel.Left = 0;
            _textLabel.Width = ModalWidth;
            _textLabel.Text = text;

            _confirmButton.Text = confirm;
            _cancelButton.Text = cancel;
            _confirmButton.Top = _cancelButton.Top = _textLabel.Bottom + Spacing;
            var spacing = (ModalWidth - _confirmButton.Width - _cancelButton.Width) / 3;
            _confirmButton.Left = spacing;
            _cancelButton.Left = _confirmButton.Right + spacing;

            ModalHeight = _confirmButton.Bottom;

            _background.Location = Point.Zero - Margin;
            _background.Size = ModalSize + Margin + Margin;

            Show();
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            base.PaintBeforeChildren(spriteBatch, bounds);

            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, bounds, Color.Black * .3f);
        }

        protected override void DisposeControl()
        {
            _background.Texture?.Dispose();
            base.DisposeControl();
        }
    }
}
