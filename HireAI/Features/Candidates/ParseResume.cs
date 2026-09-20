using FluentValidation;
using HireAI.Authentication;
using HireAI.Common;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;
using HireAI.Common.Messaging;
using HireAI.Database;
using Microsoft.AspNetCore.Http;

namespace HireAI.Features.Candidates.ParseResume;

public static class ParseResume
{
    public sealed record Command(IFormFile Resume) : ICommand<Guid>;

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
        }
    }

    public sealed class Handler(IResumeParser resumeParser, IPdfTextExtractor pdfTextExtractor, IUserContext userContext, ApplicationDbContext applicationDbContext)
        : ICommandHandler<Command, Guid>
    {
        public async Task<Result<Guid>> Handle(Command command, CancellationToken cancellationToken)
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
                userId = userContext.UserId(),
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

            applicationDbContext.Candidates.Add(candidate);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return candidate.Id;
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("candidates/parse-resume", async (IFormFile Resume, ICommandHandler<Command, Guid> handler, CancellationToken cancellationToken) =>
            {
                var command = new Command
                (
                    Resume
                );

                Result<Guid> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomeResults.Problem);

            }).DisableAntiforgery().WithTags(Tags.Candidates).RequireAuthorization();
        }
    }
}
