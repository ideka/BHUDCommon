using Gw2Sharp.Mumble.Models;
using Ideka.NetCommon;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using static Blish_HUD.GameService;

namespace Ideka.BHUDCommon;

public static class UIOverlay
{
    public static float GetScale()
        =>  Math.Min(
                Math.Min(Graphics.WindowWidth, 1024) / 1024f,
                Math.Min(Graphics.WindowHeight, 768) / 768f) * Gw2Mumble.UI.UISize switch
            {
                UiSize.Small => .9f,
                UiSize.Large => 1.1f,
                UiSize.Larger => 1.1f * 1.1f,
                _ => 1,
            };

    public static float ToWindowScale()
        => GetScale() / Graphics.WindowHeight * Graphics.SpriteScreen.AbsoluteBounds.Height;

    public static Vector2 GetHealthCenter()
        => new Vector2(
            .5005f * Graphics.SpriteScreen.AbsoluteBounds.Width,
            Graphics.SpriteScreen.AbsoluteBounds.Height - 65.5f * ToWindowScale());

    public static float GetHealthRadius()
        => 51 * ToWindowScale();

    public static float GetEnduranceThickness()
        => 7 * ToWindowScale();

    public static (Vector2, Vector2) GetEndurancePoints(double x)
    {
        var p = MathUtils.Lerp(-360 * .175, +360 * .175, x);
        var radians = (p - 90) * MathUtils.DegToRad;
        var cossin = new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));

        var start = GetHealthCenter() + cossin * GetHealthRadius();
        var end = start + cossin * GetEnduranceThickness();

        return (start, end);
    }

    public static RectangleF GetCastbar()
    {
        var scale = ToWindowScale();
        var width = 195 * scale;
        var height = 15 * scale;
        var bottom = 224 * scale;
        return new RectangleF(
            Graphics.SpriteScreen.AbsoluteBounds.Width * .5f - width / 2,
            Graphics.SpriteScreen.AbsoluteBounds.Height - bottom - height,
            width,
            height);
    }
}
