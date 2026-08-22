using RideGoo.Domain.Common;

namespace RideGoo.Domain.Entities;

public class PromoRedemption : BaseEntity
{
    public Guid PromoCodeId { get; private set; }
    public PromoCode PromoCode { get; private set; } = null!;

    public Guid CustomerId { get; private set; }
    public User Customer { get; private set; } = null!;

    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    private PromoRedemption() { }

    public static PromoRedemption Create(Guid promoCodeId, Guid customerId, Guid orderId)
    {
        return new PromoRedemption
        {
            PromoCodeId = promoCodeId,
            CustomerId = customerId,
            OrderId = orderId
        };
    }
}