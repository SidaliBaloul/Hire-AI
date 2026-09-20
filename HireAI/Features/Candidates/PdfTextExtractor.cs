using UglyToad.PdfPig;

namespace HireAI.Features.Candidates;

internal sealed class PdfTextExtractor : IPdfTextExtractor
{
    public Task<string> ExtractAsync(
        Stream pdfStream,
        CancellationToken cancellationToken)
    {
        using var document = PdfDocument.Open(pdfStream);

        string text = string.Join(
            Environment.NewLine,
            document.GetPages().Select(page => page.Text));

        return Task.FromResult(text);
    }
}
