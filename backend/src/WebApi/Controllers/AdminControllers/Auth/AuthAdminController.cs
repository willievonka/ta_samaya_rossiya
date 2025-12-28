using Application.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.AdminControllers.Auth.Request;

namespace WebApi.Controllers.AdminControllers.Auth;

[ApiController]
[Route("api/admin/auth")]
public class AuthAdminController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthAdminController> _logger;
    private readonly IAdminManager _adminManager;

    public AuthAdminController(IAuthService authService, ILogger<AuthAdminController> logger, IAdminManager adminManager)
    {
        _authService = authService;
        _logger = logger;
        _adminManager = adminManager;
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

        var adminId = await _adminManager.ValidateAdminCredentialsAsync(request.Email, request.Password);

        if (adminId == null) 
            return Unauthorized("Invalid credentials");
        
        var token = _authService.GenerateJwtToken(adminId!.Value, request.Email);
        
        _logger.LogInformation("Admin with id {adminId} has been authorized.", adminId);
        
        return Ok(token);
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
    public IActionResult Logout()
    {
        var adminId = _authService.GetCurrentUserId();
        if (adminId == null)
            return Unauthorized("You are not logged in.");
        
        _authService.Logout();
        _logger.LogInformation("Admin with id {adminId} has been logged out.", adminId);
        return Ok();
    }
}