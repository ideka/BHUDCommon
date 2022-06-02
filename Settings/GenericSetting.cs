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

        public Action<T> Changed { get; set; }

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

        protected virtual void SettingChanged(object sender, ValueChangedEventArgs<T> e)
        {
            Changed?.Invoke(Setting.Value);
        }

        public virtual void Dispose()
        {
            Setting.SettingChanged -= SettingChanged;
        }
    }
}
