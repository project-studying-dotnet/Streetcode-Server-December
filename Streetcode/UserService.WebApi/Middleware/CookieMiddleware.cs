using UserService.BLL.Services.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text;

namespace UserService.WebApi.Middleware
{
    public class CookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CookieMiddleware> _logger;
        private readonly JwtConfiguration _jwtConfiguration;

        public CookieMiddleware(RequestDelegate next, ILogger<CookieMiddleware> logger, IOptions<JwtConfiguration> options)
        {
            _next = next;
            _logger = logger;
            _jwtConfiguration = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // If the token is present, pass control to the next middleware
            if (IsTokenPresent(context))
            {
                await _next(context);
                return;
            }

            await InterceptResponseAsync(context);
        }

        // Checking if the token is present in the request
        private bool IsTokenPresent(HttpContext context)
        {
            // Check Authorization header for token
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var tokenFromHeader = authHeader.ToString().Split(' ').Last();
                if (!string.IsNullOrEmpty(tokenFromHeader))
                {
                    _logger.LogInformation("Token found in Authorization header.");
                    return true;
                }
            }

            // Check cookies for token
            if (context.Request.Cookies.TryGetValue("AuthToken", out var tokenFromCookie))
            {
                _logger.LogInformation("Token found in cookies.");
                return true;
            }

            return false;
        }

        // Intercepting and process the server response
        private async Task InterceptResponseAsync(HttpContext context)
        {
            // Save the original response body stream
            var originalBodyStream = context.Response.Body;

            // Create a new memory stream to temporarily hold the response body
            using var memoryStream = new MemoryStream();

            // Replace the response body with the memory stream
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                // If the response is in JSON format, extract the token
                if (IsJsonResponse(context.Response))
                {
                    var token = await ExtractTokenFromResponseAsync(memoryStream);
                    if (!string.IsNullOrEmpty(token))
                    {
                        AppendTokenToCookie(context, token);
                    }
                }

                // Copy the modified response back to the original stream
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Copy the modified response back to the original stream
                await memoryStream.CopyToAsync(originalBodyStream);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private bool IsJsonResponse(HttpResponse response)
        {
            return response.ContentType != null &&
                   response.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);
        }

        // Extracting the token from the JSON response
        private async Task<string?> ExtractTokenFromResponseAsync(MemoryStream memoryStream)
        {
            // Reset the memory stream position to the beginning
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Read the response body
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

            memoryStream.Seek(0, SeekOrigin.Begin);

            try
            {
                var json = JsonDocument.Parse(responseBody);
                if (json.RootElement.TryGetProperty("token", out var tokenElement))
                {
                    return tokenElement.GetString();
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse response body for token.");
            }

            return null;
        }

        // Adding the token to the cookie
        private void AppendTokenToCookie(HttpContext context, string token)
        {
            if (context.Request.Cookies.TryGetValue("AuthToken", out var existingToken) && existingToken == token)
            {
                _logger.LogInformation("Token already exists in cookies, skipping cookie update.");
                return;
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(_jwtConfiguration.AccessTokenLifetime)
            };

            context.Response.Cookies.Append("AuthToken", token, cookieOptions);
            _logger.LogInformation("AuthToken cookie set by middleware.");
        }
    }
}
