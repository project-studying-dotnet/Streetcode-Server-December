using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Exceptions;
using Streetcode.BLL.Validators.Exceptions;

namespace Streetcode.WebApi.Middleware
{
    public sealed class GlobalExceptionHandlerMiddleware
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
            catch (HttpException ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, ex.Source);
                await HandleFluentValidationErrorsAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while processing the request to {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception is HttpException httpException
                ? httpException.StatusCode
                : StatusCodes.Status500InternalServerError;

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

        private async Task HandleFluentValidationErrorsAsync(HttpContext context, ValidationException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation error",
                Detail = $"{ex.Errors.Count} validation errors has occurred"
            };

            if (ex.Errors is not null)
            {
                var formattedErrors = ex.Errors.Select(error => new
                {
                    errorProperty = error.PropertyName,
                    errorMessage = error.ErrorMessage
                });

                problemDetails.Extensions["errorsDetails"] = ex.Errors;
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}