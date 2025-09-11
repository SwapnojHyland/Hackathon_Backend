namespace Hackathon_TestCaseGenerator.Models
{
    public class TestCaseUploadResult
    {
        public string Title { get; set; }
        public bool UploadSuccess { get; set; }
        public string JiraKey { get; set; }
        public bool LinkSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }

}
