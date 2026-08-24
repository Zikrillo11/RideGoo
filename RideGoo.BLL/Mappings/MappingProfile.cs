using AutoMapper;
using RideGoo.Domain.Entities;
using RideGoo.Shared.DTOs.Driver;
using RideGoo.Shared.DTOs.Notification;
using RideGoo.Shared.DTOs.Order;
using RideGoo.Shared.DTOs.Payment;
using RideGoo.Shared.DTOs.PromoCode;
using RideGoo.Shared.DTOs.Rating;
using RideGoo.Shared.DTOs.User;
using RideGoo.Shared.DTOs.Vehicle;
using RideGoo.Shared.DTOs.Wallet;

namespace RideGoo.BLL.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserForResultDto>();
        CreateMap<User, UserForShortResultDto>();

        CreateMap<Vehicle, VehicleForResultDto>();
        CreateMap<Vehicle, VehicleForShortResultDto>();

        CreateMap<Driver, DriverForResultDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User.FullName))
            .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.User.PhoneNumber))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CurrentLatitude, opt => opt.MapFrom(s => s.CurrentLocation != null ? s.CurrentLocation.Latitude : (double?)null))
            .ForMember(d => d.CurrentLongitude, opt => opt.MapFrom(s => s.CurrentLocation != null ? s.CurrentLocation.Longitude : (double?)null));

        CreateMap<Driver, DriverForShortResultDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User.FullName))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

        CreateMap<Order, OrderForResultDto>()
            .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer.FullName))
            .ForMember(d => d.DriverName, opt => opt.MapFrom(s => s.Driver != null ? s.Driver.User.FullName : null))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.EstimatedPrice, opt => opt.MapFrom(s => s.EstimatedPrice.Amount))
            .ForMember(d => d.FinalPrice, opt => opt.MapFrom(s => s.FinalPrice != null ? s.FinalPrice.Amount : (decimal?)null))
            .ForMember(d => d.Source, opt => opt.MapFrom(s => s.Source.ToString()));

        CreateMap<Order, OrderForShortResultDto>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.EstimatedPrice, opt => opt.MapFrom(s => s.EstimatedPrice.Amount));

        CreateMap<Payment, PaymentForResultDto>()
            .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
            .ForMember(d => d.Method, opt => opt.MapFrom(s => s.Method.ToString()));

        CreateMap<Rating, RatingForResultDto>()
            .ForMember(d => d.RatedByUserName, opt => opt.MapFrom(s => s.RatedByUser.FullName));
        CreateMap<Rating, RatingForShortResultDto>();

        CreateMap<Wallet, WalletForResultDto>()
            .ForMember(d => d.Balance, opt => opt.MapFrom(s => s.Balance.Amount))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Balance.Currency));

        CreateMap<WalletTransaction, WalletTransactionForResultDto>()
            .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));

        CreateMap<PromoCode, PromoCodeForResultDto>()
            .ForMember(d => d.DiscountType, opt => opt.MapFrom(s => s.DiscountType.ToString()));

        CreateMap<Notification, NotificationForResultDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
    }
}