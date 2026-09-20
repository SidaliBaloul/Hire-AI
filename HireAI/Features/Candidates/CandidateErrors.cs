using HireAI.Common;

namespace HireAI.Features.Candidates;

public static class CandidateErrors
{
    public static readonly Error CondidateNotFound = Error.NotFound("Candidate not found", "candidate with enetred Id has not been found");
}
