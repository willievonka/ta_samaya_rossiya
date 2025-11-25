using System.Reflection;
using Infrastructure;
using Microsoft.OpenApi;
using Serilog;

namespace WebApi;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/app.log")
            .CreateBootstrapLogger();

        builder.Host.UseSerilog();
        
        ConfigureServices(builder.Services, builder.Configuration);
        
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Та САМАЯ Россия", Version = "v1" });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        Directory.CreateDirectory("logs");

        ConfigureMiddleware(app);
        
        await app.Services.CheckAndMigrateDatabaseAsync();
        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost3000",
                policy => policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        services.AddInfrastructure(configuration);
        
        services.AddEndpointsApiExplorer();
    }
    
    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseCors("AllowLocalhost3000");
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();
    }
}