using System;
using System.Collections.Generic;
using System.Text;

namespace FakeDOORS.DatabaseControls.DatabaseSettingsControls
{
    interface IDatabaseSettingsView
    {
        event EventHandler<ViewSettingsEventArgs> ViewSettingsChanged;
    }
}
