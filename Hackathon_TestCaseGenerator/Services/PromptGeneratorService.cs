namespace Hackathon_TestCaseGenerator.Services
{
    public static class PromptGeneratorService
    {
        public static string BuildPrompt(
            string summary,
            string description,
            string steps,
            string acceptanceCriteria,
            string testingRecommendations,
            string documentationContent)
        {
            var prompt = $@"You are an expert QA engineer writing test cases in pure Gherkin format for Hyland Software's OnBase Workview platform.

## Objective:
Generate high-quality test cases using the Given-When-Then structure based on the JIRA details provided and the Reference Documentation.
Do not include markdown symbols like *, #, or bullets.

## Requirements:
- Cover positive, negative, and edge cases.
- Include functional and non-functional aspects where applicable.
- Use clear, concise language.
- Each test case must begin with:
  Scenario: [Title]
  Given ...
  When ...
  Then ...

## JIRA Summary:
{summary}

## JIRA Description:
{description}

## JIRA Steps to Recreate:
{steps}

## JIRA Acceptance Criteria:
{acceptanceCriteria}

## JIRA Testing Recommendations:
{testingRecommendations}

## Reference Documentation:
{documentationContent}

## Example TestCase: Verify deleting an object from the object viewer, filter bar item, or embedded filter displays an appropriate prompt if there are dependent objects
## Output:
Only output the test cases in Gherkin format.";

            return prompt.Trim();
        }


        public static string BuildImprovedPrompt(
            string title,
            string originalTestCase,
            string feedback,
            string summary,
            string description,
            string steps,
            string acceptanceCriteria,
            string testingRecommendations,
            string documentationContent)
        {
            var prompt = $@"
You are an expert QA engineer writing test cases in pure Gherkin format for Hyland Software's OnBase Workview platform.

## Objective:
Regenerate the following test case using the Given-When-Then structure based on user feedback and the original JIRA context and the reference documentation.

## Original Test Case:
Scenario: {title}
{originalTestCase}

## User Feedback:
{feedback}

## JIRA Context:
- Summary: {summary}
- Description: {description}
- Steps to Recreate: {steps}
- Acceptance Criteria: {acceptanceCriteria}
- Testing Recommendations: {testingRecommendations}

## Reference Documentation:
{documentationContent}

## Instructions:
- Use exactly one Given, one When, and one Then per test case.
- Avoid using And clauses unless absolutely necessary.
- Do not use markdown symbols like *, #, or bullets.
- Ensure clarity, relevance to Hyland Workview, and coverage of class/object/attribute validations, filter visibility, expected results, and edge cases.
- Incorporate feedback to improve accuracy, completeness, and usability.
- Output only the improved test case in Gherkin format:
Scenario: [Title]
Given ...
When ...
Then ...
";

            return prompt.Trim();
        }

    }
}
