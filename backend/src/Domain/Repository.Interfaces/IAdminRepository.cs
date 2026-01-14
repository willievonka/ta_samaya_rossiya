using Domain.Entities;

namespace Domain.Repository.Interfaces;

public interface IAdminRepository
{
    Task AddAsync(Admin admin);
    Task<Admin?> GetByEmailAsync(string email);
    Task<List<Admin>> GetAllAsync();
    Task DeleteAsync(Guid adminId);
}