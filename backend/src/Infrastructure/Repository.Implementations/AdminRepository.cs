using Domain.Entities;
using Domain.Repository.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations;

internal class AdminRepository : IAdminRepository
{
    private readonly MapDbContext _context;

    public AdminRepository(MapDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Admin admin)
    {
        await _context.AddAsync(admin);
        await _context.SaveChangesAsync();
    }

    public async Task<Admin?> GetByEmailAsync(string email)
    {
        return await _context.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<List<Admin>> GetAllAsync()
    {
        return await _context.Admins
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task DeleteAsync(Guid adminId)
    {
        var existingAdmin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);
        if (existingAdmin != null)
        {
            _context.Admins.Remove(existingAdmin);
            await _context.SaveChangesAsync();
        }
    }
}