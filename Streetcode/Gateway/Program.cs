using Gateway.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddOcelot();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


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

app.UseMiddleware<TokenExtractorMiddleware>();

app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs"; 
});

app.UseHttpsRedirection();       
app.UseRouting();                
app.UseCors();                   
app.UseAuthentication();         
app.UseAuthorization();          
app.UseSwaggerForOcelotUI();     
await app.UseOcelot();           
app.Run();                       
