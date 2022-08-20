using Blish_HUD;
using Blish_HUD.Settings;
using System;

namespace Ideka.BHUDCommon
{
    public class GenericSetting<T> : IDisposable
    {
        public SettingEntry<T> Setting { get; private set; }

        public T Value
        {
            get => Setting.Value;
            set => Setting.Value = value;
        }

        private Action<T> _changed;

        protected GenericSetting()
        {
        }

        public GenericSetting(SettingCollection settings,
            string key, T defaultValue, Func<string> displayNameFunc, Func<string> descriptionFunc)
        {
            Initialize(settings.DefineSetting(key, defaultValue, displayNameFunc, descriptionFunc));
        }

        protected void Initialize(SettingEntry<T> setting)
        {
            Setting = setting;
            Setting.SettingChanged += SettingChanged;
        }

        public void OnChanged(Action<T> changed)
        {
            _changed = changed;
        }

        public void OnChangedAndNow(Action<T> changed)
        {
            _changed = changed;
            changed(Value);
        }

        protected virtual void SettingChanged(object sender, ValueChangedEventArgs<T> e)
        {
            _changed?.Invoke(Value);
        }

        public virtual void Dispose()
        {
            Setting.SettingChanged -= SettingChanged;
        }
    }
}
