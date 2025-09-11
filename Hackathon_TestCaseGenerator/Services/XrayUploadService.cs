using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hackathon_TestCaseGenerator.Models;

namespace Hackathon_TestCaseGenerator.Services
{

    public class XrayUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly JiraConfig _config;

        public XrayUploadService(JiraConfig config)
        {
            _config = config;
            _httpClient = new HttpClient { BaseAddress = new Uri(_config.BaseUrl) };
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_config.Email}:{_config.ApiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        }
        public async Task<List<TestCaseUploadResult>> PushTestCasesAsync(string issueKey, List<TestCase> testCases)
        {
            var results = new List<TestCaseUploadResult>();

            foreach (var testCase in testCases)
            {
                var result = new TestCaseUploadResult
                {
                    Title = testCase.Title
                };

                try
                {
                    var payload = new
                    {
                        fields = new
                        {
                            project = new { key = _config.ProjectKey },
                            summary = testCase.Title,
                            issuetype = new { name = "Xray Test" },
                            description = $"**Given** {testCase.Given}\n**When** {testCase.When}\n**Then** {testCase.Then}"
                        }
                    };

                    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("/rest/api/2/issue", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        result.UploadSuccess = false;
                        result.ErrorMessage = $"Upload failed: {response.ReasonPhrase}";
                        results.Add(result);
                        continue;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var doc = JsonDocument.Parse(json);
                    result.JiraKey = doc.RootElement.GetProperty("key").GetString();
                    result.UploadSuccess = true;

                    var linkPayload = new
                    {
                        type = new { name = "Test" },
                        inwardIssue = new { key = result.JiraKey },
                        outwardIssue = new { key = issueKey }
                    };

                    var linkContent = new StringContent(JsonSerializer.Serialize(linkPayload), Encoding.UTF8, "application/json");
                    var linkResponse = await _httpClient.PostAsync("/rest/api/2/issueLink", linkContent);

                    result.LinkSuccess = linkResponse.IsSuccessStatusCode;
                    if (!result.LinkSuccess)
                    {
                        result.ErrorMessage = $"Linking failed: {linkResponse.ReasonPhrase}";
                    }
                }
                catch (Exception ex)
                {
                    result.UploadSuccess = false;
                    result.ErrorMessage = $"Exception: {ex.Message}";
                }

                results.Add(result);
            }

            return results;
        }


        public async Task<bool> MoveTestToFolderAsync(string testIssueKey, string folderId)
        {
            var graphqlPayload = new
            {
                query = $@"
            mutation {{
                addTestsToFolder(
                    folderId: ""{folderId}"",
                    testIssueIds: [""{testIssueKey}""]
                ) {{
                    addedTestIssueIds
                }}
            }}"
            };

            var content = new StringContent(JsonSerializer.Serialize(graphqlPayload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/v2/graphql", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Test {testIssueKey} moved to folder {folderId}");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to move test {testIssueKey} to folder {folderId}");
                return false;
            }
        }
    }
    
}
