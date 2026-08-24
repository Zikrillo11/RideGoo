using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;

namespace RideGoo.Api.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.LogTo(Console.WriteLine, LogLevel.Information);
            options.EnableSensitiveDataLogging(); // Parametr qiymatlarini ham ko'rsatadi (faqat development uchun!)
        });

        return services;
    }
}