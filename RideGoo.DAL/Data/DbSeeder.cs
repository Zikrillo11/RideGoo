using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;

namespace RideGoo.DAL.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(AppDbContext context, IConfiguration configuration)
    {
        var adminExists = await context.Users.AnyAsync(u => u.Role == UserRole.Admin);
        if (adminExists) return;

        var phoneNumber = configuration["AdminSeed:PhoneNumber"];
        var password = configuration["AdminSeed:Password"];

        if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("[DbSeeder] OGOHLANTIRISH: AdminSeed:PhoneNumber / AdminSeed:Password sozlanmagan — standart Admin yaratilmadi. User Secrets orqali qo'shing.");
            return;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var admin = User.RegisterAdmin("Super Admin", phoneNumber, passwordHash);

        context.Users.Add(admin);

        var wallet = Wallet.CreateFor(admin.Id);
        context.Wallets.Add(wallet);

        await context.SaveChangesAsync();
    }
}