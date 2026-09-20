namespace HireAI.Features.Candidates;

public sealed class CandidateExperience
{
    public Guid Id { get; set; }

    public Guid CandidateId { get; set; }

    public string Company { get; set; } = string.Empty;

    public string JobTitle { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? YearsOfExperience { get; set; }

    public Candidate Candidate { get; set; } = null!;
}
