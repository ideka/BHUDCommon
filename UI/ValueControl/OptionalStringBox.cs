namespace Ideka.BHUDCommon;

public class OptionalStringBox : ValueTextBox<string?>
{
    public OptionalStringBox() : base(null)
    {
    }

    protected override bool TryMakeValue(string innerValue, out string? value)
    {
        value = string.IsNullOrEmpty(innerValue) ? null : innerValue;
        return true;
    }

    protected override bool TryMakeText(ref string? value, out string text)
    {
        text = value ?? "";
        return true;
    }
}
