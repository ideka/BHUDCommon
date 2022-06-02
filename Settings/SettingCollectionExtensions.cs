using Blish_HUD.Input;
using Blish_HUD.Settings;
using System;

namespace Ideka.BHUDCommon
{
    internal static class SettingCollectionExtensions
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
    }
}
