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

            options.LogTo(
                message => System.IO.File.AppendAllText(@"C:\temp\efcore-log.txt", message + Environment.NewLine),
                LogLevel.Information);

            options.EnableSensitiveDataLogging();
        });

        return services;
    }
}