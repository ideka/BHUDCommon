using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ideka.BHUDCommon.AnchoredRect;

public class AnchoredRect
{
    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;

    protected virtual Anchoring Anchoring { get; } = new();

    public virtual Vector2 AnchorMin { get => Anchoring.AnchorMin; set => Anchoring.AnchorMin = value; }
    public virtual Vector2 AnchorMax { get => Anchoring.AnchorMax; set => Anchoring.AnchorMax = value; }
    public virtual float AnchorMinX { get => Anchoring.AnchorMinX; set => Anchoring.AnchorMinX = value; }
    public virtual float AnchorMinY { get => Anchoring.AnchorMinY; set => Anchoring.AnchorMinY = value; }
    public virtual float AnchorMaxX { get => Anchoring.AnchorMaxX; set => Anchoring.AnchorMaxX = value; }
    public virtual float AnchorMaxY { get => Anchoring.AnchorMaxY; set => Anchoring.AnchorMaxY = value; }
    public virtual float AnchorX { set { AnchorMinX = AnchorMaxX = value; } }
    public virtual float AnchorY { set { AnchorMinY = AnchorMaxY = value; } }
    public Vector2 Anchor { set { AnchorMin = AnchorMax = value; } }

    public virtual Vector2 Pivot { get => Anchoring.Pivot; set => Anchoring.Pivot = value; }
    public virtual float PivotX { get => Anchoring.PivotX; set => Anchoring.PivotX = value; }
    public virtual float PivotY { get => Anchoring.PivotY; set => Anchoring.PivotY = value; }

    public virtual Vector2 Position { get => Anchoring.Position; set => Anchoring.Position = value; }
    public virtual float PositionX { get => Anchoring.PositionX; set => Anchoring.PositionX = value; }
    public virtual float PositionY { get => Anchoring.PositionY; set => Anchoring.PositionY = value; }

    public virtual Vector2 SizeDelta { get => Anchoring.SizeDelta; set => Anchoring.SizeDelta = value; }
    public virtual float SizeDeltaX { get => Anchoring.SizeDeltaX; set => Anchoring.SizeDeltaX = value; }
    public virtual float SizeDeltaY { get => Anchoring.SizeDeltaY; set => Anchoring.SizeDeltaY = value; }

    public event Action<AnchoredRect>? OnUpdate;

    public AnchoredRect? Parent { get; private set; }

    public List<(string, Func<string>)> DebugData { get; } = [];

    private readonly List<AnchoredRect> _children = [];
    public IReadOnlyList<AnchoredRect> Children => _children;

    public T InsertChild<T>(int index, T child) where T : AnchoredRect
    {
        Debug.Assert(child.Parent == null);
        child.Parent = this;
        _children.Insert(index, child);
        return child;
    }

    public T AddChild<T>(T child) where T : AnchoredRect
        => InsertChild(_children.Count, child);

    public void ClearChildren()
    {
        foreach (var child in _children)
            child.Parent = null;
        _children.Clear();
    }

    public bool RemoveChild(AnchoredRect child)
    {
        var removed = _children.Remove(child);
        if (removed)
            child.Parent = null;
        return removed;
    }

    public RectangleF Target(RectangleF container)
        => new RectangleF(
            new Vector2(
                container.X + container.Width * AnchorMin.X + Position.X - SizeDelta.X * Pivot.X,
                container.Y + container.Height * AnchorMin.Y + Position.Y - SizeDelta.Y * Pivot.Y),
            new Size2(
                (AnchorMax.X - AnchorMin.X) * container.Width + SizeDelta.X,
                (AnchorMax.Y - AnchorMin.Y) * container.Height + SizeDelta.Y));

    public static RectangleF ToCtrlF(Control ctrl, RectangleF target)
        => new RectangleF(
            target.X - ctrl.Left,
            target.Y - ctrl.Top,
            target.Width, target.Height);

    public static Rectangle ToCtrl(Control ctrl, RectangleF target)
        => ToCtrlF(ctrl, target).ToRectangle();

    public void Update(GameTime gameTime)
    {
        if (!Enabled)
            return;

        EarlyUpdate(gameTime);

        foreach (var child in _children)
            child.Update(gameTime);

        LateUpdate(gameTime);

        OnUpdate?.Invoke(this);
    }

    protected virtual void EarlyUpdate(GameTime gameTime) { }
    protected virtual void LateUpdate(GameTime gameTime) { }

    public void Draw(SpriteBatch spriteBatch, Control control, RectangleF rect)
    {
        if (!Visible || !Enabled)
            return;

        rect = Target(rect);

        var target = new RectTarget(spriteBatch, control, rect);

        EarlyDraw(target);

        foreach (var child in _children)
            child.Draw(spriteBatch, control, rect);

        LateDraw(target);
    }

    protected virtual void EarlyDraw(RectTarget target) { }
    protected virtual void LateDraw(RectTarget target) { }

    public readonly struct RectTarget(SpriteBatch spriteBatch, Control control, RectangleF rect)
    {
        public SpriteBatch SpriteBatch { get; } = spriteBatch;
        public Control Control { get; } = control;
        public RectangleF Rect { get; } = rect;
        public Rectangle ControlRect { get; } = ToCtrl(control, rect);

        public void DrawString(string text, BitmapFont font, Color color,
            bool wrap = false, bool stroke = false, int strokeDistance = 0,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Middle)
        {
            SpriteBatch.DrawStringOnCtrl(Control, text, font, ControlRect, color, wrap, stroke, strokeDistance,
                horizontalAlignment, verticalAlignment);
        }
    }
}

public static class AnchoredRectExtensions
{
    public static T With<T>(this T anchoredRect, Action<T> act) where T : AnchoredRect
    {
        act(anchoredRect);
        return anchoredRect;
    }

    public static T WithUpdate<T>(this T anchoredRect, Action<T> update) where T : AnchoredRect
    {
        anchoredRect.OnUpdate += delegate { update(anchoredRect); };
        return anchoredRect;
    }
}
