using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReqTools
{
    public class DatabaseService : IDatabaseService
    {
        private const string defaultCachedFileName = "cached_reqs.json";
        private const string defaultServerCachedFileName = @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Support_Tools\FakeDOORS\Data\cached_reqs.json";

        private IReqParser reqParser;

        private List<Requirement> requirements;
        private List<TestCase> allTestCases;
        private DateTime reqsExportDate;

        private string versionFilter;
        public DatabaseService(IReqParser reqParser)
        {
            this.reqParser = reqParser;
            requirements = new List<Requirement>();
            allTestCases = new List<TestCase>();
        }

        public event EventHandler RequirementsChanged;

        public async Task Init(string versionFilter)
        {
            this.versionFilter = versionFilter;

            if (File.Exists(defaultCachedFileName))
            {
                await RefreshCachedData();
                RequirementsChanged?.Invoke(this, EventArgs.Empty);
            }
            else
                await DownloadNewestVersion();
        }
        public async Task<bool> CheckForUpdates()
        => await Task.Run(() =>
        {
            string cachedDateJson = File.ReadAllLines(defaultCachedFileName)[0];
            string serverDateJson = File.ReadAllLines(defaultServerCachedFileName)[0];

            var cachedDate = JsonConvert.DeserializeObject<DateTime>(cachedDateJson);
            var serverDate = JsonConvert.DeserializeObject<DateTime>(serverDateJson);

            return serverDate > cachedDate;
        });

        public async Task DownloadNewestVersion()
        => await Task.Run(() => File.Copy(defaultServerCachedFileName, defaultCachedFileName, true))
            .ContinueWith(s => RefreshCachedData())
            .ContinueWith(s => RequirementsChanged?.Invoke(this, EventArgs.Empty));

        private async Task RefreshCachedData()
        {
            requirements.Clear();
            requirements.AddRange((await reqParser.GetReqsFromCachedFile(defaultCachedFileName)).reqs);

            allTestCases.Clear();
            allTestCases.AddRange(requirements
                .AsParallel()
                .SelectMany(x => x.TCs)
                .Distinct()
                .OrderBy(x => x.IDValue));
        }
        public List<Requirement> GetRequirements()
        => requirements
            .Where(x => x.IsValidInSpecifiedVersion(versionFilter))
            .ToList();

        public async Task<List<TestCase>> GetTestCases()
        => await Task.Run(() =>
         {
             return allTestCases
                .Where(x => x.IsValidInSpecifiedVersion(versionFilter))
                .ToList();
         });

        public string GetTestCaseText(int tc)
        {
            return allTestCases
                .First(x => x.IDValue == tc)
                .Text;
        }
    }
}
