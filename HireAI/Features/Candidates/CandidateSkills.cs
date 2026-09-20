namespace HireAI.Features.Candidates;


public sealed class CandidateSkill
{
    public Guid CandidateId { get; set; }

    public Candidate Candidate { get; set; } = null!;

    public string Skill { get; set; } = string.Empty;
}
