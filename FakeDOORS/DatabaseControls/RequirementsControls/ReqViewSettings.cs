using System;
using System.Collections;
using System.Collections.Generic;

namespace FakeDOORS.DatabaseControls.RequirementsControls
{
    public class ReqViewSettings : IEnumerable<ReqViewSettings.SettingTypes>
    {
        public enum SettingTypes
        {
            IDColumn,
            TextColumn,
            FVariantColumn,
            StatusColumn,
            BoldHeaders,
            ValidFromToColumn
        }

        private Stack<SettingTypes> Settings;

        public void AddSetting(SettingTypes setting)
        => Settings.Push(setting);

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
            Settings = new Stack<SettingTypes>();
        }
    }
}
