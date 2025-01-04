using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Добавляем конфигурацию Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Добавляем Ocelot и Swagger для Ocelot
builder.Services.AddControllers();
builder.Services.AddOcelot();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs"; 
});

// Middleware
app.UseCors();
app.UseHttpsRedirection();
app.UseSwaggerForOcelotUI();
await app.UseOcelot();
app.Run();