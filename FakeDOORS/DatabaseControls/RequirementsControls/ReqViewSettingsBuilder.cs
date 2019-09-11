using System;
using System.Collections.Generic;
using System.Text;

namespace FakeDOORS.DatabaseControls.RequirementsControls
{
    public class ReqViewSettingsBuilder
    {
        private ReqViewSettings reqViewSettings;
        public ReqViewSettingsBuilder()
        {
            reqViewSettings = new ReqViewSettings();
        }

        public ReqViewSettingsBuilder AddDefaultSettings()
        {
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.IDColumn);
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.TextColumn);
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.FVariant);
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.BoldHeaders);

            return this;
        }

        public ReqViewSettings Build()
        => reqViewSettings;
    }
}
