using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.DAL.Entities.Roles;
using UserService.DAL.Entities.Users;
using UserService.BLL.Interfaces.Jwt;
using UserService.BLL.Interfaces.User;
using UserService.BLL.Services;
using UserService.WebApi.Extensions;

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
var jwtSettings = builder.Configuration.GetSection("Jwt");
// var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);


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
        // IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService.BLL.Services.UserService>();
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
app.MapControllers();

app.UseHttpsRedirection();

app.Run();
