using System;
using System.Collections.Generic;
using System.Text;

namespace ReqTools
{
    public static class ValidInChecker
    {
        public static bool IsValidIn(string version, string ValidFrom, string ValidTo)
        {
            if (version == "-" || version is null)
                return true;
            if (version == "Not Closed")
                return ValidTo == "-";

            var selectedVersion = int.Parse(version.Replace(".", ""));

            bool isValidBefore;
            if (ValidFrom == "-")
                isValidBefore = true;
            else
            {
                var validBefore = int.Parse(ValidFrom.Replace(".", ""));
                isValidBefore = validBefore <= selectedVersion;
            }

            bool isValidAfter;
            if (ValidTo == "-")
                isValidAfter = true;
            else
            {
                var validAfter = int.Parse(ValidTo.Replace(".", ""));
                isValidAfter = validAfter >= selectedVersion;
            }
            return isValidBefore && isValidAfter;
        }
    }
}
