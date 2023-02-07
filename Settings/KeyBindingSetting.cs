using Blish_HUD.Input;
using Blish_HUD.Settings;
using Ideka.NetCommon;
using System;

namespace Ideka.BHUDCommon;

#nullable disable

public class KeyBindingSetting : GenericSetting<KeyBinding>
{
    private Action _activated;
    private Action<KeyBinding> _bindingChanged;

    public KeyBindingSetting(SettingCollection settings,
        string key, KeyBinding defaultValue, Func<string> displayNameFunc, Func<string> descriptionFunc)
    {
        Initialize(settings.DefineSetting(key, defaultValue, displayNameFunc, descriptionFunc));
        Setting.Value.Enabled = true;
        Setting.Value.Activated += Activated;
        Setting.Value.BindingChanged += BindingChanged;
    }

    public IDisposable OnActivated(Action action)
    {
        _activated += action;
        return new WhenDisposed(() => _activated -= action);
    }

    public IDisposable OnBindingChanged(Action<KeyBinding> action)
    {
        _bindingChanged += action;
        return new WhenDisposed(() => _bindingChanged -= action);
    }

    public IDisposable OnBindingChangedAndNow(Action<KeyBinding> action)
    {
        action(Value);
        return OnBindingChanged(action);
    }

    private void Activated(object sender, EventArgs e) => _activated?.Invoke();

    private void BindingChanged(object sender, EventArgs e) => _bindingChanged?.Invoke(Value);

    public override void Dispose()
    {
        base.Dispose();
        Setting.Value.Enabled = false;
        Setting.Value.Activated -= Activated;
    }
}
