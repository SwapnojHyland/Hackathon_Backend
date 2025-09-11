using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Hackathon_TestCaseGenerator.Models;

namespace Hackathon_TestCaseGenerator.Services
{
    public class JiraService
    {
        private readonly HttpClient _httpClient;

        public JiraService(JiraConfig config)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(config.BaseUrl) };
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{config.Email}:{config.ApiToken}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        }

        public async Task<JiraIssue> GetIssueAsync(string issueKey)
        {
            var response = await _httpClient.GetAsync($"/rest/api/2/issue/{issueKey}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JiraIssue>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<JiraIssue>> GetPendingQAIssuesAsync(string email)
        {
            var jql = $"assignee = \"{email}\" AND status = \"Pending QA\"";
            var response = await _httpClient.GetAsync($"/rest/api/2/search?jql={Uri.EscapeDataString(jql)}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JiraSearchResult>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result.Issues;
        }

        public class JiraSearchResult
        {
            public List<JiraIssue> Issues { get; set; }
        }

    }

    public class JiraIssue
    {
        public string Key { get; set; }
        public Fields Fields { get; set; }
    }

    public class Fields
    {
        public string Summary { get; set; }
        public string Description { get; set; }
        public string customfield_10830 { get; set; } //steps to recreate
        public string customfield_11535 { get; set; } //design notes
        public string customfield_10402 { get; set; } //acceptance criteria
        public string customfield_11626 { get; set; } //testing recommendations
    }

}
