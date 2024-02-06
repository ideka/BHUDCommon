using System;
using System.Linq;

namespace Ideka.BHUDCommon;

public class StringBox(Func<string, string>? validator = null) : ValueTextBox<string>("")
{
    private bool _hideValue;
    public bool HideValue
    {
        get => _hideValue;
        set
        {
            _hideValue = value;

            var prev = Value;
            TryReflectValue(ref prev);
        }
    }

    private readonly Func<string, string> _validator = validator ?? (s => s);

    public StringBox() : this(null)
    {
    }

    protected override bool TryMakeValue(string innerValue, out string value)
    {
        value = _validator(innerValue);
        return value != null;
    }

    protected override bool TryMakeText(ref string value, out string text)
    {
        value = _validator(value);

        text = HideValue
            ? string.Join("", value.Select(_ => '*'))
            : value;

        return value != null;
    }
}
