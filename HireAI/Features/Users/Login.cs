using System.Windows.Input;
using FluentValidation;
using HireAI.Authentication;
using HireAI.Common;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;
using HireAI.Common.Messaging;
using HireAI.Database;
using Microsoft.EntityFrameworkCore;


namespace HireAI.Features.Users;

public static class Login
{
    public sealed record Command(string email, string password) : ICommand<AccessTokenResponse>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.email).NotEmpty().EmailAddress();
            RuleFor(x => x.password).NotEmpty();
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IPasswordHasher passwordHasher, ITokenProvider tokenProvider) 
        : ICommandHandler<Command, AccessTokenResponse>
    {
        public async Task<Result<AccessTokenResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            UserItem? user = await applicationDbContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Email == command.email, cancellationToken);

            if(user is null)
            {
                return Result.Failure<AccessTokenResponse>(UserErrors.EmailNotFound);
            }

            bool verified = passwordHasher.Verify(command.password, user.PasswordHash);

            if(!verified)
            {
                return Result.Failure<AccessTokenResponse>(UserErrors.EmailNotFound);
            }

            string AccessToken = tokenProvider.Create(user);
            string RefreshToken = tokenProvider.GenerateRefreshToken();

            var refresh = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = RefreshToken,
                UserId = user.Id,
                ExpiresOnUtc = DateTime.UtcNow.AddDays(RefreshTokenExpirationInDays)
            };

            applicationDbContext.RefreshTokens.Add(refresh);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return new AccessTokenResponse(AccessToken , RefreshToken);
        }

        private const int RefreshTokenExpirationInDays = 7;
    }

    public sealed class Endpoint : IEndpoint
    {
        public async void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/login", async (string email, string password, ICommandHandler<Command, AccessTokenResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new Command
                (
                    email: email,
                    password: password
                );

                Result<AccessTokenResponse> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomeResults.Problem);

            }).WithTags(Tags.Users);
        }
    }
}
