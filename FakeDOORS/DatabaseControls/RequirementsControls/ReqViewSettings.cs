using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FakeDOORS.DatabaseControls.RequirementsControls
{
    public class ReqViewSettings : IEnumerable<ReqViewSettings.SettingTypes>
    {
        public enum SettingTypes
        {
            IDColumn,
            TextColumn,
            FVariant
        }

        private Queue<SettingTypes> Settings;

        public void AddSetting(SettingTypes setting)
        => Settings.Enqueue(setting);

        public IEnumerator<SettingTypes> GetEnumerator()
        {
            return Settings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public ReqViewSettings()
        {
            Settings = new Queue<SettingTypes>();
        }
    }
}
