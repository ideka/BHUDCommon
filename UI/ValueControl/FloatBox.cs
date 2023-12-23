﻿using Blish_HUD.Input;
using Ideka.BHUDCommon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Ideka.BHUDCommon;

public class FloatBox : ValueTextBox<float>
{
    private static bool HoldingAlt
        => Input.Keyboard.KeysDown.Contains(Keys.LeftAlt) || Input.Keyboard.KeysDown.Contains(Keys.RightAlt);

    private static bool HoldingShift
        => Input.Keyboard.KeysDown.Contains(Keys.LeftShift) || Input.Keyboard.KeysDown.Contains(Keys.RightShift);

    private float _minValue = float.MinValue;
    public float MinValue
    {
        get => _minValue;
        set
        {
            if (_minValue == value)
                return;
            _minValue = value;
            if (Value < MinValue)
                CommitValue(MinValue);
        }
    }

    public float XScale { get; set; } = 1;
    public float YScale { get; set; } = -1;
    public float Scale { get; set; } = 0.01f;

    private readonly KeyBinding _dragCancel;

    private readonly EscBlockWindow _escBlock;
    private Point? _dragStart = null;
    private Vector2 _dragAmount = Vector2.Zero;

    public FloatBox() : base(0)
    {
        _escBlock = new EscBlockWindow(this);

        Spacing = 10;

        _dragCancel = new KeyBinding(Keys.Escape);
        _dragCancel.Enabled = _dragCancel.BlockSequenceFromGw2 = false;

        Input.Mouse.LeftMouseButtonPressed += LeftMouseButtonPressed;
        Input.Mouse.MouseMoved += MouseMoved;
        Input.Mouse.LeftMouseButtonReleased += LeftMouseButtonReleased;
        _dragCancel.Activated += DragCancel;
    }

    protected override bool TryMakeText(ref float value, out string text)
    {
        value = Math.Max(value, MinValue);
        text = $"{value}";
        return true;
    }

    protected override bool TryMakeValue(string innerValue, out float value)
    {
        if (!float.TryParse(innerValue, out value))
            return false;
        value = Math.Max(value, MinValue);
        return true;
    }

    private float DragValue() => (_dragAmount.X * XScale + _dragAmount.Y * YScale) * Scale;

    private void DragStart()
    {
        _escBlock.BlockStart();

        _dragStart = Input.Mouse.PositionRaw;
        _dragAmount = Vector2.Zero;

        _dragCancel.Enabled = _dragCancel.BlockSequenceFromGw2 = true;
    }

    private void DragEnd(bool commit)
    {
        _escBlock.BlockEnd();

        _dragStart = null;
        _dragCancel.Enabled = _dragCancel.BlockSequenceFromGw2 = false;
        UnsetTempValue();

        if (commit)
            CommitValue(Value + DragValue());

        _dragAmount = Vector2.Zero;
    }

    private bool HandleHidden()
    {
        var isHidden = !this.IsVisible();
        if (isHidden && _dragStart != null)
            DragEnd(false);
        return isHidden;
    }

    private new void LeftMouseButtonPressed(object sender, MouseEventArgs e)
    {
        if (HandleHidden()) return;

        if (_dragStart.HasValue)
            DragEnd(false);

        if (MouseOver && !MouseOverControl)
            DragStart();
    }

    private new void MouseMoved(object sender, MouseEventArgs e)
    {
        if (_dragStart is not Point start || HandleHidden())
            return;

        _dragAmount += (Input.Mouse.PositionRaw - start).ToVector2() *
            (HoldingAlt ? .1f : 1) *
            (HoldingShift ? 10 : 1);

        // Don't go under MinValue
        if (Value + DragValue() < MinValue)
            _dragAmount = new Vector2((MinValue - Value) / Scale / XScale, 0);

        Mouse.SetPosition(start.X, start.Y);

        SetTempValue(Value + DragValue(), true);
    }

    private new void LeftMouseButtonReleased(object sender, MouseEventArgs e)
    {
        if (HandleHidden()) return;

        if (_dragStart.HasValue)
            DragEnd(true);
    }

    private void DragCancel(object sender, EventArgs e)
    {
        if (HandleHidden()) return;

        if (_dragStart.HasValue)
            DragEnd(false);
    }

    protected override void DisposeControl()
    {
        if (_dragStart.HasValue)
            DragEnd(false);

        _dragCancel.Enabled = _dragCancel.BlockSequenceFromGw2 = false;

        Input.Mouse.LeftMouseButtonPressed -= LeftMouseButtonPressed;
        Input.Mouse.MouseMoved -= MouseMoved;
        Input.Mouse.LeftMouseButtonReleased -= LeftMouseButtonReleased;
        _dragCancel.Activated -= DragCancel;

        _escBlock?.Dispose();

        base.DisposeControl();
    }
}
