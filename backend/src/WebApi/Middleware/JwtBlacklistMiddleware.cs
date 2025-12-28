using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Services.Auth.Interfaces;

namespace WebApi.Middleware;

public class JwtBlacklistMiddleware
{
    private readonly IBlacklistService _blacklistService;
    private readonly RequestDelegate _next;

    public JwtBlacklistMiddleware(RequestDelegate next, IBlacklistService blacklistService)
    {
        _next = next;
        _blacklistService = blacklistService;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var jti = context.User?.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (!string.IsNullOrEmpty(jti) && _blacklistService.IsTokenBlacklisted($"bl_{jti}"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            var message = "У вас нет доступа к этому сервису.";
            await context.Response.WriteAsync(message);
            return;
        }
        
        await _next(context);
    }
}