using FilmForumWebAPI.Models.Errors;
using System.Net;

namespace FilmForumWebAPI.Middlewares;

public class RequestExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestExceptionMiddleware> _logger;

    public RequestExceptionMiddleware(RequestDelegate next, ILogger<RequestExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
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