namespace HireAI.Features.Candidates;

public interface IPdfTextExtractor
{
    Task<string> ExtractAsync(
        Stream pdfStream,
        CancellationToken cancellationToken);
}
