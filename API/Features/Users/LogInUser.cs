using Carter;
using FluentValidation;
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

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
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
                throw new UnauthorizedAccessException("User not found");
            }

            var doPasswordsMatch = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!doPasswordsMatch)
            {
                throw new UnauthorizedAccessException("Wrong password");
            }

            return true;
        }
    }
}

public class LoginUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "users/login",
                async (
                    ISender sender,
                    LoginUser.Request request,
                    IValidator<LoginUser.Request> validator
                ) =>
                {
                    var validationResult = await validator.ValidateAsync(request);

                    if (!validationResult.IsValid)
                    {
                        return Results.ValidationProblem(validationResult.ToDictionary());
                    }

                    var isAuthenticated = await sender.Send(request);

                    return Results.Ok(isAuthenticated);
                }
            )
            .WithTags(UserShared.Tag);
    }
}
