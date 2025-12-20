using Application.Services.Auth.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;
using Isopoh.Cryptography.Argon2;

namespace Infrastructure.Services.Implementations.Auth;

public class AdminManager : IAdminManager
{
    private readonly IAdminRepository _adminRepository;
    private readonly ILogger<IAdminManager> _logger;

    public AdminManager(IAdminRepository adminRepository, ILogger<IAdminManager> logger)
    {
        _adminRepository = adminRepository;
        _logger = logger;
    }
    
    public async Task CreateAsync(string email, string password)
    {
        await _adminRepository.AddAsync(new Admin
        {
            Email = email,
            PasswordHash = Argon2.Hash(password)
        });
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
}