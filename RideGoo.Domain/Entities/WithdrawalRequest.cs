using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;

namespace RideGoo.Domain.Entities;

public class WithdrawalRequest : BaseEntity
{
    public Guid DriverId { get; private set; }
    public Driver Driver { get; private set; } = null!;

    public Money Amount { get; private set; } = Money.Zero();
    public WithdrawalStatus Status { get; private set; } = WithdrawalStatus.Pending;
    public string? CardNumber { get; private set; }
    public string? AdminComment { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private WithdrawalRequest() { }

    public static WithdrawalRequest Create(Guid driverId, Money amount, string? cardNumber)
    {
        if (amount.Amount <= 0)
            throw new DomainException("Yechiladigan summa 0 dan katta bo'lishi kerak.");

        return new WithdrawalRequest
        {
            DriverId = driverId,
            Amount = amount,
            CardNumber = cardNumber,
            Status = WithdrawalStatus.Pending
        };
    }

    public void Approve(string? adminComment = null)
    {
        if (Status != WithdrawalStatus.Pending)
            throw new DomainException("Faqat kutilayotgan so'rovni tasdiqlash mumkin.");

        Status = WithdrawalStatus.Approved;
        AdminComment = adminComment;
        ProcessedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void Reject(string? adminComment = null)
    {
        if (Status != WithdrawalStatus.Pending)
            throw new DomainException("Faqat kutilayotgan so'rovni rad etish mumkin.");

        Status = WithdrawalStatus.Rejected;
        AdminComment = adminComment;
        ProcessedAt = DateTime.UtcNow;
        MarkAsUpdated();
    }
}