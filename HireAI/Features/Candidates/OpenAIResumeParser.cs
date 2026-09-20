
using OpenAI.Chat;
using System.Text.Json;

namespace HireAI.Features.Candidates;


internal sealed class OpenAIResumeParser(
    IConfiguration configuration) : IResumeParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static JsonSerializerOptions JsonOptions1 => JsonOptions;

    public async Task<ResumeData> ParseAsync(
     string Resume,
     CancellationToken cancellationToken)
    {
        string apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI API key is missing.");

        ChatClient client = new(
            model: "gpt-5.6-luna",
            apiKey: apiKey);

        string prompt = $"""
        Extract the candidate information from the following resume.

        Extract:
        - Full name
        - Email address
        - Phone number
        - Skills
        - Work experiences, including:
          - Company
          - Job title
          - Description
          - Years of experience

        If information is missing, return null for nullable fields
        and an empty array for lists.

        Resume:
        {Resume}
        """;

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        jsonSchemaFormatName: "resume_data",
        jsonSchema: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {
                    "fullName": {
                        "type": "string"
                    },
                    "email": {
                        "type": ["string", "null"]
                    },
                    "phone": {
                        "type": ["string", "null"]
                    },
                    "skills": {
                        "type": "array",
                        "items": {
                            "type": "string"
                        }
                    },
                    "experiences": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "company": {
                                    "type": "string"
                                },
                                "jobTitle": {
                                    "type": "string"
                                },
                                "description": {
                                    "type": ["string", "null"]
                                },
                                "yearsOfExperience": {
                                    "type": ["integer", "null"]
                                }
                            },
                            "required": [
                                "company",
                                "jobTitle",
                                "description",
                                "yearsOfExperience"
                            ],
                            "additionalProperties": false
                        }
                    }
                },
                "required": [
                    "fullName",
                    "email",
                    "phone",
                    "skills",
                    "experiences"
                ],
                "additionalProperties": false
            }
            """u8.ToArray()),
        jsonSchemaIsStrict: true)
        };

        ChatCompletion completion = await client.CompleteChatAsync(
    [new UserChatMessage(prompt)],
    options,
    cancellationToken: cancellationToken);

        
        string response = completion.Content[0].Text;

        ResumeData? result = JsonSerializer.Deserialize<ResumeData>(response, JsonOptions) ?? throw new InvalidOperationException("OpenAI returned an invalid resume response.");

        return result;
    }

}
