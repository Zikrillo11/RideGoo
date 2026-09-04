using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;

namespace RideGoo.Domain.Entities;

public class Order : BaseEntity
{

    public PaymentMethod PaymentMethod { get; private set; }
    public Guid CustomerId { get; private set; }
    public User Customer { get; private set; } = null!;

    public Guid? DriverId { get; private set; }
    public Driver? Driver { get; private set; }

    public string FromAddress { get; private set; } = string.Empty;
    public GeoLocation FromLocation { get; private set; } = null!;

    public string ToAddress { get; private set; } = string.Empty;
    public GeoLocation ToLocation { get; private set; } = null!;

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public OrderSource Source { get; private set; }

    public Money EstimatedPrice { get; private set; } = Money.Zero();
    public Money? FinalPrice { get; private set; }
    public double DistanceKm { get; private set; }

    public DateTime? AcceptedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? CancellationReason { get; private set; }

    public Payment? Payment { get; private set; }
    public Rating? Rating { get; private set; }
    public PromoRedemption? PromoRedemption { get; private set; }

    private Order() { }

    public static Order Create(Guid customerId, string fromAddress, GeoLocation fromLocation,
    string toAddress, GeoLocation toLocation, Money estimatedPrice, double distanceKm,
    OrderSource source, PaymentMethod paymentMethod)
    {
        return new Order
        {
            CustomerId = customerId,
            FromAddress = fromAddress,
            FromLocation = fromLocation,
            ToAddress = toAddress,
            ToLocation = toLocation,
            EstimatedPrice = estimatedPrice,
            DistanceKm = distanceKm,
            Source = source,
            PaymentMethod = paymentMethod,
            Status = OrderStatus.Pending
        };
    }
    public void ApplyPromo(PromoRedemption redemption, Money discountedPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Promo-kodni faqat kutilayotgan buyurtmaga qo'llash mumkin.");

        PromoRedemption = redemption;
        EstimatedPrice = discountedPrice;
        MarkAsUpdated();
    }

    public void Accept(Driver driver)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Faqat kutilayotgan (Pending) buyurtmani qabul qilish mumkin.");

        driver.StartTrip();

        DriverId = driver.Id;
        Driver = driver;
        Status = OrderStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void MarkDriverArrived()
    {
        if (Status != OrderStatus.Accepted)
            throw new DomainException("Faqat qabul qilingan buyurtmada haydovchi yetib kelishi mumkin.");

        Status = OrderStatus.DriverArrived;
        MarkAsUpdated();
    }

    public void StartTrip()
    {
        if (Status is not (OrderStatus.Accepted or OrderStatus.DriverArrived))
            throw new DomainException("Safarni boshlash uchun buyurtma avval qabul qilingan bo'lishi kerak.");

        Status = OrderStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void Complete(Money finalPrice)
    {
        if (Status != OrderStatus.InProgress)
            throw new DomainException("Faqat davom etayotgan safarni yakunlash mumkin.");

        Driver?.FinishTrip();

        Status = OrderStatus.Completed;
        FinalPrice = finalPrice;
        CompletedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void Cancel(string reason, bool cancelledByCustomer)
    {
        if (Status is OrderStatus.Completed or OrderStatus.CancelledByCustomer or OrderStatus.CancelledByDriver)
            throw new DomainException("Yakunlangan yoki allaqachon bekor qilingan buyurtmani bekor qilib bo'lmaydi.");

        Driver?.GoOnline();

        Status = cancelledByCustomer ? OrderStatus.CancelledByCustomer : OrderStatus.CancelledByDriver;
        CancellationReason = reason;
        MarkAsUpdated();
    }
}