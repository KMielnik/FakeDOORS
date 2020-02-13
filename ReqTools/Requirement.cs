using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ReqTools
{
    public class Requirement
    {
        public string ID { get; }
        [JsonIgnore]
        public int IDValue { get; }
        public string Text { get; }
        [JsonIgnore]
        public string TextIntended { get; }
        public Types Type { get; }
        public string FVariants { get; }
        public string HVariants { get; }
        public string Comment { get; }

        public int Level { get; }

        public List<TestCase> TCs { get; }
        [JsonIgnore]
        public HashSet<int> TCIDsValue { get; }

        public Statuses Status { get; }
        public string ValidFrom { get; }
        public string ValidTo { get; }
        [JsonIgnore]
        public string ValidFromTo { get; }

        [JsonIgnore]
        public bool IsVisible { get; set; }

        public enum Types
        {
            Head,
            Req,
            Info
        }

        public enum Statuses
        {
            Approved,
            Draft,
            Dropped,
            NA,
            Stable
        }

        public Requirement(string iD, string text, int level, List<TestCase> TCs, string fVariants, string hVariants, Types type, Statuses status, string ValidFrom, string ValidTo, string comment)
        {
            ID = iD;
            IDValue = int.Parse(ID.Replace("PR_PH_", ""));

            Level = level;
            Text = text;
            TextIntended = new string(' ', Level * 3) + Text;
            FVariants = fVariants;
            HVariants = hVariants;
            Comment = comment;
            Type = type;

            Status = status;
            this.ValidFrom = ValidFrom;
            this.ValidTo = ValidTo;
            this.ValidFromTo = $"[{ValidFrom}/{ValidTo}]";

            this.TCs = new List<TestCase>();

            if(TCs!=null)
                this.TCs.AddRange(TCs);
            TCIDsValue = TCs?.Select(x => x.IDValue).ToHashSet() ?? new HashSet<int>();

            IsVisible = true;
        }

        public bool IsValidInSpecifiedVersion(string version)
        => ValidInChecker.IsValidIn(version, ValidFrom, ValidTo);
    }
}
