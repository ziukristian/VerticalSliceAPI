using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace VerticalSliceAPI.Middleware
{
    public sealed class ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        RequestDelegate next
    )
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ve)
            {
                logger.LogError(ve, ve.Message);

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                ProblemDetails problem =
                    new()
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Type = "Bad request",
                        Title = "Bad request",
                        Detail = ve.Message,
                    };

                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(
                    problem,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

                await context.Response.WriteAsync(json);
            }
            catch (UnauthorizedAccessException ae)
            {
                logger.LogError(ae, ae.Message);

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                ProblemDetails problem =
                    new()
                    {
                        Status = (int)HttpStatusCode.Unauthorized,
                        Type = "Unauthorized",
                        Title = "Unauthorized",
                        Detail = ae.Message,
                    };

                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(
                    problem,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

                await context.Response.WriteAsync(json);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                ProblemDetails problem =
                    new()
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Type = "Server error",
                        Title = "Server error",
                        Detail = null,
                    };

                context.Response.ContentType = "application/json";

                var json = JsonSerializer.Serialize(
                    problem,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );

                await context.Response.WriteAsync(json);
            }
        }
    }
}
