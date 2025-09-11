using System.Text;
using Hackathon_TestCaseGenerator.Models;
using OllamaSharp;

namespace Hackathon_TestCaseGenerator.Services
{
    public class OllamaService
    {
        private readonly OllamaApiClient _client;

        public OllamaService(string model)
        {
            var endpoint = new Uri("http://localhost:11434/");
            _client = new OllamaApiClient(endpoint, model);
        }

        public async Task<List<TestCase>> GenerateTestCasesAsync(string prompt)
        {
            var fullResponse = new StringBuilder();

            await foreach (var response in _client.GenerateAsync(prompt))
            {
                fullResponse.Append(response?.Response);
            }

            var rawText = fullResponse.ToString().Trim();
            var testCases = new List<TestCase>();
            var lines = rawText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            TestCase current = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("Scenario:"))
                {
                    if (current != null)
                        testCases.Add(current);

                    current = new TestCase
                    {
                        Title = line.Replace("Scenario:", "").Trim()
                    };
                }
                else if (line.StartsWith("Given"))
                {
                    current.Given = line.Replace("Given", "").Trim();
                }
                else if (line.StartsWith("When"))
                {
                    current.When = line.Replace("When", "").Trim();
                }
                else if (line.StartsWith("Then"))
                {
                    current.Then = line.Replace("Then", "").Trim();
                }
            }

            if (current != null)
                testCases.Add(current);

            return testCases;
        }


    }
}
