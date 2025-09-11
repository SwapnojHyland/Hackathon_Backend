namespace Hackathon_TestCaseGenerator.Models.DTOs
{
    public class UploadRequest
    {
        public string JiraId { get; set; }
        public List<TestCase> TestCases { get; set; }
    }
}
