using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;
using NetTopologySuite.IO.Converters;
using WebApi.Middleware;

namespace WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 268435456;
        });
        
        DotNetEnv.Env.TraversePath().Load();
        builder.Configuration.AddEnvironmentVariables();
        
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/app.log")
            .CreateLogger();

        builder.Host.UseSerilog();
        
        AddAuthentication(builder.Services, builder.Configuration);
        ConfigureServices(builder.Services, builder.Configuration);

        AddSwagger(builder.Services);
        
        var app = builder.Build();

        Directory.CreateDirectory("logs");

        ConfigureMiddleware(app);

        if (!app.Environment.IsEnvironment("Testing"))
        {
            await app.Services.CheckAndMigrateDatabaseAsync();
        }
        await app.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        
        services.AddInfrasctructureServices();
        
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
                                                          "http://localhost:8080",
                                                          "https://ta-samaya-rossiya.ru")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        services.AddInfrastructure(configuration);
        services.AddApplicationServices();
        services.AddEndpointsApiExplorer();
        
        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 268435456;
            options.ValueLengthLimit = 268435456;
        });
    }
    
    private static void AddAuthentication(IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddAuthentication(options =>
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
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    ClockSkew = TimeSpan.Zero,
                    
                };
            });
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Та САМАЯ Россия", Version = "v1" });
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath)) 
            {
                c.IncludeXmlComments(xmlPath);
            }

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Введите JWT токен: Bearer {ваш_токен}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });
        });
    }
    
    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        
        app.UseSerilogRequestLogging();
        app.UseCors("AllowAngularDev");
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseRouting();
        
        app.UseAuthentication();
        app.UseMiddleware<JwtBlacklistMiddleware>();
        app.UseAuthorization();
        
        app.MapControllers();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}