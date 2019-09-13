using FakeDOORS.DatabaseControls.RequirementsControls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace FakeDOORS
{
    public interface IRequirementsView
    {
        void SetReqView(ReqViewSettings settings);
        Task SetSelectedTestCases(List<int> testCases);
        event RoutedEventHandler Loaded;
    }
}