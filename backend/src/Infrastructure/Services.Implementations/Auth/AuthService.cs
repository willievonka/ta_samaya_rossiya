using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services.Auth.Interfaces;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Implementations.Auth;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBlacklistService _blacklistService;
    private readonly ILogger<IAuthService> _logger;

    public AuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
        IBlacklistService blacklistService, ILogger<IAuthService> logger)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _blacklistService = blacklistService;
        _logger = logger;
    }

    public Guid? GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return null;
        
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("sub");
        
        if (userIdClaim == null)
            return null;

        if (Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        
        return null;
    }

    public string GenerateJwtToken(Guid id, string email)
    {
        _logger.LogInformation("Generate jwt token for user {id}", id);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, id.ToString()),
            new (JwtRegisteredClaimNames.Email, email),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = _configuration["Jwt:ExpireHours"] ?? "5";

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(Convert.ToDouble(expires)),
            signingCredentials: credentials
            );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public void Logout()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        var jti = httpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var email = httpContext?.User.FindFirstValue(ClaimTypes.Email);
        var expClaim = httpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Exp);

        if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expClaim))
        {
            _logger.LogWarning("Logout failed for user {Email}: JTI or EXP claim is missing.", email);
        }
        
        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim!)).UtcDateTime;
        var remaining = expirationTime - DateTime.UtcNow;

        if (remaining > TimeSpan.Zero)
        {
            _blacklistService.AddTokenToBlackList(jti!, remaining);
            _logger.LogInformation("Token {Jti} blacklisted for {Remaining} min.", jti, remaining.TotalMinutes);
        }
    }
}