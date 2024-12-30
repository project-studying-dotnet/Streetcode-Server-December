using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.DAL.Entities.Roles;
using UserService.DAL.Entities.Users;
using UserService.BLL.Interfaces.Jwt;
using UserService.BLL.Interfaces.User;
using System.Text;
using UserService.BLL.Services.Jwt;
using UserService.BLL.Services.User;
using UserService.BLL.Services;
using UserService.WebApi.Extensions;
using Microsoft.AspNetCore.Authorization;
using Streetcode.BLL.DTO.Users;
using System.Security.Claims;
using UserService.BLL.DTO.User;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// MongoDB Configuration
string mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")!;

builder.Services.AddIdentityMongoDbProvider<User, Role>(identityOptions =>
{
    identityOptions.Password.RequireDigit = false;
    identityOptions.Password.RequiredLength = 6;
    identityOptions.Password.RequireUppercase = false;
}, mongoIdentityOptions =>
{
    mongoIdentityOptions.ConnectionString = mongoConnectionString;
});


// JWT Configuration
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));
var jwtConfiguration = builder.Configuration.GetSection("Jwt").Get<JwtConfiguration>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfiguration.Issuer,
        ValidAudience = jwtConfiguration.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey)),
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Token = authorizationHeader.ToString().Split(" ").Last();
            }

            if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.TryGetValue("AuthToken", out var cookieToken))
            {
                context.Token = cookieToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<IClaimsService, ClaimsService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService.BLL.Services.User.RegistrationService>();

var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(currentAssemblies);


var app = builder.Build();
await app.SeedDataAsync();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapPost("/register", async (IUserService userService, RegistrationDTO registrationDto) =>
{
    var result = await userService.Registration(registrationDto);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
});

app.MapPost("/login", async (ILoginService loginService, IOptions<JwtConfiguration> jwtConfiguration, LoginDTO loginDto, HttpContext httpContext) =>
{
    var loginResult = await loginService.Login(loginDto);
    if (loginResult.IsFailed)
    {
        return Results.BadRequest(loginResult.Errors);
    }
    var token = loginResult.Value;
    httpContext.AppendTokenToCookie(token.AccessToken, jwtConfiguration); // Use jwtConfiguration.Value here
    return Results.Ok(new { token });
});

app.MapPost("/logout", async (ILoginService loginService, HttpContext httpContext, ClaimsPrincipal user) =>
{
    var logoutResult = await loginService.Logout(user);
    if (logoutResult.IsFailed)
    {
        return Results.BadRequest(logoutResult.Errors);
    }
    httpContext.DeleteAuthTokenCookie();
    return Results.Ok("User successfully logged out.");
});

app.MapPost("/refresh-token", async (ILoginService loginService, IOptions<JwtConfiguration> jwtConfiguration, TokenRequestDTO tokenRequest, HttpContext httpContext) =>
{
    var refreshResult = await loginService.RefreshToken(tokenRequest.RefreshToken);
    if (refreshResult.IsFailed)
    {
        return Results.BadRequest(refreshResult.Errors);
    }
    var token = refreshResult.Value;
    httpContext.AppendTokenToCookie(token.AccessToken, jwtConfiguration);
    return Results.Ok(token);
});

app.MapGet("/test-endpoint", [Authorize] () =>
{
    return Results.Ok("Hello from User Service");
});

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
