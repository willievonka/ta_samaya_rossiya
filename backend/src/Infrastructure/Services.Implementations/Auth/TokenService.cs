using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Services.Auth.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Implementations.Auth;

public class TokenService : ITokenService
{
    private readonly ILogger<ITokenService> _logger;
    private readonly IConfiguration _configuration;

    public TokenService(ILogger<ITokenService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
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

        var expires = _configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15";

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expires)),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        
        try
        {
            _logger.LogInformation("Get principal token for user {token}", token);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParametrs = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),

                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],

                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],

                ValidateLifetime = false
            };

            var principal = tokenHandler.ValidateToken(token, tokenValidationParametrs, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogError("Refresh failed: Invalid token algorithm.");
                return null;
            }
            
            return principal;
        }
        catch (Exception e)
        {
            _logger.LogError("Refresh failed: Token validation error. {Message}", e.Message);
            return null;
        }
    }
}