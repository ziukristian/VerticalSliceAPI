using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using VerticalSliceAPI.Entities;
using VerticalSliceAPI.Model;

namespace VerticalSliceAPI.Features.Users
{
    public class CreateUser { }

    public class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost(
                "users",
                async (
                    AppDbContext context,
                    ISwaggerProvider provider,
                    IOutputCacheStore cacheStore
                ) =>
                {
                    User user = new() { Email = "ziukristian@gmail.com", Password = "123" };

                    await context.AddAsync(user);

                    await context.SaveChangesAsync();

                    await cacheStore.EvictByTagAsync(UserShared.Tag, default);

                    return Results.Ok(user.Id);
                }
            );
        }
    }
}
