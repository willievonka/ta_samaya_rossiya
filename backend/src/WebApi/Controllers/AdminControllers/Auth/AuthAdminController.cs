using Application.Services.Auth.Interfaces;
using Application.Services.Dtos.Auth.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.AdminControllers.Auth;

[ApiController]
[Route("api/admin/auth")]
public class AuthAdminController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthAdminController> _logger;
    private readonly IWebHostEnvironment _environment;

    public AuthAdminController(IAuthService authService, ILogger<AuthAdminController> logger, 
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Войти в учётную запись администратора
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            _logger.LogWarning("Admin {email} already logged in.", request.Email);

            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var dto = await _authService.LoginAsync(request.Email, request.Password);
        if (dto == null) 
            return Unauthorized("Invalid credentials");

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = _environment.IsProduction(),
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(dto.RefreshTokenExpiresHours))
        };
        
        Response.Cookies.Append("refreshToken", dto.RefreshToken, cookieOptions);
        
        return Ok(new { accessToken = dto.AccessToken });
    }

    /// <summary>
    /// Обновить сессию через Refresh Token
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized("Refresh token missing");
        
        var authHeader = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return Unauthorized("Access token missing");
        
        var accessToken = authHeader.Substring("Bearer ".Length).Trim();
        
        var dto = await _authService.RefreshAsync(accessToken, refreshToken, ct);

        if (dto == null)
        {
            Response.Cookies.Delete("refreshToken");
            return Unauthorized("Invalid token session");
        }
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = _environment.IsProduction(), 
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(dto.RefreshTokenExpiresHours))
        };
        Response.Cookies.Append("refreshToken", dto.RefreshToken, cookieOptions);
        
        return Ok(new { accessToken = dto.AccessToken });
    }

    /// <summary>
    /// Выйти из учётной записи администратора
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var adminId = _authService.GetCurrentUserId();
        
        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = _environment.IsProduction(),
            SameSite = SameSiteMode.Lax
        });
        
        if (adminId == null)
            return Unauthorized("You are not logged in.");
        
        var logoutSuccess = await _authService.LogoutAsync(adminId.Value, ct);
        
        if (!logoutSuccess)
            return Unauthorized("Invalid user session.");
        
        return Ok();
    }
}