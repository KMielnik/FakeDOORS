using System;
using System.Collections.Generic;

namespace FakeDOORS.DatabaseControls.TestCasesControls
{
    public interface ITestCasesView
    {
        IEnumerable<int> SelectedTCs { get; }

        event EventHandler SelectionChanged;

        void AddSelectedTCs(List<int> tcs);
        void SetSelectedTCs(List<int> tcs);
    }
}