using FluentValidation;
using HireAI.Common;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;
using HireAI.Common.Messaging;
using HireAI.Database;
using Microsoft.EntityFrameworkCore;

namespace HireAI.Features.Candidates;

public static class MatchCandidate
{
    public sealed record Command(Guid CandidateId, string JobDescription) : ICommand<MatchResult>;

    public sealed class Validator : AbstractValidator<MatchCandidate.Command>
    {
        public Validator()
        {
            RuleFor(x => x.CandidateId)
                .NotEmpty()
                .WithMessage("Candidate ID is required.");

            RuleFor(x => x.JobDescription)
                .NotEmpty()
                .WithMessage("Job description is required.")
                .MaximumLength(10_000)
                .WithMessage("Job description cannot exceed 10,000 characters.");
        }
    }


    public sealed class Handler(ApplicationDbContext applicationDbContext, ICandidateMatcher candidateMatcher)
        : ICommandHandler<Command, MatchResult>
    {
        public async Task<Result<MatchResult>> Handle(Command command, CancellationToken cancellationToken)
        {
            Candidate? candidate = await applicationDbContext.Candidates
                .Include(x => x.Skills)
                .Include(x => x.Experiences)
                .SingleOrDefaultAsync( x => x.Id == command.CandidateId, cancellationToken);

            if (candidate is null)
            {
                return Result.Failure<MatchResult>(CandidateErrors.CondidateNotFound);
            }

            MatchResult result = await candidateMatcher.MatchAsync(candidate, command.JobDescription, cancellationToken);

            return result;
        }

    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("candidates/match", async (Guid CandidateId, string JobDescription, ICommandHandler<Command, MatchResult> handler, CancellationToken cancellationToken) =>
            {
                var command = new Command
                (
                    CandidateId,
                    JobDescription
                );

                Result<MatchResult> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomeResults.Problem);
            }).WithTags(Tags.Candidates).RequireAuthorization();
        }
    }

}
