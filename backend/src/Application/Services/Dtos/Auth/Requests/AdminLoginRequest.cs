namespace Application.Services.Dtos.Auth.Requests;

public record AdminLoginRequest(
    string Email,
    string Password
    );