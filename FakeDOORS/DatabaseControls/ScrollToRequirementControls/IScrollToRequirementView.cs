using System;

namespace FakeDOORS.DatabaseControls.ScrollToRequirementControls
{
    public interface IScrollToRequirementView
    {
        event EventHandler<ReqScrollEventArgsr> ReqScrollRequested;
    }
}