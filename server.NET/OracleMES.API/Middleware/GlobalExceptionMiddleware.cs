

using System.Net;
using OracleMES.Core.DTOs;
using OracleMES.Core.Exceptions;
using OracleMES.COre.DTOs;

namespace OracleMES.API.Middleware;


public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IWebHostEnvironment env)
{

    private readonly RequestDelegate _next = next;

    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;
    private readonly IWebHostEnvironment _env = env;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();

                var buffer = new byte[context.Request.ContentLength.Value];

                await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);

                var bodyText = System.Text.Encoding.UTF8.GetString(buffer);

                context.Request.Body.Position = 0;


            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }

    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Code = GetErrorCode(exception),
            Message = GetErrorMessage(exception),
            Details = _env.IsDevelopment() ? exception.ToString() : null
        };

        response.StatusCode = GetStatusCode(exception);

        _logger.LogError(exception, "An error occurred: {Message}. Request Path: {Path}, Method: {Method}",
          exception.Message,
          context.Request.Path,
          context.Request.Method);

        var apiResponse = ApiResponse<object>.CreateError(errorResponse);
        await response.WriteAsJsonAsync(apiResponse);
    }

    private string GetErrorCode(Exception exception)
    {
        return exception switch
        {
            AppException appException => appException.ErrorCode,
            KeyNotFoundException => ErrorCodes.NotFound,
            UnauthorizedAccessException => ErrorCodes.Unauthorized,
            InvalidOperationException => ErrorCodes.BusinessRuleViolation,
            _ => "INTERNAL_SERVER_ERROR"
        };
    }

    private string GetErrorMessage(Exception exception)
    {
        return exception switch
        {
            AppException appException => string.Format(appException.Message, appException.Parameters),
            KeyNotFoundException => "The requested resource was not found.",
            UnauthorizedAccessException => "You are not authorized to perform this action.",
            InvalidOperationException => exception.Message,
            _ => "An unexpected error occurred."
        };
    }

    private int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            AppException appException => appException.ErrorCode switch
            {
                ErrorCodes.NotFound => (int)HttpStatusCode.NotFound,
                ErrorCodes.Unauthorized => (int)HttpStatusCode.Unauthorized,
                ErrorCodes.Forbidden => (int)HttpStatusCode.Forbidden,
                ErrorCodes.ValidationError => (int)HttpStatusCode.BadRequest,
                ErrorCodes.DuplicateEntry => (int)HttpStatusCode.Conflict,
                _ => (int)HttpStatusCode.BadRequest
            },
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

}

