using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using FluentValidation;
using HireAI.Authentication;
using HireAI.Common;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;
using HireAI.Common.Messaging;
using HireAI.Database;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.EntityFrameworkCore;

namespace HireAI.Features.Users;

public static class Refresh
{
    public sealed record Command(string refreshToken) : ICommand<AccessTokenResponse>;

    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.refreshToken).NotEmpty();
        }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, ITokenProvider tokenProvider) : ICommandHandler<Command, AccessTokenResponse>
    {
        public async Task<Result<AccessTokenResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            RefreshToken? refreshToken = await applicationDbContext.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == command.refreshToken, cancellationToken);

            if (refreshToken is null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            {
                return Result.Failure<AccessTokenResponse>(UserErrors.InvalidRefreshToken);
            }

            string accessToken = tokenProvider.Create(refreshToken.User);
            string newRefreshToken = tokenProvider.GenerateRefreshToken();

            // Rotate the refresh token so a stolen token can only be used once.
            refreshToken.Token = newRefreshToken;
            refreshToken.ExpiresOnUtc = DateTime.UtcNow.AddDays(RefreshTokenExpirationInDays);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return new AccessTokenResponse(accessToken, newRefreshToken);
        }

        private const int RefreshTokenExpirationInDays = 7;
    
    }

    public sealed class Endpoint : IEndpoint
    {
        public sealed record Request(string RefreshToken);

        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("users/refresh-token", async (
                Request request,
                ICommandHandler<Command, AccessTokenResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new Command(request.RefreshToken);

                Result<AccessTokenResponse> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomeResults.Problem);
            })
            .WithTags(Tags.Users)
            .RequireRateLimiting(RateLimitingPolicies.Authentication);
        }
    }
}
