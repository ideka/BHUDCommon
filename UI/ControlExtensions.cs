using Blish_HUD;
using Blish_HUD.Controls;
using System;

namespace Ideka.BHUDCommon;

public static class ControlExtensions
{
    private static void Arrange(this Control from, int spacing, Func<Control, int> pGet, Action<Control, int> nSet, params Control[] others)
    {
        var prev = from;
        foreach (var control in others)
        {
            control.Location = prev.Location;
            nSet(control, pGet(prev) + spacing);
            prev = control;
        }
    }

    public static void ArrangeTopDown(this Control from, int spacing, params Control[] others)
        => from.Arrange(spacing, c => c.Bottom, (c, x) => c.Top = x, others);

    public static void ArrangeBottomUp(this Control from, int spacing, params Control[] others)
        => from.Arrange(-spacing, c => c.Top, (c, x) => c.Bottom = x, others);

    public static void ArrangeLeftRight(this Control from, int spacing, params Control[] others)
        => from.Arrange(spacing, c => c.Right, (c, x) => c.Left = x, others);

    public static void ArrangeRightLeft(this Control from, int spacing, params Control[] others)
        => from.Arrange(-spacing, c => c.Left, (c, x) => c.Right = x, others);

    public static void WidthFillRight(this Control control, int spacing = 0)
        => control.Width = control.Parent.ContentRegion.Width - control.Left - spacing;

    public static void WidthFillLeft(this Control control, int spacing = 0)
    {
        control.Width = control.Right - spacing * 2;
        control.Left = spacing;
    }

    public static void HeightFillDown(this Control control, int spacing = 0)
        => control.Height = control.Parent.ContentRegion.Height - control.Top - spacing;

    public static void HeightFillUp(this Control control, int spacing = 0)
    {
        control.Height = control.Bottom - spacing * 2;
        control.Top = spacing;
    }

    public static void CenterWith(this Control control, Control other)
        => control.Left = other.Left + other.Width / 2 - control.Width / 2;

    public static void MiddleWith(this Control control, Control other)
        => control.Top = other.Top + other.Height / 2 - control.Height / 2;

    public static void HorizontalAlign(this Control control, float p)
        => control.Left = (int)(control.Parent.ContentRegion.Width * p - control.Width * p);

    public static void AlignCenter(this Control control)
        => HorizontalAlign(control, .5f);

    public static void AlignRight(this Control control)
        => HorizontalAlign(control, 1);

    public static void VerticalAlign(this Control control, float p)
        => control.Top = (int)(control.Parent.ContentRegion.Height * p - control.Height * p);

    public static void AlignMiddle(this Control control)
        => VerticalAlign(control, .5f);

    public static void AlignBottom(this Control control)
        => VerticalAlign(control, 1);

    public static void SetContentRegionWidth(this Container container, int width)
    {
        var diff = container.Width - container.ContentRegion.Width;
        container.Width = width + diff;
    }

    public static void SetContentRegionHeight(this Container container, int height)
    {
        var diff = container.Height - container.ContentRegion.Height;
        container.Height = height + diff;
    }

    public static bool IsVisible(this Control control)
    {
        while (control != GameService.Graphics.SpriteScreen)
        {
            if (control?.Visible != true)
                return false;
            control = control.Parent;
        }

        return true;
    }

    public static float NearestScrollTarget(this Panel panel, Control control)
    {
        var viewportTop = panel.VerticalScrollOffset;
        var viewportHeight = panel.ContentRegion.Height;

        if (control.Top < viewportTop)
            return control.Top;
        else if (control.Bottom > viewportTop + viewportHeight)
            return control.Bottom - viewportHeight;

        return -1;
    }
}
