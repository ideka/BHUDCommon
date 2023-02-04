using Blish_HUD;
using Blish_HUD.Settings;
using System;

namespace Ideka.BHUDCommon;

#nullable disable

public abstract class ReflectedSetting<T> : GenericSetting<T>
{
    private SettingEntry<string> _string;
    private bool _reflecting;

    protected void Initialize(SettingCollection settings,
        string key, T defaultValue,
        Func<string> displayNameFunc,
        Func<string> descriptionFunc,
        Func<string> validationErrorFunc)
    {
        _string = settings.DefineSetting($"{key}Str", "", displayNameFunc, descriptionFunc);
        var setting = settings.DefineSetting($"{key}", defaultValue, () => " ", descriptionFunc);

        _string.Value = GetString(setting.Value);

        _string.SetValidation(str => !TryGetValue(str, out T value)
            ? new SettingValidationResult(false, validationErrorFunc?.Invoke())
            : new SettingValidationResult(true));

        _string.SettingChanged += StringChanged;

        Initialize(setting);
    }

    protected abstract string GetString(T from);
    protected abstract bool TryGetValue(string from, out T value);

    private void StringChanged(object sender, ValueChangedEventArgs<string> e)
    {
        if (_reflecting)
            return;

        _reflecting = true;
        if (TryGetValue(_string.Value, out T val))
            Setting.Value = val;
        else
            _string.Value = GetString(Setting.Value);
        _reflecting = false;
    }

    protected override void SettingChanged(object sender, ValueChangedEventArgs<T> e)
    {
        base.SettingChanged(sender, e);

        if (_reflecting)
            return;

        _reflecting = true;
        _string.Value = GetString(Setting.Value);
        _reflecting = false;
    }

    public override void Dispose()
    {
        _string.SettingChanged -= StringChanged;
        base.Dispose();
    }
}

public class CustomReflectedSetting<T, U> : ReflectedSetting<T>
{
    private readonly Func<T, string> _getString;
    private readonly Func<string, U> _getValue;

    public CustomReflectedSetting(SettingCollection settings,
        string key, T defaultValue,
        Func<T, string> getString,
        Func<string, U> getValue,
        Func<string> displayNameFunc,
        Func<string> descriptionFunc,
        Func<string> validationErrorFunc)
    {
        _getString = getString;
        _getValue = getValue;

        Initialize(settings, key, defaultValue, displayNameFunc, descriptionFunc, validationErrorFunc);
    }

    protected override string GetString(T from)
        => _getString(from);

    protected override bool TryGetValue(string from, out T value)
    {
        dynamic val = _getValue(from);

        // Must be done like this to avoid weird race condition errors
        if (val is T concreteValue)
        {
            value = concreteValue;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }
}
