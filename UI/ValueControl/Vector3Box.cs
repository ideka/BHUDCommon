using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using System;

namespace Ideka.BHUDCommon;

public class Vector3Box : Container
{
    public const int InnerSpacing = 10;
    public const int BoxWidth = 90;

    public Vector3 Value
    {
        get => new Vector3(_x.Value, _y.Value, _z.Value);
        set
        {
            _x.Value = value.X;
            _y.Value = value.Y;
            _z.Value = value.Z;
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

    public float XScale { set => _x.XScale = _y.XScale = _z.XScale = value; }
    public float YScale { set => _x.YScale = _y.YScale = _z.YScale = value; }
    public float Scale { set => _x.Scale = _y.Scale = _z.Scale = value; }

    public event Action<Vector3>? ValueCommitted;
    public event Action<Vector3>? TempValue;

    private readonly FloatBox _x;
    private readonly FloatBox _y;
    private readonly FloatBox _z;

    public Vector3Box() : base()
    {
        static string labeler(string x) => $"[ {x} ]";

        _x = new FloatBox()
        {
            Parent = this,
            Label = labeler("X"),
            Spacing = InnerSpacing,
            ControlWidth = BoxWidth,
        };

        _y = new FloatBox()
        {
            Parent = this,
            Label = labeler("Y"),
            Spacing = InnerSpacing,
            ControlWidth = BoxWidth,
        };

        _z = new FloatBox()
        {
            Parent = this,
            Label = labeler("Z"),
            Spacing = InnerSpacing,
            ControlWidth = BoxWidth,
        };

        {
            void temp(float? x = null, float? y = null, float? z = null)
                => TempValue?.Invoke(new Vector3(x ?? _x.Value, y ?? _y.Value, z ?? _z.Value));

            _x.TempValue += v => temp(x: v);
            _y.TempValue += v => temp(y: v);
            _z.TempValue += v => temp(z: v);
        }

        {
            void value() => ValueCommitted?.Invoke(Value);

            _x.ValueCommitted += _ => value();
            _y.ValueCommitted += _ => value();
            _z.ValueCommitted += _ => value();
        }

        Spacing = 20;
    }

    protected override void OnResized(ResizedEventArgs e)
    {
        base.OnResized(e);
        UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (_x == null)
            return;

        _x.Width =
        _y.Width =
        _z.Width = (int)((ContentRegion.Width - Spacing * 2) / 3f);

        _x.Left = 0;
        _y.Left = _x.Right + Spacing;
        _z.Left = _y.Right + Spacing;

        this.SetContentRegionHeight(_z.Bottom);
    }
}
