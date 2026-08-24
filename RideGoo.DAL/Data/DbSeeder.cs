using Microsoft.EntityFrameworkCore;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;

namespace RideGoo.DAL.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(AppDbContext context)
    {
        var adminExists = await context.Users.AnyAsync(u => u.Role == UserRole.Admin);
        if (adminExists) return;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var admin = User.RegisterAdmin("Super Admin", "+998900000000", passwordHash);

        context.Users.Add(admin);

        var wallet = Wallet.CreateFor(admin.Id);
        context.Wallets.Add(wallet);

        await context.SaveChangesAsync();
    }
}