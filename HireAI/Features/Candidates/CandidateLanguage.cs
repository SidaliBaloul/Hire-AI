namespace HireAI.Features.Candidates;

public class CandidateLanguage
{
    public Guid CandidateId { get; set; }
    public Candidate Candidate { get; set; } = null!;
    public string Language { get; set; } = string.Empty;
    public string? Level { get; set; }
}
