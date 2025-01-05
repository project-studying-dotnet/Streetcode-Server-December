using EmailService.BLL.Interfaces;
using EmailService.BLL.Services;
using EmailService.DAL.Entities;
using EmailService.BLL.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

namespace EmailService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

            builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);


            var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);

            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(currentAssemblies));

            builder.Services.AddScoped<IEmailService, BLL.Services.EmailService>();
            builder.Services.AddScoped<ILoggerService, LoggerService>();

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
        }
    }
}
