using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Streetcode.BLL.Interfaces.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.HolidayFormatter;
using Streetcode.BLL.Interfaces.Image;
using Streetcode.BLL.Interfaces.Instagram;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Payment;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.Services.Audio;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.BLL.Services.Email;
using Streetcode.BLL.Services.HolidayDate;
using Streetcode.BLL.Services.HolidayFormatter;
using Streetcode.BLL.Services.Image;
using Streetcode.BLL.Services.Instagram;
using Streetcode.BLL.Services.Logging;
using Streetcode.BLL.Services.OpenAI;
using Streetcode.BLL.Services.Payment;
using Streetcode.BLL.Services.Text;
using Streetcode.BLL.Validators;
using Streetcode.DAL.Caching.RedisCache;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.WebApi.Controllers.HolidayDate.Parsers;

namespace Streetcode.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddRepositoryServices();
            services.AddFeatureManagement();
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddAutoMapper(currentAssemblies);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(currentAssemblies));
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IInstagramService, InstagramService>();
            services.AddScoped<ITextService, AddTermsToTextService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IAudioService, AudioService>();

            services.AddHttpClient("OpenAI_Client", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(300);
            });
            services.AddScoped<IHolidayFormatter, HolidayFormatter>();
            services.AddScoped<HolidayDateService>();
            services.AddScoped<HolidaySource1Parser>();

            services.AddValidatorsFromAssembly(typeof(ValidationError).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
            var connectionString = configuration.GetValue<string>($"{environment}:ConnectionStrings:DefaultConnection");
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);

            services.AddDbContext<StreetcodeDbContext>(options =>
            {
                options.UseSqlServer(connectionString, opt =>
                {
                    opt.MigrationsAssembly(typeof(StreetcodeDbContext).Assembly.GetName().Name);
                    opt.MigrationsHistoryTable("__EFMigrationsHistory", schema: "entity_framework");
                });
            });
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(connectionString);
            });

            //OpenAI
            services.AddScoped<OpenAIService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var apiKey = configuration["OpenAiApiKey"];
                return new OpenAIService(httpClientFactory, apiKey);
            });


            // Redis-Caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisCache:Configuration"];
                options.InstanceName = configuration["RedisCache:InstanceName"];
            });

            services.AddHangfireServer();

            var corsConfig = configuration.GetSection("CORS").Get<CorsConfiguration>();
            services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            services.AddHsts(opt =>
            {
                opt.Preload = true;
                opt.IncludeSubDomains = true;
                opt.MaxAge = TimeSpan.FromDays(30);
            });

            services.AddLogging();
            services.AddControllers();
        }

        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });
                opt.CustomSchemaIds(x => x.FullName);
            });
        }

        public class CorsConfiguration
        {
            public List<string> AllowedOrigins { get; set; }
            public List<string> AllowedHeaders { get; set; }
            public List<string> AllowedMethods { get; set; }
            public int PreflightMaxAge { get; set; }
        }
    }
}