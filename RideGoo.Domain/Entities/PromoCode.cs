using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;

namespace RideGoo.Domain.Entities;

public class PromoCode : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public PromoDiscountType DiscountType { get; private set; }
    public decimal DiscountValue { get; private set; }      // Foiz (0-100) yoki summa (Money.Amount)
    public DateTime ValidFrom { get; private set; }
    public DateTime ValidTo { get; private set; }
    public int MaxUsageCount { get; private set; }
    public int CurrentUsageCount { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<PromoRedemption> _redemptions = new();
    public IReadOnlyCollection<PromoRedemption> Redemptions => _redemptions.AsReadOnly();

    private PromoCode() { }

    public static PromoCode Create(string code, PromoDiscountType discountType, decimal discountValue,
        DateTime validFrom, DateTime validTo, int maxUsageCount)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Promo-kod bo'sh bo'lishi mumkin emas.");

        if (discountType == PromoDiscountType.Percentage && discountValue is <= 0 or > 100)
            throw new DomainException("Foizli chegirma 0 dan 100 gacha bo'lishi kerak.");

        if (discountType == PromoDiscountType.FixedAmount && discountValue <= 0)
            throw new DomainException("Belgilangan summa 0 dan katta bo'lishi kerak.");

        if (validTo <= validFrom)
            throw new DomainException("Tugash sanasi boshlanish sanasidan keyin bo'lishi kerak.");

        return new PromoCode
        {
            Code = code.ToUpperInvariant(),
            DiscountType = discountType,
            DiscountValue = discountValue,
            ValidFrom = validFrom,
            ValidTo = validTo,
            MaxUsageCount = maxUsageCount,
            CurrentUsageCount = 0,
            IsActive = true
        };
    }

    public Money ApplyDiscount(Money originalPrice)
    {
        EnsureUsable();

        return DiscountType switch
        {
            PromoDiscountType.Percentage => originalPrice.ApplyPercentageDiscount(DiscountValue),
            PromoDiscountType.FixedAmount => originalPrice.Subtract(Money.Create(DiscountValue, originalPrice.Currency)),
            _ => originalPrice
        };
    }

    public PromoRedemption Redeem(Guid customerId, Guid orderId)
    {
        EnsureUsable();

        CurrentUsageCount += 1;
        var redemption = PromoRedemption.Create(Id, customerId, orderId);
        _redemptions.Add(redemption);
        MarkAsUpdated();

        return redemption;
    }

    private void EnsureUsable()
    {
        if (!IsActive)
            throw new DomainException("Bu promo-kod faol emas.");

        var now = DateTime.UtcNow;
        if (now < ValidFrom || now > ValidTo)
            throw new DomainException("Promo-kodning amal qilish muddati tugagan yoki hali boshlanmagan.");

        if (CurrentUsageCount >= MaxUsageCount)
            throw new DomainException("Promo-kodning ishlatilish limiti tugagan.");
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }
}