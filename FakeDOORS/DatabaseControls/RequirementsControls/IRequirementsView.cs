﻿using FakeDOORS.DatabaseControls.RequirementsControls;
using ReqTools;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace FakeDOORS
{
    public interface IRequirementsView
    {
        void SetReqView(ReqViewSettings settings, (bool column, bool style) needsReload);
        Task SetSelectedTestCases(List<int> testCases);
        IEnumerable<Requirement> GetSelectedRequirements();
        void LimitScrollingToOneChapter(int chapter);
        event RoutedEventHandler Loaded;
        bool ScrollToReq(string Req);
    }
}