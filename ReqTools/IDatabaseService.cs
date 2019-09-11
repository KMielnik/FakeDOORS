using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReqTools
{
    public interface IDatabaseService
    {
        event EventHandler RequirementsChanged;
        Task<List<Requirement>> GetRequirements();
        Task<bool> CheckForUpdates();
        Task DownloadNewestVersion();
    }
}
