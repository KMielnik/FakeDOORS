using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReqTools
{
    public interface IDatabaseService
    {
        event EventHandler RequirementsChanged;
        Task Init();
        List<Requirement> GetRequirements();
        Task<List<TestCase>> GetTestCases();
        Task<bool> CheckForUpdates();
        Task DownloadNewestVersion();
    }
}
