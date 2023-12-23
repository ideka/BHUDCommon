namespace Ideka.BHUDCommon;

public abstract class ValueTextBox<TValue> : ValueControl<TValue, string, TextBoxFix>
{
    public ValueTextBox(TValue start) : base(start)
    {
        Control.TextChanged += delegate
        {
            if (TryMakeValue(Control.Text, out var newValue))
                SetTempValue(newValue, false);
        };

        Control.InputFocusChanged += delegate
        {
            if (Control.Focused)
                return;

            if (!TryMakeValue(Control.Text, out var newValue))
            {
                ResetValue();
                return;
            }

            if (Value?.Equals(newValue) != true)
                CommitValue(newValue);

            Value = newValue;
        };
    }

    protected sealed override bool TryReflectValue(ref TValue value)
    {
        if (!TryMakeText(ref value, out var text))
            return false;
        Control.Text = text;
        return true;
    }

    protected abstract bool TryMakeText(ref TValue value, out string text);
}
