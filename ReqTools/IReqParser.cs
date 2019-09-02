using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReqTools
{
    interface IReqParser
    {
        Task ParseToFileAsync(IProgress<string> progress, string input, string output); 
        Task<(List<Requirement> reqs, DateTime exportDate)> GetReqsFromCachedFile(string filename);
        Task<bool> CheckForUpdates();
        Task DownloadNewestVersion();
    }
}
