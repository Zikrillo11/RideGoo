using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;

namespace RideGoo.Domain.Entities;

public class Wallet : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Money Balance { get; private set; } = Money.Zero();

    private readonly List<WalletTransaction> _transactions = new();
    public IReadOnlyCollection<WalletTransaction> Transactions => _transactions.AsReadOnly();

    private Wallet() { }

    public static Wallet CreateFor(Guid userId)
    {
        return new Wallet
        {
            UserId = userId,
            Balance = Money.Zero()
        };
    }

    public WalletTransaction TopUp(Money amount, string? description = null)
    {
        Balance = Balance.Add(amount);
        var transaction = WalletTransaction.Create(Id, amount, WalletTransactionType.TopUp, description);
        _transactions.Add(transaction);
        MarkAsUpdated();
        return transaction;
    }

    public WalletTransaction Pay(Money amount, string? description = null)
    {
        if (Balance.Amount < amount.Amount)
            throw new DomainException("Hamyonda yetarli mablag' yo'q.");

        Balance = Balance.Subtract(amount);
        var transaction = WalletTransaction.Create(Id, amount, WalletTransactionType.Payment, description);
        _transactions.Add(transaction);
        MarkAsUpdated();
        return transaction;
    }

    public WalletTransaction Receive(Money amount, string? description = null)
    {
        Balance = Balance.Add(amount);
        var transaction = WalletTransaction.Create(Id, amount, WalletTransactionType.TopUp, description);
        _transactions.Add(transaction);
        MarkAsUpdated();
        return transaction;
    }

    public WalletTransaction Refund(Money amount, string? description = null)
    {
        Balance = Balance.Add(amount);
        var transaction = WalletTransaction.Create(Id, amount, WalletTransactionType.Refund, description);
        _transactions.Add(transaction);
        MarkAsUpdated();
        return transaction;
    }
}