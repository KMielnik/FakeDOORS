﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReqTools
{
    public interface IReqParser
    {
        Task ParseToFileAsync(IProgress<string> progress, string input, string output);
        Task<(List<Requirement> reqs, DateTime exportDate)> Parse(IProgress<string> progress, string input);
        Task<(List<Requirement> reqs, DateTime exportDate)> GetReqsFromCachedFile(string filename);
    }
}
