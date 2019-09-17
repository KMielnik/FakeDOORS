using System;
using System.Collections.Generic;
using System.Text;

namespace FakeDOORS.DatabaseControls.DatabaseSettingsControls
{
    public class ViewSettingsEventArgs : EventArgs
    {
        public string NewFilterVersion { get; set; }
    }
}
