using Domain.Entities;
using Infrastructure.Persistence;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using WebApi;

namespace Tests.Integrations;

public class AuthTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MapDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<MapDbContext>(options =>
            {
                options.UseInMemoryDatabase("IntegrationTestDb");
            });
            
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MapDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Admins.Add(new Admin
            {
                Email = "admin.1.@admin.com",
                PasswordHash = Argon2.Hash("admin1Password")
            });
            
            db.Admins.Add(new Admin
            {
                Email = "admin.2.@admin.com",
                PasswordHash = Argon2.Hash("admin2Password")
            });
            
            db.SaveChanges();
        });
        
        builder.UseEnvironment("Testing");
    }
}