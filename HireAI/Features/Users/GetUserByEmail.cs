using HireAI.Authentication;
using HireAI.Common;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;
using HireAI.Common.Messaging;
using HireAI.Database;
using Microsoft.EntityFrameworkCore;
using HireAI.Authorization;

namespace HireAI.Features.Users;

public sealed class GetUserByEmail
{
    public sealed record Query(string email) : IQuery<Response>;


    public sealed record Response
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public sealed class Handler(ApplicationDbContext applicationDbContext, IUserContext userContext) : IQueryHandler<Query, Response>
    {
        public async Task<Result<Response>> Handle(Query query, CancellationToken cancellationToken)
        {
            Response? response = await applicationDbContext.Users
                .Select(x => new Response { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, Email = x.Email })
                .SingleOrDefaultAsync(x => x.Email == query.email.Trim(), cancellationToken);

            if(response is null)
            {
                return Result.Failure<Response>(UserErrors.EmailNotFound);
            }

            if (response.Id != userContext.UserId())
            {
                return Result.Failure<Response>(UserErrors.Unauthorized());
            }

            return response;
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("users/{Email}", async (string Email, IQueryHandler<Query, Response> handler, CancellationToken cancellationToken) =>
            {
                var query = new Query
                (
                    email: Email
                );

                Result<Response> response = await handler.Handle(query, cancellationToken);

                return response.Match(Results.Ok, CustomeResults.Problem);

            }).WithTags(Tags.Users)
                .RequireAuthorization();
                
        }
    }
}
