using Application.Services.Auth.Interfaces;
using Application.Services.Interfaces;
using Domain.Repository.Interfaces;

namespace Infrastructure.Services.Implementations;

public class AdminSeederService : IAdminSeederService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IAdminManager _adminManager;
    private readonly ILogger<IAdminSeederService> _logger;
    private readonly IConfiguration _configuration;

    public AdminSeederService(IAdminRepository adminRepository, IAdminManager adminManager,
        ILogger<IAdminSeederService> logger, IConfiguration configuration)
    {
        _adminRepository = adminRepository;
        _adminManager = adminManager;
        _logger = logger;
        _configuration = configuration;
    }
    
    public async Task SeedIfNotExistAsync()
    {
        _logger.LogInformation("Table Admins starting seed");
        
        var index = 0;

        while (true)
        {
            var email = _configuration[$"ADMIN:{index}:EMAIL"];
            var password = _configuration[$"ADMIN:{index}:PASSWORD"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("Environment file does not contain email or password for creating admin number {index}"
                    , index);
                break;
            }
            
            var existingAdmin = await _adminRepository.GetByEmailAsync(email);
            if (existingAdmin != null)
            {
                _logger.LogWarning("Admin already exists for email {email}",  email);
                index++;
                continue;
            }
            
            await _adminManager.CreateAsync(email, password);
            _logger.LogInformation("Admin created for email {email}", email);
            
            index++;
        }
    }
}