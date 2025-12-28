namespace Domain.Entities;

public class Admin
{ 
    public Guid Id { get; set; }
    
    public required string Email { get; set; }
    
    public required string PasswordHash { get; set; }
}