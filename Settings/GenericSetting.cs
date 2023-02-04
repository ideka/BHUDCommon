using Blish_HUD;
using Blish_HUD.Settings;
using Ideka.NetCommon;
using System;

namespace Ideka.BHUDCommon;

#nullable disable

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

    public IDisposable OnChanged(Action<T> changed)
    {
        _changed += changed;
        return new WhenDisposed(() => _changed -= changed);
    }

    public IDisposable OnChangedAndNow(Action<T> changed)
    {
        changed(Value);
        return OnChanged(changed);
    }

    protected virtual void SettingChanged(object sender, ValueChangedEventArgs<T> e) => _changed?.Invoke(Value);

    public virtual void Dispose()
    {
        Setting.SettingChanged -= SettingChanged;
    }
}
