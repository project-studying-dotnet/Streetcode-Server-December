using System.Text;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.BLL.Validators;
using Streetcode.WebApi.Extensions;
using Streetcode.WebApi.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureApplication();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerServices();
builder.Services.AddCustomServices();
builder.Services.ConfigureBlob(builder);
builder.Services.ConfigurePayment(builder);
builder.Services.ConfigureInstagram(builder);
builder.Services.ConfigureSerilog(builder);
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

if (app.Environment.EnvironmentName == "Local" || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
}
else
{
    app.UseHsts();
}

await app.ApplyMigrations();

// func to seed data
await app.SeedDataAsync();
app.UseCors();

app.UseHttpsRedirection();

app.UseGlobalExceptionHandler();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/dash");

if (app.Environment.EnvironmentName != "Local")
{
    BackgroundJob.Schedule<WebParsingUtils>(
    wp => wp.ParseZipFileFromWebAsync(), TimeSpan.FromMinutes(1));

    RecurringJob.AddOrUpdate<WebParsingUtils>(
        "ParseZipFileFromWeb_Monthly",
        wp => wp.ParseZipFileFromWebAsync(),
        Cron.Monthly,
        new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time") });

    RecurringJob.AddOrUpdate<BlobService>(
        "CleanBlobStorage_Monthly",
        b => b.CleanBlobStorage(),
        Cron.Monthly,
        new RecurringJobOptions { TimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time") });
}

app.MapControllers();

app.Run();
public partial class Program
{
}
