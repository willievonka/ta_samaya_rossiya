namespace WebApi.Controllers.AdminControllers.Auth.Request;

public record AdminLoginRequest(
    string Email,
    string Password
    );