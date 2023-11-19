using FilmForumModels.Dtos.RequestDtos;
using FilmForumModels.Models.Errors;
using FilmForumWebAPI.Services.Interfaces;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;

namespace FilmForumWebAPI.Middlewares;

public class RequestMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestMiddleware> _logger;

    public RequestMiddleware(RequestDelegate next,
                             ILogger<RequestMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext, IRequestLogService _requestLogService)
    {
        try
        {
            await _next(httpContext);

            string? userId = httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is not null)
            {
                CreateRequestLogDto createRequestLogDto = new(int.Parse(userId),
                                                              httpContext.Request.Path,
                                                              httpContext.Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                                                              httpContext.Request.Method,
                                                              httpContext.Response.StatusCode);

                await _requestLogService.CreateAsync(createRequestLogDto);
            }
        }
        catch (Exception exception)
        {
            await WriteExceptionToResponseAsync(httpContext, exception);
            _logger.LogError(exception, "Exception caught by exception middleware");
        }
    }

    private static async Task WriteExceptionToResponseAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(new RequestError(context.Response.StatusCode,
                                                           exception.Message).ToString());
    }
}