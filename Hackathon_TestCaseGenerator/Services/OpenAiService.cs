using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackathon_TestCaseGenerator.Models;
using OpenAI.Chat;

namespace Hackathon_TestCaseGenerator.Services
{
    public class OpenAIService
    {
        private readonly ChatClient _client;

        public OpenAIService(string apiKey)
        {
            _client = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);
        }

        public async Task<List<TestCase>> GenerateTestCasesAsync(string prompt)
        {
            ChatCompletion completion = await _client.CompleteChatAsync(prompt);
            string rawText = completion.Content[0].Text.Trim();

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
