namespace HireAI.Features.Candidates;

public interface ICandidateMatcher
{
    Task<MatchResult> MatchAsync(
        Candidate candidate,
        string jobDescription,
        CancellationToken cancellationToken);
}
