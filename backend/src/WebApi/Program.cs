using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Infrastructure;
using Microsoft.OpenApi;
using Serilog;
using NetTopologySuite.IO.Converters;

namespace WebApi;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        DotNetEnv.Env.TraversePath().Load();
        builder.Configuration.AddEnvironmentVariables();
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/app.log")
            .CreateLogger();

        builder.Host.UseSerilog();
        
        builder.Services.AddInfrasctructureServices();
        
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
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
                options.JsonSerializerOptions.DefaultIgnoreCondition = 
                    JsonIgnoreCondition.WhenWritingNull;
            });
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularDev",
                policy => policy.WithOrigins("http://localhost:4200",
                                                          "http://localhost:3000",
                                                          "http://localhost:8080")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        services.AddInfrastructure(configuration);
        services.AddApplicationServices();
        services.AddEndpointsApiExplorer();
    }
    
    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseCors("AllowAngularDev");
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}