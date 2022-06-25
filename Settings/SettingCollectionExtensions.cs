using Blish_HUD.Input;
using Blish_HUD.Settings;
using System;

namespace Ideka.BHUDCommon
{
    public static class SettingCollectionExtensions
    {
        public static GenericSetting<T> Generic<T>(this SettingCollection settings, string key,
            T defaultValue, Func<string> displayNameFunc = null, Func<string> descriptionFunc = null)
                => new GenericSetting<T>(settings, key, defaultValue, displayNameFunc, descriptionFunc);

        public static KeyBindingSetting KeyBinding(this SettingCollection settings, string key,
            KeyBinding defaultValue, Func<string> displayNameFunc = null, Func<string> descriptionFunc = null)
                => new KeyBindingSetting(settings, key, defaultValue, displayNameFunc, descriptionFunc);

        public static SliderSetting Slider(this SettingCollection settings, string key,
            int defaultValue, int minValue, int maxValue, Func<string> displayNameFunc = null,
            Func<string> descriptionFunc = null, Func<int, int, string> validationErrorFunc = null)
                => new SliderSetting(settings, key, defaultValue, minValue, maxValue,
                    displayNameFunc, descriptionFunc, validationErrorFunc);

        public static ReflectedSetting<float> PercentageSlider(this SettingCollection settings, string key,
            float defaultPercentage, float minPercentage, float maxPercentage, Func<string> displayNameFunc = null,
            Func<string> descriptionFunc = null, Func<string, string, string> validationErrorFunc = null)
        {
            string format(float p) => $"{p:P}";

            var setting = new CustomReflectedSetting<float, float?>(settings, key, defaultPercentage, format, str =>
            {
                if (!float.TryParse(str.TrimEnd('%'), out var raw))
                    return null;
                float value = raw / 100f;
                return value < minPercentage || value > maxPercentage ? (float?)null : value;
            }, displayNameFunc, descriptionFunc, ()
                => validationErrorFunc?.Invoke(format(minPercentage), format(maxPercentage)));

            setting.Setting.SetRange(minPercentage, maxPercentage);
            return setting;
        }
    }
}
