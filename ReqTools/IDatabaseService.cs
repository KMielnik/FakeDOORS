using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReqTools
{
    public interface IDatabaseService
    {
        event EventHandler RequirementsChanged;
        Task Init(string versionFilter = "-");
        List<Requirement> GetRequirements();
        Task<List<TestCase>> GetTestCases();
        string GetTestCaseText(int tc);
        Task<bool> CheckForUpdates();
        Task DownloadNewestVersion();
    }
}
