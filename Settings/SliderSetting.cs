using Blish_HUD.Settings;
using System;

namespace Ideka.BHUDCommon;

#nullable disable

public class SliderSetting : ReflectedSetting<int>
{
    private readonly int _minValue;
    private readonly int _maxValue;

    public SliderSetting(SettingCollection settings,
        string key, int defaultValue, int minValue, int maxValue,
        Func<string> displayNameFunc, Func<string> descriptionFunc, Func<int, int, string> validationErrorFunc)
    {
        _minValue = minValue;
        _maxValue = maxValue;

        Initialize(settings, key, defaultValue, displayNameFunc, descriptionFunc,
            () => validationErrorFunc?.Invoke(minValue, maxValue));

        Setting.SetRange(_minValue, _maxValue);
    }

    protected override string GetString(int from)
        => $"{from}";

    protected override bool TryGetValue(string str, out int value)
        => int.TryParse(str, out value) && value > _minValue && value < _maxValue;
}
