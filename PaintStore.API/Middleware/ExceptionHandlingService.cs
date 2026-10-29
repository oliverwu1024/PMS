using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using PaintStore.API.Dtos;


namespace PaintStore.API.Middleware;

public class ExceptionHandlingService
{
    private readonly ILogger<ExceptionHandlingService> _logger;

    public ExceptionHandlingService(ILogger<ExceptionHandlingService> logger)
    {
        _logger = logger;
    }
    public async Task HandleExceptionAsync(HttpContext context)
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature is null) return;

        var exception = exceptionHandlerFeature.Error;
        _logger.LogError(exception, "Unhandled exception occurred.");
        var statusCode = GetStatusCode(exception);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode; //GetStatusCode(exception);

        var errorResponse = new ErrorResponse(
            //Message = exception.Message,
            //Type = exception.GetType().Name            
            // changed to use custom error response
            statusCode,
            exception.Message,
            exception.GetType().Name,
            context.Request.Path 

        );

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private int GetStatusCode(Exception exception) => exception switch
    {
        ArgumentException => StatusCodes.Status400BadRequest,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };    

    private static string GetErrorMessage(Exception exception) => exception switch
    {
        ArgumentException => "Invalid input provided.",
        KeyNotFoundException => "Requested resource was not found.",
        UnauthorizedAccessException => "You are not authorized to access this resource.",
        _ => "An unexpected error occurred. Please try again later."
    };



}
