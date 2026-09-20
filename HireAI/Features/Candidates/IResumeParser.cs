namespace HireAI.Features.Candidates;

public interface IResumeParser
{
    Task<ResumeData> ParseAsync(string Resume, CancellationToken cancellationToken);
}
