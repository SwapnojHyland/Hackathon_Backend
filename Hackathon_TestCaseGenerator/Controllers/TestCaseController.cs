using Hackathon_TestCaseGenerator.Models;
using Hackathon_TestCaseGenerator.Models.DTOs;
using Hackathon_TestCaseGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon_TestCaseGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestCaseController : ControllerBase
    {
        private readonly JiraService _jiraService;
        private readonly OpenAIService _openAIService;
        private readonly XrayUploadService _xrayService;
        private readonly DocumentationService _docService;

        public TestCaseController(
            JiraService jiraService,
            OpenAIService openAIService,
            XrayUploadService xrayService,
            DocumentationService docService)
        {
            _jiraService = jiraService;
            _openAIService = openAIService;
            _xrayService = xrayService;
            _docService = docService;
        }

        [HttpGet("jira/{jiraId}")]
        public async Task<IActionResult> GetJiraDetails(string jiraId)
        {
            try
            {
                var issue = await _jiraService.GetIssueAsync(jiraId);
                return Ok(issue);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet("pendingqa/{email}")]
        public async Task<IActionResult> GetPendingQAIssues(string email)
        {
            var issues = await _jiraService.GetPendingQAIssuesAsync(email);
            return Ok(issues);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateTestCases([FromBody] GenerateRequest request)
        {
            try
            {
                var issue = await _jiraService.GetIssueAsync(request.JiraId);
                var documentationContent = _docService.GetDocumentationContent();

                var prompt = PromptGeneratorService.BuildPrompt(
                    issue.Fields.Summary,
                    issue.Fields.Description,
                    issue.Fields.customfield_10830,
                    issue.Fields.customfield_10402,
                    issue.Fields.customfield_11626,
                    documentationContent
                );

                var testCases = await _openAIService.GenerateTestCasesAsync(prompt);
                return Ok(testCases);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("review")]
        public async Task<IActionResult> ReviewTestCases([FromBody] FeedbackRequest request)
        {
            var updatedTestCases = new List<TestCase>();
            var documentationContent = _docService.GetDocumentationContent();

            foreach (var feedback in request.Feedbacks)
            {
                var improvedPrompt = PromptGeneratorService.BuildImprovedPrompt(
                    feedback.OriginalTitle,
                    $"{feedback.Given}\n{feedback.When}\n{feedback.Then}",
                    feedback.FeedbackText,
                    feedback.Summary,
                    feedback.Description,
                    feedback.StepsToRecreate,
                    feedback.AcceptanceCriteria,
                    feedback.TestingRecommendations,
                    documentationContent
                );

                var improvedList = await _openAIService.GenerateTestCasesAsync(improvedPrompt);
                updatedTestCases.Add(improvedList.FirstOrDefault() ?? new TestCase
                {
                    Title = feedback.OriginalTitle,
                    Given = feedback.Given,
                    When = feedback.When,
                    Then = feedback.Then
                });
            }

            return Ok(updatedTestCases);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadTestCases([FromBody] UploadRequest request)
        {
            try
            {
                var results = await _xrayService.PushTestCasesAsync(request.JiraId, request.TestCases);
                return Ok(new { results });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
