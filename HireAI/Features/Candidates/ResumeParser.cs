namespace HireAI.Features.Candidates;


internal sealed class ResumeParser : IResumeParser
{
    public Task<ResumeData> ParseAsync(string Resume, CancellationToken cancellationToken)
    {
        ResumeData result = new(
            "John Smith",
            "john@example.com",
            null,
            ["C#", "ASP.NET Core", "SQL Server"],
            []);

        return Task.FromResult(result);
    }
}
