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
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.FVariantColumn);
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.BoldHeaders);

            return this;
        }

        public ReqViewSettingsBuilder AddIDColumn()
        {
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.IDColumn);
            return this;
        }

        public ReqViewSettingsBuilder AddTextColumn()
        {
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.TextColumn);
            return this;
        }

        public ReqViewSettingsBuilder AddFVariantsColumn()
        {
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.FVariantColumn);
            return this;
        }

        public ReqViewSettingsBuilder SetBoldHeaders()
        {
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.BoldHeaders);
            return this;
        }

        public ReqViewSettingsBuilder AddStatusColumn()
        {
            reqViewSettings.AddSetting(ReqViewSettings.SettingTypes.StatusColumn);
            return this;
        }

        public ReqViewSettings Build()
        => reqViewSettings;
    }
}
