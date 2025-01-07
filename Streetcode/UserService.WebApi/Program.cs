﻿using AspNetCore.Identity.Mongo;
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
}, mongoIdentityOptions =>
{
    mongoIdentityOptions.ConnectionString = mongoConnectionString;
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
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, RegistrationService>();
builder.Services.AddScoped<ITokenCleanupService, TokenCleanupService>();

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

RecurringJob.AddOrUpdate<TokenCleanupService>(
    "RemoveExpiredRefreshTokens",
    service => service.RemoveExpiredRefreshTokensAsync(),
    Cron.Minutely); // Щоденний запуск завдання

app.Run();
