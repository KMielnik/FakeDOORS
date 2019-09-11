using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReqTools
{
    public class DatabaseService : IDatabaseService
    {
        private const string defaultCachedFileName = "cached_reqs.json";
        private const string defaultServerCachedFileName = @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Support_Tools\FakeDOORS\Data\cached_reqs.json";

        private IReqParser reqParser;
        public DatabaseService(IReqParser reqParser)
        {
            this.reqParser = reqParser;
        }

        public event EventHandler RequirementsChanged;

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
            .ContinueWith((s) => RequirementsChanged?.Invoke(this, EventArgs.Empty));

        public async Task<List<Requirement>> GetRequirements()
        => (await reqParser.GetReqsFromCachedFile(defaultCachedFileName)).reqs;
        
    }
}
