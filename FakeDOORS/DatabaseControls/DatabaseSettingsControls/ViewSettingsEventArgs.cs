using FakeDOORS.DatabaseControls.RequirementsControls;
using System;
using System.Collections.Generic;
using System.Text;

namespace FakeDOORS.DatabaseControls.DatabaseSettingsControls
{
    public class ViewSettingsEventArgs : EventArgs
    {
        public string NewFilterVersion { get; set; }
        public ReqViewSettings Settings { get; set; }
        public bool ReqsNeedsReloading { get; set; } = false;
        public bool ColumnsNeedsReloading { get; set; } = false;
        public bool StyleNeedsReloading { get; set; } = false;
    }
}
