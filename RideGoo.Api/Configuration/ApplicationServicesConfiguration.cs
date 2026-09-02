using FluentValidation;
using RideGoo.Api.Hubs;
using RideGoo.BLL.Interfaces;
using RideGoo.BLL.Mappings;
using RideGoo.BLL.Services;
using RideGoo.BLL.Validators.Auth;
using RideGoo.DAL.Repositories;
using RideGoo.Domain.Interfaces;

namespace RideGoo.Api.Configuration;

public static class ApplicationServicesConfiguration
{
    public static IServiceCollection AddApplicationServicesConfiguration(this IServiceCollection services)
    {
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // SignalR notification wrapper
        services.AddScoped<INotificationHub, NotificationHubService>();

        // Business Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IPromoCodeService, PromoCodeService>();
        services.AddScoped<INotificationService, NotificationService>();

        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        // FluentValidation
        services.AddValidatorsFromAssemblyContaining<AuthForRegisterDtoValidator>();

        return services;
    }
}