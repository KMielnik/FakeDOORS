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
        Task ParseToFileAsync(IProgress<string> progress, string input, string output);
        DateTime GetCacheCreationDate();
        List<Requirement> GetRequirements();
        Task<List<TestCase>> GetTestCases();
        void ChangeVersionFilter(string newVersionFilter);
        IEnumerable<(string, int)> GetChapters();
        IEnumerable<Requirement> GetRequirementsFromChapter(int chapter);
        IEnumerable<TestCase> GetTestCasesFromChapter(int chapter);
        IEnumerable<TestCase> GetTestCasesFromReqList(IEnumerable<Requirement> reqs);
        string GetTestCaseText(int tc);
        Task<bool> CheckForUpdates();
        Task DownloadNewestVersion();
    }
}
