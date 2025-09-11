namespace Hackathon_TestCaseGenerator.Models.DTOs
{

    public class GenerateRequest
    {
        public string JiraId { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string StepsToRecreate { get; set; }
        public string DesignNotes { get; set; }
        public string AcceptanceCriteria { get; set; }
        public string TestingRecommendations { get; set; }
    }

}
