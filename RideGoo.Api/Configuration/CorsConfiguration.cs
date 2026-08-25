namespace RideGoo.Api.Configuration;

public static class CorsConfiguration
{
    private const string PolicyName = "RideGooWebPolicy";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy.WithOrigins("http://localhost:5173", "https://localhost:5173") // Vite dev server manzili
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app)
    {
        return app.UseCors(PolicyName);
    }
}