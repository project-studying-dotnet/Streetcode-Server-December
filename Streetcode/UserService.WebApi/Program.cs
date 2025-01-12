using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.DAL.Entities.Roles;
using UserService.DAL.Entities.Users;
using UserService.BLL.Interfaces.Jwt;
using UserService.BLL.Interfaces.User;
using System.Text;
using UserService.BLL.Interfaces.Azure;
using UserService.BLL.Services.Azure;
using UserService.BLL.Services.Jwt;
using UserService.BLL.Services.User;
using UserService.WebApi.Extensions;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Options;
using Streetcode.BLL.DTO.Users;
using UserService.BLL.DTO.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// MongoDB Configuration
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")!;
var azureServiceBusConn = builder.Configuration.GetConnectionString("ServiceBusConn")!;

builder.Services.AddIdentityMongoDbProvider<User, Role>(identityOptions =>
{
    identityOptions.Password.RequireDigit = false;
    identityOptions.Password.RequiredLength = 6;
    identityOptions.Password.RequireUppercase = false;

    // Configure token lifespan for email confirmation
    identityOptions.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
}, mongoIdentityOptions =>
{
    mongoIdentityOptions.ConnectionString = mongoConnectionString;
});

// Configure the token lifespan for the default token provider
var emailConfirmationTokenLifeSpan = 2;

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(emailConfirmationTokenLifeSpan); // Set token lifespan to 2 minutes
});

// JWT Configuration for DI
builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("Jwt"));

// JWT Configuration for Program.cs
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
builder.Services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, RegistrationService>();
builder.Services.AddScoped<ITokenCleanupService, TokenCleanupService>();
builder.Services.AddScoped<IUserPasswordService, UserPasswordService>();

var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(currentAssemblies);

builder.Services.AddSingleton<IAzureServiceBus, AzureServiceBus>(sb =>
    new AzureServiceBus(azureServiceBusConn));
builder.Services.AddHttpClient();

// Configure Hangfire MongoStorage with Migration
var migrationOptions = new MongoMigrationOptions
{
    MigrationStrategy = new MigrateMongoMigrationStrategy(),
    BackupStrategy = new CollectionMongoBackupStrategy()
};

var storageOptions = new MongoStorageOptions
{
    MigrationOptions = migrationOptions,
    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
};

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMongoStorage(builder.Configuration.GetConnectionString("Hangfire"), storageOptions));

builder.Services.AddHangfireServer();

var app = builder.Build();
await app.SeedDataAsync();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();

app.UseHttpsRedirection();
app.UseHangfireDashboard();

// Мінімальні API Endpoints
app.MapPost("/register", async (RegistrationDto registrationDto, IUserService userService) =>
{
    var result = await userService.Registration(registrationDto);
    return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
});

app.MapPost("/login", async (LoginDto loginDto, ILoginService loginService, HttpContext httpContext, IOptions<JwtConfiguration> jwtConfig) =>
{
    var loginResult = await loginService.Login(loginDto);
    if (loginResult.IsFailed)
        return Results.BadRequest(loginResult.Errors);

    var token = loginResult.Value;
    httpContext.AppendTokenToCookie(token.AccessToken, jwtConfig);
    return Results.Ok(new { token });
});

app.MapPost("/logout", async (HttpContext httpContext, ILoginService loginService) =>
{
    var logoutResult = await loginService.Logout(httpContext.User);
    if (logoutResult.IsFailed)
        return Results.BadRequest(logoutResult.Errors);

    httpContext.DeleteAuthTokenCookie();
    return Results.Ok("User successfully logged out.");
});

app.MapPost("/refresh-token", async (TokenRequestDTO tokenRequest, ILoginService loginService, HttpContext httpContext, IOptions<JwtConfiguration> jwtConfig) =>
{
    var refreshResult = await loginService.RefreshToken(tokenRequest.RefreshToken);
    if (refreshResult.IsFailed)
        return Results.BadRequest(refreshResult.Errors);

    var token = refreshResult.Value;
    httpContext.AppendTokenToCookie(token.AccessToken, jwtConfig);
    return Results.Ok(refreshResult.Value);
});

app.MapPost("/forgot-password", async (string email, IUserPasswordService userPasswordService, HttpContext httpContext, IOptions<JwtConfiguration> jwtConfig) =>
{
    var result = await userPasswordService.ForgotPassword(email);
    if (result.IsFailed)
        return Results.BadRequest(result.Errors);

    return Results.Ok();
});

app.MapPost("/reset-password", async (PassResetDto passResetDto, IUserPasswordService userPasswordService, HttpContext httpContext, IOptions<JwtConfiguration> jwtConfig) =>
{
    var result = await userPasswordService.ResetPassword(passResetDto);
    if (result.IsFailed)
        return Results.BadRequest(result.Errors);

    return Results.Ok();
});

app.MapGet("/confirm-email", async (HttpContext httpContext, string userId, string token, IEmailConfirmationService emailConfirmationService, IOptions<JwtConfiguration> jwtConfig) =>
{
    var result = await emailConfirmationService.ConfirmEmailAsync(userId, token);
    if (result.IsSuccess)
    {
        httpContext.AppendTokenToCookie(result.Value, jwtConfig);
        return Results.Ok(new { Token = result.Value });
    }
    else
    {
        return Results.BadRequest(result.Errors);
    }
});

app.MapPost("/change-password", async (PassChangeDto passChangeDto, IUserPasswordService userPasswordService, HttpContext httpContext, IOptions<JwtConfiguration> jwtConfig) =>
{
    if (!httpContext.User.Identity?.IsAuthenticated ?? false)
    {
        return Results.Unauthorized();
    }
    var userName = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    var result = await userPasswordService.ChangePassword(passChangeDto, userName);
    if (result.IsFailed)
        return Results.BadRequest(result.Errors);
    
    return Results.Ok();
}).RequireAuthorization();

RecurringJob.AddOrUpdate<TokenCleanupService>(
    "RemoveExpiredRefreshTokens",
    service => service.RemoveExpiredRefreshTokensAsync(),
    Cron.Daily); // Щоденний запуск завдання

app.Run();
