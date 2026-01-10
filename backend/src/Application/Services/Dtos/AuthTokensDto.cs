namespace Application.Services.Dtos;

public class AuthTokensDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string RefreshTokenExpiresHours { get; set; }
}