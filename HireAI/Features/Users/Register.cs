using HireAI.Authentication;
using FluentValidation;
using HireAI.Common.Messaging;
using HireAI.Common;
using HireAI.Database;
using Microsoft.EntityFrameworkCore;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;


namespace HireAI.Features.Users;


public static class Register
{
    public sealed record Command(string FirstName, string LastName, string Email, string Password) : ICommand<Guid>;
    public sealed record Mommand(string FirstName, string LastName, string Email, string Password) : ICommand;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IPasswordHasher passwordHasher) : ICommandHandler<Command,Guid>
    {
        public async Task<Result<Guid>> Handle(Command command, CancellationToken cancellationToken)
        {
            if(await applicationDbContext.Users.AnyAsync(x => x.Email == command.Email, cancellationToken))
            {
                return Result.Failure<Guid>(UserErrors.EmailNotUnique);
            }

            var user = new UserItem
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PasswordHash = passwordHasher.Hash(command.Password),
                Id = Guid.NewGuid()
            };

            applicationDbContext.Users.Add(user);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }

    public sealed class Mandler(ApplicationDbContext applicationDbContext, IPasswordHasher passwordHasher) : ICommandHandler<Mommand>
    {
        public async Task<Result> Handle(Mommand command, CancellationToken cancellationToken)
        {
            if (await applicationDbContext.Users.AnyAsync(x => x.Email == command.Email, cancellationToken))
            {
                return Result.Failure<Guid>(UserErrors.EmailNotUnique);
            }

            var user = new UserItem
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PasswordHash = passwordHasher.Hash(command.Password),
                Id = Guid.NewGuid()
            };

            applicationDbContext.Users.Add(user);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public sealed record Request(string Firstname, string Lastname, string email, string password);
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/register", async (Request request, ICommandHandler<Command, Guid> handler, CancellationToken CancellationToken) =>
            {
                var command = new Command
                (
                    FirstName: request.Firstname,
                    LastName: request.Lastname,
                    Email: request.email,
                    Password: request.password
                );

                Result<Guid> result = await handler.Handle(command, CancellationToken);

                return result.Match(Results.Ok, CustomeResults.Problem);
            })
              .WithTags(Tags.Users);
        }
    }
}
