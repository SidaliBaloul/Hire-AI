namespace HireAI.Features.Candidates;

public sealed record MatchResult(
    int Score,
    List<string> MatchingSkills,
    List<string> MissingSkills,
    List<string> MatchingExperience,
    List<string> MissingExperience,
    List<string> MatchingLanguages,
    List<string> MissingLanguages,
    string Explanation);
