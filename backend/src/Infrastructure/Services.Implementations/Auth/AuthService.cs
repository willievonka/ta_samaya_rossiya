using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Services.Auth.Interfaces;
using Application.Services.Dtos.Auth;
using Domain.Entities;
using Domain.Repository.Interfaces;
using Isopoh.Cryptography.Argon2;

namespace Infrastructure.Services.Implementations.Auth;

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBlacklistService _blacklistService;
    private readonly ILogger<IAuthService> _logger;
    private readonly IAdminRepository _adminRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly string _refreshTokenExpires;
    
    public AuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
        IBlacklistService blacklistService, ILogger<IAuthService> logger, IAdminRepository adminRepository,
        IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _blacklistService = blacklistService;
        _logger = logger;
        _adminRepository = adminRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _refreshTokenExpires = configuration["Jwt:RefreshTokenExpirationHours"] ?? "12";
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

    public async Task<AuthTokensDto?> LoginAsync(string email, string password, CancellationToken ct)
    {
        var adminId = await ValidateAdminCredentialsAsync(email, password);

        if (adminId == null)
            return null;
        
        var accessToken = _tokenService.GenerateJwtToken(adminId.Value, email);
        var refreshTokenString = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenString,
            UserId = adminId.Value,
            ExpiresAt = DateTime.UtcNow.AddHours(Convert.ToDouble(_refreshTokenExpires)),
            CreatedAt = DateTime.UtcNow,
        };
        
        await _refreshTokenRepository.AddAsync(refreshToken, ct);
        
        _logger.LogInformation("Admin with id {adminId} has been authorized.", adminId);
        
        return new AuthTokensDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresHours = _refreshTokenExpires
        };
    }

    public async Task<Guid?> ValidateAdminCredentialsAsync(string email, string password)
    {
        var admin = await _adminRepository.GetByEmailAsync(email);

        if (admin == null)
        {
            _logger.LogError("Admin with {email} doesn't exist", email);
            return null;
        }

        if (Argon2.Verify(admin.PasswordHash, password))
        {
            return admin.Id;
        }
        
        _logger.LogError("Invalid credentials");
        return null;
    }

    public async Task<AuthTokensDto?> RefreshAsync(string accessToken, string refreshToken, CancellationToken ct)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
            return null;
        
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst("sub");
        var email = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirstValue("email");
        
        _logger.LogInformation("Refreshing token for user with id = {userId}", userIdClaim);

        if (string.IsNullOrEmpty(email) || !Guid.TryParse(userIdClaim!.Value, out var userId))
        {
            _logger.LogError("Refresh failed: Token claims are incomplete.");
            return null;
        }
        
        var savedRefreshToken = await _refreshTokenRepository.GetByTokenAndUserIdAsync(refreshToken, userId, ct);
        if (savedRefreshToken == null || savedRefreshToken.IsExpired)
        {
            _logger.LogError("Refresh token for user with id = {userId} is expired or doesn't exist", userId);
            return null;
        }
        
        await _refreshTokenRepository.DeleteByIdAsync(savedRefreshToken.Id, ct);
        
        var newAccessToken = _tokenService.GenerateJwtToken(userId, email);
        var newRefreshTokenString = _tokenService.GenerateRefreshToken();
        var newRefreshToken = new RefreshToken
        {
            Token = newRefreshTokenString,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(Convert.ToDouble(_refreshTokenExpires))
        };
        
        await _refreshTokenRepository.AddAsync(newRefreshToken, ct);

        return new AuthTokensDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresHours = _refreshTokenExpires
        };
    }

    public async Task<bool> LogoutAsync(Guid adminId, CancellationToken ct)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        var jti = httpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        var email = httpContext?.User.FindFirstValue(ClaimTypes.Email);
        var expClaim = httpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Exp);

        if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expClaim))
        {
            _logger.LogWarning("Logout failed for user {Email}: JTI or EXP claim is missing.", email);
            return false;
        }
        
        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
        var remaining = expirationTime - DateTime.UtcNow;

        if (remaining > TimeSpan.Zero)
        {
            await _blacklistService.AddTokenToBlackListAsync(jti, remaining, ct);
            _logger.LogInformation("Token {Jti} blacklisted for {Remaining} min.", jti, remaining.TotalMinutes);
        }

        var tokens = await _refreshTokenRepository.GetAllByUserIdAsync(adminId, ct);
        await _refreshTokenRepository.DeleteRangeAsync(tokens, ct);
        
        _logger.LogInformation("Admin with id {adminId} has been logged out.", adminId);
        return true;
    }
}