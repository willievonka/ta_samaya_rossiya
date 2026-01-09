namespace Domain.Entities;

/// <summary>
/// Refresh токен для механизма аутентификации
/// </summary>
public class RefreshToken
{
    public Guid Id { get; set; }
    
    public string Token { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public Guid UserId { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}