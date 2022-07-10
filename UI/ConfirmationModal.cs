using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks;

namespace Ideka.BHUDCommon
{
    public class ConfirmationModal : Container
    {
        private static readonly Point Margin = new Point(20, 15);
        private const int Spacing = 10;
        private const int Border = 5;

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

        private readonly EscBlockWindow _escBlock;
        private readonly Image _border;
        private readonly Image _background;
        private readonly Label _titleLabel;
        private readonly Label _textLabel;
        private readonly StandardButton _confirmButton;
        private readonly KeyboundButton _cancelButton;

        private TaskCompletionSource<bool> _buttonClick;
        private Action _confirmed;
        private Action _canceled;

        public ConfirmationModal(Texture2D background) : base()
        {
            Visible = false;
            ZIndex = int.MaxValue / 10;

            _escBlock = new EscBlockWindow(this);

            _border = new Image()
            {
                Parent = this,
                ClipsBounds = false,
                BackgroundColor = new Color(Color.Black, .5f),
            };

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
            _cancelButton = new KeyboundButton(new KeyBinding(Keys.Escape))
            {
                Parent = this,
                KeybindEnabled = false,
                BasicTooltipText = null,
            };

            {
                void done(bool confirmed)
                {
                    Hide();

                    try
                    {
                        if (confirmed)
                            _confirmed?.Invoke();
                        else
                            _canceled?.Invoke();
                    }
                    finally
                    {
                        _buttonClick.SetResult(confirmed);
                    }
                }

                _confirmButton.Click += delegate { done(true); };
                _cancelButton.Click += delegate { done(false); };
            }
        }

        public void Show(string title, string text, string confirm, string cancel, Action confirmed = null,
            Action canceled = null)
        {
            Parent = GameService.Graphics.SpriteScreen;
            Location = Point.Zero;
            Size = Parent.Size;

            _buttonClick = new TaskCompletionSource<bool>();
            _confirmed = confirmed;
            _canceled = canceled;

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
            _border.Location = _background.Location - new Point(Border);
            _border.Size = _background.Size + new Point(Border * 2);

            Show();
        }

        public Task<bool> ShowAsync(string title, string text, string confirm, string cancel)
        {
            Show(title, text, confirm, cancel);
            return _buttonClick.Task;
        }

        public override void Show()
        {
            _escBlock.BlockStart();
            _cancelButton.KeybindEnabled = true;
            base.Show();
        }

        public override void Hide()
        {
            _escBlock.BlockEnd();
            _cancelButton.KeybindEnabled = false;
            base.Hide();
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, bounds, Color.Black * .3f);

            base.PaintBeforeChildren(spriteBatch, bounds);
        }

        protected override void DisposeControl()
        {
            _background.Texture?.Dispose();

            _escBlock?.Dispose();
            _cancelButton?.Dispose();

            base.DisposeControl();
        }
    }
}
