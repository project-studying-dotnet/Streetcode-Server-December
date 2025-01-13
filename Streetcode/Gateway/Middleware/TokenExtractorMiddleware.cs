namespace Gateway.Middleware;

public class TokenExtractorMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
        {
            context.Request.Headers.Authorization = $"Bearer {token}";
        }

        await next(context);
    }
}