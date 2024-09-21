using System.Reflection.Metadata;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using VerticalSliceAPI.Entities;
using VerticalSliceAPI.Model;

namespace VerticalSliceAPI.Features.Users;

public static class CreateUser
{
    public class Command : IRequest<Guid>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    internal sealed class Handler(AppDbContext context, IOutputCacheStore cacheStore)
        : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Password = request.Password,
            };

            context.Users.Add(user);

            await context.SaveChangesAsync(cancellationToken);

            await cacheStore.EvictByTagAsync(UserShared.Tag, default);

            return user.Id;
        }
    }
}

public class CreateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "users",
            async (CreateUser.Command command, ISender sender) =>
            {
                var userId = await sender.Send(command);

                return Results.Ok(userId);
            }
        );
    }
}
