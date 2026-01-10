using Application.Services.Auth.Interfaces;
using Domain.Entities;
using Domain.Repository.Interfaces;
using Isopoh.Cryptography.Argon2;

namespace Infrastructure.Services.Implementations.Auth;

public class AdminManager : IAdminManager
{
    private readonly IAdminRepository _adminRepository;

    public AdminManager(IAdminRepository adminRepository)
    {
        _adminRepository = adminRepository;
    }
    
    public async Task CreateAsync(string email, string password)
    {
        await _adminRepository.AddAsync(new Admin
        {
            Email = email,
            PasswordHash = Argon2.Hash(password)
        });
    }
}