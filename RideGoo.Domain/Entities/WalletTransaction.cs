using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;
using RideGoo.Domain.ValueObjects;

namespace RideGoo.Domain.Entities;

public class WalletTransaction : BaseEntity
{
    public Guid WalletId { get; private set; }
    public Wallet Wallet { get; private set; } = null!;

    public Money Amount { get; private set; } = Money.Zero();
    public WalletTransactionType Type { get; private set; }
    public string? Description { get; private set; }

    private WalletTransaction() { }

    public static WalletTransaction Create(Guid walletId, Money amount, WalletTransactionType type, string? description)
    {
        return new WalletTransaction
        {
            WalletId = walletId,
            Amount = amount,
            Type = type,
            Description = description
        };
    }
}