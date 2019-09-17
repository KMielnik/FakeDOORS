using System;
using System.Collections.Generic;

namespace FakeDOORS.DatabaseControls.TestCasesControls
{
    public interface ITestCasesView
    {
        IEnumerable<int> SelectedTCs { get; }

        event EventHandler SelectionChanged;
        event EventHandler SelectTCsForSelectedReqsButtonClicked;
        void AddSelectedTCs(List<int> tcs);
        void SetSelectedTCs(List<int> tcs);
    }
}