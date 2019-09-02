using System.Text.Json.Serialization;

namespace ReqTools
{
    public class TestCase
    {
        public string ID { get; }
        [JsonIgnore]
        public int IDValue { get; }
        public string Text { get; }
        public string ValidFrom { get; }
        public string ValidTo { get; }

        public TestCase(string ID, string Text, (string From, string To) Valid)
        {
            this.ID = ID;
            IDValue = int.Parse(ID);
            this.Text = Text;
            ValidFrom = Valid.From;
            ValidTo = Valid.To;
        }

        public bool IsValidInSpecifiedVersion(string version)
        => ValidInChecker.IsValidIn(version, ValidFrom, ValidTo);

        public override int GetHashCode()
        => IDValue;

        public override bool Equals(object obj)
        {
            var testCase = obj as TestCase;
            if (testCase is null)
                return false;
            return IDValue == testCase.IDValue;
        }
    }
}