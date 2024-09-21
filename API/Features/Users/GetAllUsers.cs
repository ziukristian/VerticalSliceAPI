using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerticalSliceAPI.Entities;
using VerticalSliceAPI.Model;

namespace VerticalSliceAPI.Features.Users
{
    public class GetAllUsers { }

    public class GetAllUsersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(
                    "users",
                    async (AppDbContext context) =>
                    {
                        return Results.Ok(await context.Users.ToListAsync());
                    }
                )
                .WithTags(UserShared.Tag)
                .CacheOutput(builder =>
                    builder.Expire(TimeSpan.FromMinutes(10)).Tag(UserShared.Tag)
                );
        }
    }
}
