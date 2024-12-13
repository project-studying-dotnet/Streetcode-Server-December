using System.Net;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Util;

namespace Streetcode.WebApi.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                LogException(ex);
                await HandleExceptionAsync(context, ex);
            }
        }

        private void LogException(Exception exception)
        {
            switch (exception)
            {
                case BadRequestException badRequestException:
                    _logger.LogWarning($"Bad Request: {badRequestException.Message}");
                    break;

                case BLL.Exceptions.UnauthorizedAccessException unauthorizedAccessException:
                    _logger.LogWarning($"Unauthorized Access: {unauthorizedAccessException.Message}");
                    break;

                case ForbiddenAccessException forbiddenAccessException:
                    _logger.LogWarning($"Forbidden Access: {forbiddenAccessException.Message}");
                    break;

                case NotFoundException notFoundException:
                    _logger.LogInformation($"Not Found: {notFoundException.Message}");
                    break;

                case ValidationException validationException:
                    _logger.LogInformation($"Validation Error: {validationException.Message}");
                    break;

                default:
                    _logger.LogError($"An unexpected error occurred: {exception.Message}");
                    break;
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = ExceptionStatusCodeMapper.MapToStatusCode(exception);
            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "An error occurred while processing your request.",
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}