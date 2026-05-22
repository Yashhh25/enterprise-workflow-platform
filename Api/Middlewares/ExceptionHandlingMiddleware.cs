using System.Text.Json;

namespace Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = MapException(exception);

        _logger.LogError(
            exception,
            "Unhandled exception occurred. StatusCode: {StatusCode}, Message: {Message}",
            (int)statusCode,
            exception.Message);

        if (context.Response.HasStarted)
        {
            throw exception;
        }

        context.Response.Clear();
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse(
            Success: false,
            Message: message,
            StatusCode: (int)statusCode);

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, JsonSerializerOptions));
    }

    private (System.Net.HttpStatusCode StatusCode, string Message) MapException(Exception exception) =>
        exception switch
        {
            UnauthorizedAccessException unauthorized => (
                System.Net.HttpStatusCode.Unauthorized,
                string.IsNullOrWhiteSpace(unauthorized.Message)
                    ? "Unauthorized access."
                    : unauthorized.Message),
            KeyNotFoundException notFound => (
                System.Net.HttpStatusCode.NotFound,
                string.IsNullOrWhiteSpace(notFound.Message)
                    ? "The requested resource was not found."
                    : notFound.Message),
            InvalidOperationException invalid => (
                System.Net.HttpStatusCode.BadRequest,
                string.IsNullOrWhiteSpace(invalid.Message)
                    ? "The request could not be processed."
                    : invalid.Message),
            _ => (
                System.Net.HttpStatusCode.InternalServerError,
                _environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred.")
        };

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private sealed record ApiErrorResponse(bool Success, string Message, int StatusCode);
}
