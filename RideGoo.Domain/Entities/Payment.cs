using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;

namespace RideGoo.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public Money Amount { get; private set; } = Money.Zero();
    public PaymentMethod Method { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, Money amount, PaymentMethod method)
    {
        return new Payment
        {
            OrderId = orderId,
            Amount = amount,
            Method = method,
            IsPaid = false
        };
    }

    public void MarkAsPaid()
    {
        if (IsPaid)
            throw new DomainException("Bu to'lov allaqachon amalga oshirilgan.");

        IsPaid = true;
        PaidAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void ChangeMethod(PaymentMethod method)
    {
        if (IsPaid)
            throw new DomainException("To'langan to'lovning usulini o'zgartirib bo'lmaydi.");

        Method = method;
        MarkAsUpdated();
    }
}