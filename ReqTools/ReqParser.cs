using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MoreLinq.Extensions.BatchExtension;

namespace ReqTools
{
    public class ReqParser : IReqParser
    {
        private async Task<HtmlDocument> LoadDocumentFromString(string text)
        => await Task.Run(() =>
        {
            var document = new HtmlDocument();
            document.LoadHtml(text);
            return document;
        }).ConfigureAwait(false);

        private async Task<IEnumerable<string>> LoadExportFileInParts(string filename)
        => await Task.Run(() =>
        {
            var file = File.ReadAllText(filename);
            file = file.Replace("<br>", "\t");
            file = Regex.Replace(file, @"^</a>", "");
            var body = Regex.Match(file, @"<BODY .+?>").Value;
            file = file.Substring(file.IndexOf(body) + body.Length);

            const int batchSize = 50;

            var parts = Regex.Split(file, @"<a name=.+?>")
                .Batch(batchSize)
                .Select(x => x.Aggregate(new StringBuilder(), (acc, y) => acc.AppendLine(y)).ToString());

            var lastPart = parts.Last();
            parts = parts
                .Reverse()
                .Skip(1)
                .Reverse();

            lastPart = Regex.Replace(lastPart, @"<DIV.+?Produced by DOORS.+?</DIV>", "");

            parts = parts.Append(lastPart);

            return parts;
        });

        private Task<List<Requirement>> GetRequiermentsList(HtmlDocument document)
        => Task.Run(() =>
        {
            var divs = document
                .DocumentNode
                .SelectNodes("/div")
                .ToList();

            var requirments = divs
                .AsParallel().AsOrdered()
                .Select(x =>
                {
                    var reqStrings = x
                        .InnerText
                        .Trim()
                        .Split('\n')
                        .Where(y => !string.IsNullOrWhiteSpace(y))
                        .Select(WebUtility.HtmlDecode);

                    var ID = reqStrings
                        .FirstOrDefault()
                        ?.Trim()
                        .Substring(4);

                    var text = reqStrings
                        .Skip(1)
                        .FirstOrDefault()
                        ?.Trim();

                    var margin = int.Parse(x
                        .GetAttributeValue("style", "0")
                        .Replace("margin-left: ", string.Empty)
                        .Replace("px", string.Empty));

                    int indentLevel = margin / 36;

                    var TCs = reqStrings
                        .Where(y => y.Contains("TC ID & Title"))
                        .FirstOrDefault()
                        ?.Replace("TC ID & Title:", string.Empty)
                        .Trim()
                        .Split('\t')
                        .Where(y => !string.IsNullOrWhiteSpace(y))
                        .Select(y =>
                        {
                            var id = Regex.Replace(y, @"^[0-9]+?\) ", string.Empty);
                            id = Regex.Replace(id, @" - .*", string.Empty);

                            var tcText = Regex.Match(y, "TC.*").Value;

                            string ValidFromTC, ValidToTC;
                            var validFromTo = Regex.Match(y, @"\[[0-9./-]+\]");
                            if (validFromTo.Success == false)
                            {
                                ValidFromTC = "-";
                                ValidToTC = "-";
                            }
                            else
                            {
                                var ValidFromToValues = validFromTo
                                    .Value
                                    .Replace("[", string.Empty)
                                    .Replace("]", string.Empty)
                                    .Split('/')
                                    .Select(z => Regex.Match(z, @"\d{2}\.\d").Value)
                                    .Select(z => string.IsNullOrWhiteSpace(z) || z == "." ? "-" : z)
                                    .ToArray();

                                if (ValidFromToValues.Length >= 2)
                                {
                                    ValidFromTC = ValidFromToValues[0];
                                    ValidToTC = ValidFromToValues[1];
                                }
                                else
                                {
                                    ValidFromTC = ValidFromToValues[0];
                                    ValidToTC = ValidFromToValues[0];
                                }
                            }

                            return new TestCase(id, tcText, (ValidFromTC, ValidToTC));
                        })
                        .ToList();

                    var fVariants = reqStrings
                        .SkipWhile(y => !y.Contains("Functional Variants (use CTRL-R for edit):"))
                        .TakeWhile(y => !y.Contains("Hardware Variants (use CTRL-R for edit):"))
                        .Select(y => y.Trim())
                        .Aggregate("", (acc, y) => acc + y)
                        .Replace("Functional Variants (use CTRL-R for edit):", string.Empty)
                        .Trim();

                    var type = reqStrings
                        .Where(y => y.Contains("Object Type:"))
                        .FirstOrDefault()
                        .Replace("Object Type:", string.Empty)
                        .Trim() switch
                    {
                        "Info" => Requirement.Types.Info,
                        "Head" => Requirement.Types.Head,
                        "Req" => Requirement.Types.Req,
                        _ => Requirement.Types.Req
                    };

                    var status = reqStrings
                        .Where(y => y.Contains("Status:"))
                        .FirstOrDefault()
                        .Replace("Status:", string.Empty)
                        .Trim() switch
                    {
                        "approved" => Requirement.Statuses.Approved,
                        "draft" => Requirement.Statuses.Draft,
                        "dropped" => Requirement.Statuses.Dropped,
                        "stable" => Requirement.Statuses.Stable,
                        _ => Requirement.Statuses.NA
                    };

                    var ValidFrom = reqStrings
                        .Where(y => y.Contains("ValidFrom:", StringComparison.InvariantCulture))
                        .Select(y => Regex.Match(y, @"\d\d\.\d").Value)
                        .Select(y => string.IsNullOrWhiteSpace(y) ? "-" : y)
                        .FirstOrDefault();

                    var ValidTo = reqStrings
                        .Where(y => y.Contains("ValidTo:"))
                        .Select(y => Regex.Match(y, @"\d\d\.\d").Value)
                        .Select(y => string.IsNullOrWhiteSpace(y) ? "-" : y)
                        .FirstOrDefault();


                    return new Requirement(
                        ID,
                        text,
                        indentLevel,
                        TCs,
                        fVariants,
                        type,
                        status,
                        ValidFrom,
                        ValidTo
                       );
                })
                .ToList();

            return requirments;
        });

        public async Task<(List<Requirement> reqs, DateTime exportDate)> GetReqsFromCachedFile(string filename)
        {
            string exportDateJson;
            string reqJson;

            if (!File.Exists(filename))
                return (new List<Requirement>(), DateTime.MinValue);

            var lines = File.ReadAllLines(filename);

            exportDateJson = lines[0];
            reqJson = lines[1];

            return await Task.Run(() =>
            (
                JsonConvert.DeserializeObject<List<Requirement>>(reqJson),
                JsonConvert.DeserializeObject<DateTime>(exportDateJson)
            ));
        }

        public async Task ParseToFileAsync(IProgress<string> progress, string input, string output)
        {
            var parseData = await Parse(progress, input);

            File.WriteAllLines(output,
                new[] {
                    JsonConvert.SerializeObject(parseData.exportDate),
                    JsonConvert.SerializeObject(parseData.reqs)
                });
        }

        public async Task<(List<Requirement> reqs, DateTime exportDate)> Parse(IProgress<string> progress, string input)
        {
            progress.Report("Loading...");

            var clock = new Stopwatch();
            clock.Start();

            progress.Report("Splitting file in parts.");
            var documentTasks = (await LoadExportFileInParts(input))
                .AsParallel()
                .AsOrdered()
                .Select(LoadDocumentFromString)
                .ToList();

            var requirements = new List<Requirement>();

            for (int i = 0; i < documentTasks.Count; i++)
            {
                progress.Report($"Parsing file ({i}/{documentTasks.Count}).");
                var document = await documentTasks[i];
                var reqsPart = await GetRequiermentsList(document);
                requirements.AddRange(reqsPart);
            }

            clock.Stop();
            progress.Report("Saving to file...");
            progress.Report($"Done. (in {clock.Elapsed.Seconds}s.)");

            return (requirements, DateTime.Now);
        }
    }
}
