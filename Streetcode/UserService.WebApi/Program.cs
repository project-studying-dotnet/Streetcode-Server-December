using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using UserService.DAL.Entities.Roles;
using UserService.DAL.Entities.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.BLL.Interfaces.Jwt;
using UserService.BLL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// MongoDB Configuration
string mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb")!;

builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    return new MongoClient(mongoConnectionString);
});


// Configuration Identity with MongoDB
var databaseName = builder.Configuration["MongoDb:DatabaseName"];

builder.Services.AddIdentity<User, Role>()
    .AddMongoDbStores<User, Role, Guid>(mongoConnectionString, databaseName)
    .AddDefaultTokenProviders();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);


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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    // Extracting token from cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.HttpContext.Request.Cookies["AuthToken"];
            return Task.CompletedTask;
        }
    };

});

builder.Services.AddScoped<IGenerateJwtService, GenerateJwtService>();

builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase("usersDB");
});

var app = builder.Build();

app.UseAuthentication();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
