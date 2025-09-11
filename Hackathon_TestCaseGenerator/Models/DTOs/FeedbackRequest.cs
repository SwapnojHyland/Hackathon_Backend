namespace Hackathon_TestCaseGenerator.Models.DTOs
{
    public class FeedbackRequest
    {

        public List<TestCaseFeedback> Feedbacks { get; set; }
    }

    public class TestCaseFeedback
    {
        public string OriginalTitle { get; set; }
        public string Given { get; set; }
        public string When { get; set; }
        public string Then { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; } 
        public string StepsToRecreate { get; set; } 
        public string DesignNotes { get; set; } 
        public string TestingRecommendations { get; set; }
        public string AcceptanceCriteria { get; set; }
        public string FeedbackText { get; set; }

    }
}
