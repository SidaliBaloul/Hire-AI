using FluentValidation;
using HireAI.Common;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;
using HireAI.Common.Messaging;
using HireAI.Database;

namespace HireAI.Features.Candidates;

public sealed class FastScan
{
    public sealed record Command(IFormFile Resume, string JobDescription) : ICommand<MatchResult>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Resume)
                .NotNull()
                .Must(file => file.Length > 0);

            RuleFor(x => x.Resume)
                .Must(file => file.ContentType == "application/pdf");

            RuleFor(x => x.Resume)
                .Must(file => file.Length <= 5 * 1024 * 1024);

            RuleFor(x => x.JobDescription)
                .NotEmpty()
                .WithMessage("Job description is required.")
                .MaximumLength(10_000)
                .WithMessage("Job description cannot exceed 10,000 characters.");
        }
    }

    public sealed class Handler(IResumeParser resumeParser, IPdfTextExtractor pdfTextExtractor, ICandidateMatcher candidateMatcher )
        : ICommandHandler<Command, MatchResult>
    {
        public async Task<Result<MatchResult>> Handle(Command command, CancellationToken cancellationToken)
        {
            await using Stream stream = command.Resume.OpenReadStream();

            string resumeText = await pdfTextExtractor.ExtractAsync(
                stream,
                cancellationToken);


            ResumeData result = await resumeParser.ParseAsync(resumeText, cancellationToken);

            var candidateId = Guid.NewGuid();

            Candidate candidate = new()
            {
                Id = candidateId,
                FullName = result.FullName,
                Email = result.Email,
                Phone = result.Phone,

                Skills = result.Skills
        .Select(skill => new CandidateSkill
        {
            CandidateId = candidateId,
            Skill = skill
        })
        .ToList(),

                Experiences = result.Experiences
        .Select(experience => new CandidateExperience
        {
            Id = Guid.NewGuid(),
            CandidateId = candidateId,
            Company = experience.Company,
            JobTitle = experience.JobTitle,
            Description = experience.Description,
            YearsOfExperience = experience.YearsOfExperience
        })
        .ToList()
            };

            MatchResult matchResult = await candidateMatcher.MatchAsync(candidate, command.JobDescription, cancellationToken);

            return matchResult;
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("candidates/fast-scan", async (IFormFile Resume, string JobDescription, ICommandHandler<Command, MatchResult> handler, CancellationToken cancellationToken) =>
            {
                var command = new Command
                (
                    Resume,
                    JobDescription
                );

                Result<MatchResult> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomeResults.Problem);
            }).DisableAntiforgery();
        }
    }
}
