using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReqTools
{
    public class FakeReqParser : IReqParser
    {
        private List<Requirement> FakeRequirements;
        public FakeReqParser()
        {
            InitializeFakeRequirementsList();
        }

        private void InitializeFakeRequirementsList()
        {
            var tcs = new List<TestCase>()
            {
                new TestCase("101","101 - Test if it works (M)",("-","-"))
            };

            FakeRequirements = new List<Requirement>()
            {
                new Requirement("1","1. Chapter One",0,new List<TestCase>(),"",Requirement.Types.Head,Requirement.Statuses.Approved,"-","-"),
                new Requirement("2","Req - 1",1,new List<TestCase>(),"Variant:Some Variant",Requirement.Types.Req,Requirement.Statuses.Approved,"-","-"),
                new Requirement("3","Req - 2",1,tcs,"",Requirement.Types.Req,Requirement.Statuses.Approved,"-","-")
            };
        }

        public Task<bool> CheckForUpdates()
        => Task.FromResult(false);

        public Task DownloadNewestVersion()
        => Task.CompletedTask;

        public Task<(List<Requirement> reqs, DateTime exportDate)> GetReqsFromCachedFile(string filename)
        => Task.FromResult((FakeRequirements, DateTime.Now));

        public Task<(List<Requirement> reqs, DateTime exportDate)> Parse(IProgress<string> progress, string input)
        => Task.FromResult((FakeRequirements, DateTime.Now));

        public Task ParseToFileAsync(IProgress<string> progress, string input, string output)
        => Task.CompletedTask;
    }
}
