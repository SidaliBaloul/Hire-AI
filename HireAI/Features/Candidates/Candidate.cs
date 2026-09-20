namespace HireAI.Features.Candidates;


public sealed class Candidate
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public List<CandidateSkill> Skills { get; set; } = [];

    public List<CandidateExperience> Experiences { get; set; } = [];
    public List<CandidateLanguage> Languages { get; set; } = [];
}


