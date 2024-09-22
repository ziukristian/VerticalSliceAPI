using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerticalSliceAPI.Entities;
using VerticalSliceAPI.Model;

namespace VerticalSliceAPI.Features.Users;

public static class LoginUser
{
    public class Request : IRequest<bool>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Request, bool>
    {
        public async Task<bool> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await context
                .Users.Where(u => u.Email.Equals(request.Email))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user == null)
            {
                return false;
            }

            if (BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return true;
            }

            return false;
        }
    }
}

public class LoginUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "users/login",
                async (ISender sender, LoginUser.Request request) =>
                {
                    var isAuthenticated = await sender.Send(request);

                    return isAuthenticated ? Results.Ok() : Results.Unauthorized();
                }
            )
            .WithTags(UserShared.Tag);
    }
}
