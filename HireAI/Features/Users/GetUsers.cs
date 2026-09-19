using System.Linq.Expressions;
using HireAI.Common;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;
using HireAI.Common.Messaging;
using HireAI.Database;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


using FluentValidation;


namespace HireAI.Features.Users;

public sealed class GetUsers
{
    public sealed record Query(string? search, string? sortColumn, string? sortOrder, int page, int pageSize)
        : IQuery<PagedList<Response>>;

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

    }

    public sealed class Handler(ApplicationDbContext applicationDbContext) : IQueryHandler<Query, PagedList<Response>>
    {
        public async Task<Result<PagedList<Response>>> Handle(Query query, CancellationToken cancellationToken)
        {
            IQueryable<UserItem> usersQuery = applicationDbContext.Users;

            if(!string.IsNullOrEmpty(query.search))
            {
                string search = query.search;
                usersQuery = usersQuery.Where(f => (f.FirstName + " " + f.LastName).Contains(search));
            }

            usersQuery = string.Equals(query.sortOrder, "desc", StringComparison.OrdinalIgnoreCase)
               ? usersQuery.OrderByDescending(GetSortProperty(query.sortColumn))
               : usersQuery.OrderBy(GetSortProperty(query.sortColumn));

            IQueryable<Response> responseQuery = usersQuery.Select(i => new Response { Id = i.Id, FirstName = i.FirstName, LastName = i.LastName, Email = i.Email });


            PagedList<Response> pagedList = await PagedList<Response>.CreateAsync(responseQuery, query.page, query.pageSize, cancellationToken);


            return pagedList;

        }

        private static Expression<Func<UserItem, object>> GetSortProperty(string? sortColumn) =>
          sortColumn?.ToUpperInvariant() switch
          {
              "FIRSTNAME" => todoItem => todoItem.FirstName!,
              "LASTNAME" => todoItem => todoItem.LastName!,
              _ => todoItem => todoItem.Email
          };


        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("users", async (string ? search, string ? sortColumn, string ? sortOrder, int? page, int? pageSize,
                    IQueryHandler<Query, PagedList<Response>> handler, CancellationToken cancellationToken) =>
                 {
                     var query = new Query
                     (
                         search,
                         sortColumn,
                         sortOrder,
                         page ?? 1,
                         pageSize ?? 10
                     );


                    Result<PagedList<Response>> result = await handler.Handle(query, cancellationToken);

                    return result.Match(Results.Ok, CustomeResults.Problem);
                })
            .WithTags(Tags.Users);
            }
        }
    }
}
