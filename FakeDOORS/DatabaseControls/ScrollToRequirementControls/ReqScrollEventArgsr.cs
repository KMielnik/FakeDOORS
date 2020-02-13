using System;

namespace FakeDOORS.DatabaseControls.ScrollToRequirementControls
{
    public class ReqScrollEventArgsr : EventArgs
    {
        public string RequestedReq;

        public ReqScrollEventArgsr(string requestedReq)
        {
            RequestedReq = requestedReq;
        }
    }
}