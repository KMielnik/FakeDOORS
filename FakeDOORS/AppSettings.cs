using System;
using System.Collections.Generic;
using System.Text;

namespace FakeDOORS
{
    public class AppSettings
    {
        public string ServerPath { get; set; } = @"\\10.128.3.1\DFS_Data_KBN_RnD_FS_Programs\Support_Tools\FakeDOORS\";
        public int VersionMajor { get; set; } = 2;
        public int VersionMinor { get; set; } = 3;
    }
}
