namespace HttpStressTest
{
    public class TestCaseStepDefinition
    {
        public string Id { get; set; }="";
        public string? Get { get; set; }
        public string? Post { get; set; }
        public string[] Headers { get; set; }= Array.Empty<string>();
        public string? Body { get; set; }
        public string? ResponseRegex {get;set;}
        public int[] AllowedStatuses {get;set;}= Array.Empty<int>();
    }
}