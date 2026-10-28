using Microsoft.AspNetCore.Mvc;
using EventBookingService.Exceptions;

namespace EventBookingService.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
        catch (Exception ex)
        {
            if (httpContext.Response.HasStarted)
            {
                _logger.LogError(ex, "An exception occurred after the response had already started.");
                
                throw;
            }
            LogException(ex);
            await HandleException(httpContext, ex);
        }
    }

    private static async Task HandleException(HttpContext httpContext, Exception ex)
    {
        var statusCode = MapStatusCode(ex);

        var detail = statusCode == StatusCodes.Status500InternalServerError
            ? "Произошла внутренняя ошибка сервера."
            : ex.Message;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }

    private static int MapStatusCode(Exception ex) =>
        ex switch 
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            _=> StatusCodes.Status500InternalServerError,
        };
    private void LogException(Exception ex)
    {
        switch (ex)
        {
            case ValidationException:
                _logger.LogWarning(ex, "Validation error.");
            break;
            case NotFoundException:
            _logger.LogWarning(ex, "Resource not found.");
            break;
            default: _logger.LogError(ex, "An unexpected error occurred.");
            break;
        }
    }   
}