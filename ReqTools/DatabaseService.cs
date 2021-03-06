﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReqTools
{
    public class DatabaseService : IDatabaseService
    {
        private const string defaultCachedFileName = "cached_reqs.json";
        private const string defaultServerCachedFileName = @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Support_Tools\FakeDOORS\cached_reqs.json";

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
                try
                {
                    await RefreshCachedData();
                }
                catch { };

                RequirementsChanged?.Invoke(this, EventArgs.Empty);
            }
            else
                try
                {
                    await DownloadNewestVersion();
                }
                catch { };
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
        {
            try
            {
                await Task.Run(() => File.Copy(defaultServerCachedFileName, defaultCachedFileName, true));
            }
            catch
            {
                return;
            }
            await RefreshCachedData();
            RequirementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private async Task RefreshCachedData()
        {
            try
            {
                var exportData = await reqParser.GetReqsFromCachedFile(defaultCachedFileName);
                requirements.Clear();
                requirements.AddRange(exportData.reqs);
                reqsExportDate = exportData.exportDate;

                allTestCases.Clear();
                allTestCases.AddRange(requirements
                    .AsParallel()
                    .SelectMany(x => x.TCs)
                    .Distinct()
                    .OrderBy(x => x.IDValue));
            }
            catch
            {
            }      
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

        public IEnumerable<(string, int)> GetChapters()
        => GetRequirements()
                .Where(x => x.Type == Requirement.Types.Head)
                .Select(x => (chapter: Regex.Match(x.Text, @"^\d+\.[\d.]+").Value, id: x.IDValue))
                .Where(x => !string.IsNullOrWhiteSpace(x.chapter));

        public IEnumerable<Requirement> GetRequirementsFromChapter(int chapter)
        {
            var firstChapterReq = GetRequirements()
                .SkipWhile(x => x.IDValue != chapter)
                .FirstOrDefault();

            var chapterLevel = firstChapterReq
                ?.Level;

            return GetRequirements()
                      .SkipWhile(x => x.IDValue != chapter)
                      .TakeWhile(x => x.Level > chapterLevel || x.IDValue == chapter);
        }

        public IEnumerable<TestCase> GetTestCasesFromChapter(int chapter)
        => GetRequirementsFromChapter(chapter)
                .SelectMany(x => x.TCs)
                .Distinct();

        public void ChangeVersionFilter(string newVersionFilter)
        {
            this.versionFilter = newVersionFilter;

            RequirementsChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public IEnumerable<TestCase> GetTestCasesFromReqList(IEnumerable<Requirement> reqs)
        => reqs
            .AsParallel()
            .SelectMany(x => x.TCs)
            .Distinct()
            .AsSequential();

        public async Task ParseToFileAsync(IProgress<string> progress, string input, string output)
        {
            await reqParser.ParseToFileAsync(progress, input, output);
        }

        public DateTime GetCacheCreationDate()
        => reqsExportDate;
    }
}
