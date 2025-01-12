using EmailService.BLL.Interfaces;
using EmailService.BLL.Interfaces.Azure;
using EmailService.BLL.Services;
using EmailService.BLL.Services.Azure;
using EmailService.DAL.Entities;
using EmailService.BLL.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) 
    .Enrich.FromLogContext() 
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();


// Add services to the container.
builder.Services.AddFluentValidationAutoValidation(); 
builder.Services.AddValidatorsFromAssemblyContaining<EmailDtoValidator>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(Log.Logger);

builder.Services.AddScoped<IEmailService, EmailService.BLL.Services.EmailService>();
builder.Services.AddScoped<ILoggerService, LoggerService>();

// Setup email sending
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Setup MediatR
var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(currentAssemblies));

// Setup Azure service bus
var azureServiceBusConn = builder.Configuration.GetConnectionString("ServiceBusConn")!;
builder.Services.AddSingleton<IAzureServiceBus, AzureServiceBus>(sb => new AzureServiceBus(azureServiceBusConn));
builder.Services.AddHttpClient();

// Setup Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisCacheEmail:Configuration"];
    options.Configuration = builder.Configuration["RedisCacheEmail:InstanceName"];
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger(); 
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();

