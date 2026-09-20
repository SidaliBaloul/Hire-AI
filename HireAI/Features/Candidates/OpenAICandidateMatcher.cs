using System.Text.Json;
using OpenAI.Chat;

namespace HireAI.Features.Candidates;

internal sealed class OpenAICandidateMatcher(IConfiguration configuration) : ICandidateMatcher
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<MatchResult> MatchAsync(Candidate candidate, string jobDescription, CancellationToken cancellationToken)
    {
        string apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key is missing.");

        ChatClient client = new(
            model: "gpt-5.6-luna",
            apiKey: apiKey);

        string skills = string.Join(", ", candidate.Skills.Select(x => x.Skill));

        string experiences = string.Join(Environment.NewLine, candidate.Experiences.Select(x =>
                $"""
                Company: {x.Company}
                Job Title: {x.JobTitle}
                Description: {x.Description}
                Years: {x.YearsOfExperience}
                """));

        string prompt = $"""
    Evaluate how well this candidate matches the following job description.

    Candidate:
    Name: {candidate.FullName}

    Skills:
    {skills}

    Work Experience:
    {experiences}

    Job Description:
    {jobDescription}

    Analyze the candidate against the job requirements using these categories:

    1. Skills
       - Identify skills the candidate has that match the job requirements.
       - Identify important skills required by the job that the candidate is missing.

    2. Work Experience
       - Identify the candidate's work experience that is relevant to the job.
       - Identify important experience requirements from the job that are missing
         from the candidate's demonstrated work experience.

    3. Human Languages
       - Identify human languages explicitly mentioned in the candidate's information
         that are required or useful for the job, (if there is ).
       - Identify human languages required or preferred by the job that the candidate
         does not demonstrate, , (if there is ).
       - Do not treat programming languages such as C#, Java, Python, or JavaScript
         as human languages.

    4. Overall Match
       - Give a score from 0 to 100 based only on the information provided.
       - Consider skills, work experience, and human language requirements.
       - Do not assume that the candidate has a skill, experience, or language
         that is not explicitly supported by the candidate information.
       - Do not penalize the candidate for requirements that are not mentioned
         in the job description.

    Return a concise explanation of the overall match.

    Candidate information must be evaluated only against the provided job description.
    """;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "candidate_match",
                jsonSchema: BinaryData.FromBytes("""
{
    "type": "object",
    "properties": {
        "score": {
            "type": "integer"
        },
        "matchingSkills": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "missingSkills": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "matchingExperience": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "missingExperience": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "matchingLanguages": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "missingLanguages": {
            "type": "array",
            "items": {
                "type": "string"
            }
        },
        "explanation": {
            "type": "string"
        }
    },
    "required": [
        "score",
        "matchingSkills",
        "missingSkills",
        "matchingExperience",
        "missingExperience",
        "matchingLanguages",
        "missingLanguages",
        "explanation"
    ],
    "additionalProperties": false
}
"""u8.ToArray()),
                jsonSchemaIsStrict: true)
        };

        ChatCompletion completion = await client.CompleteChatAsync(
            [new UserChatMessage(prompt)],
            options,
            cancellationToken);

        string response = completion.Content[0].Text;

        Console.WriteLine(response);

        return JsonSerializer.Deserialize<MatchResult>( response, JsonOptions) ?? throw new InvalidOperationException("OpenAI returned an invalid candidate match response.");
    }
}
