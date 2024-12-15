using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Validators.Exceptions;

namespace Streetcode.WebApi.CustomMiddlewares
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException exception)
            {
                Console.WriteLine("ValidationException caught in ExceptionHandlingMiddleware");
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "ValidationFailure",
                    Title = "Validation error",
                    Detail = $"{exception.Errors.Count} validation errors has occurred"
                };

                if (exception.Errors is not null)
                {
                    var formattedErrors = exception.Errors.Select(error => new
                    {
                        errorProperty = error.PropertyName,
                        errorMessage = error.ErrorMessage
                    });

                    problemDetails.Extensions["errorsDetails"] = exception.Errors;
                }

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }
    }
}
