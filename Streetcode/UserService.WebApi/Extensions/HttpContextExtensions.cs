using Microsoft.Extensions.Options;
using UserService.BLL.Services.Jwt;

namespace UserService.WebApi.Extensions
{
    public static class HttpContextExtensions
    {
        public static void AppendTokenToCookie(this HttpContext context, string token, IOptions<JwtConfiguration> options)
        {
            var _jwtConfiguration = options.Value;

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddSeconds(_jwtConfiguration.AccessTokenLifetime)
            };

            context.Response.Cookies.Append("AuthToken", token, cookieOptions);
        }
        public static void DeleteAuthTokenCookie(this HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Response.Cookies.Delete("AuthToken");
        }
    }
}