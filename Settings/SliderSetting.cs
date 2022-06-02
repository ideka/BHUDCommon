using Blish_HUD;
using Blish_HUD.Settings;
using System;

namespace Ideka.BHUDCommon
{
    public class SliderSetting : GenericSetting<int>
    {
        private readonly SettingEntry<string> _string;

        private readonly int _minValue;
        private readonly int _maxValue;

        private bool _reflecting;

        public SliderSetting(SettingCollection settings,
            string key, int defaultValue, int minValue, int maxValue,
            Func<string> displayNameFunc, Func<string> descriptionFunc, Func<int, int, string> validationErrorFunc)
        {
            _minValue = minValue;
            _maxValue = maxValue;

            _string = settings.DefineSetting($"{key}Str", "", displayNameFunc, descriptionFunc);
            var setting = settings.DefineSetting($"{key}", defaultValue, () => " ", descriptionFunc);
            setting.SetRange(_minValue, _maxValue);

            _string.Value = $"{setting.Value}";

            _string.SetValidation(str => !Validate(str, out int _)
                ? new SettingValidationResult(false, validationErrorFunc?.Invoke(_minValue, _maxValue))
                : new SettingValidationResult(true));

            _string.SettingChanged += StringChanged;

            Initialize(setting);
        }

        private bool Validate(string str, out int value)
            => int.TryParse(str, out value) && value > _minValue && value < _maxValue;

        private void StringChanged(object sender, ValueChangedEventArgs<string> e)
        {
            if (_reflecting)
                return;

            _reflecting = true;
            if (Validate(_string.Value, out int val))
                Setting.Value = val;
            else
                _string.Value = $"{Setting.Value}";
            _reflecting = false;
        }

        protected override void SettingChanged(object sender, ValueChangedEventArgs<int> e)
        {
            base.SettingChanged(sender, e);

            if (_reflecting)
                return;

            _reflecting = true;
            _string.Value = $"{Setting.Value}";
            _reflecting = false;
        }

        public override void Dispose()
        {
            base.Dispose();
            _string.SettingChanged -= StringChanged;
        }
    }
}
