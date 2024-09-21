using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerticalSliceAPI.Entities;
using VerticalSliceAPI.Model;

namespace VerticalSliceAPI.Features.Users;

public static class GetAllUsers
{
    public class Request : IRequest<IEnumerable<User>> { }

    internal sealed class Handler(AppDbContext context)
        : IRequestHandler<Request, IEnumerable<User>>
    {
        public async Task<IEnumerable<User>> Handle(
            Request request,
            CancellationToken cancellationToken
        )
        {
            return await context.Users.ToListAsync(cancellationToken: cancellationToken);
        }
    }
}

public class GetAllUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "users",
                async (ISender sender) =>
                {
                    var users = await sender.Send(new GetAllUsers.Request());
                    return Results.Ok(users);
                }
            )
            .WithTags(UserShared.Tag)
            .CacheOutput(builder => builder.Expire(TimeSpan.FromMinutes(10)).Tag(UserShared.Tag));
    }
}
