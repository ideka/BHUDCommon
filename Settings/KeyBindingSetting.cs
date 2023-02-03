using Blish_HUD.Input;
using Blish_HUD.Settings;
using Ideka.NetCommon;
using System;

namespace Ideka.BHUDCommon
{
    public class KeyBindingSetting : GenericSetting<KeyBinding>
    {
        private Action _action;

        public KeyBindingSetting(SettingCollection settings,
            string key, KeyBinding defaultValue, Func<string> displayNameFunc, Func<string> descriptionFunc)
        {
            Initialize(settings.DefineSetting(key, defaultValue, displayNameFunc, descriptionFunc));
            Setting.Value.Enabled = true;
            Setting.Value.Activated += Activated;
        }

        public IDisposable OnActivated(Action action)
        {
            _action += action;
            return new WhenDisposed(() => _action -= action);
        }

        private void Activated(object sender, EventArgs e) => _action?.Invoke();

        public override void Dispose()
        {
            base.Dispose();
            Setting.Value.Enabled = false;
            Setting.Value.Activated -= Activated;
        }
    }
}
