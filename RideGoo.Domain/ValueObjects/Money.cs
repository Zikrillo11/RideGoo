using RideGoo.Domain.Exceptions;

namespace RideGoo.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "UZS";

    private Money() { }   // EF Core uchun

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "UZS")
    {
        if (amount < 0)
            throw new DomainException("Summa manfiy bo'lishi mumkin emas.");

        return new Money(amount, currency);
    }

    public static Money Zero(string currency = "UZS") => new(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = Amount - other.Amount;

        if (result < 0)
            throw new DomainException("Natija manfiy bo'lishi mumkin emas (yetarli mablag' yo'q).");

        return new Money(result, Currency);
    }

    public Money MultiplyBy(decimal factor)
    {
        if (factor < 0)
            throw new DomainException("Ko'paytiruvchi manfiy bo'lishi mumkin emas.");

        return new Money(Amount * factor, Currency);
    }

    public Money ApplyPercentageDiscount(decimal percentage)
    {
        if (percentage is < 0 or > 100)
            throw new DomainException("Foiz 0 dan 100 gacha bo'lishi kerak.");

        var discount = Amount * (percentage / 100m);
        return new Money(Amount - discount, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Turli valyutadagi summalarni birlashtirib bo'lmaydi.");
    }

    public bool Equals(Money? other) =>
        other is not null && Amount == other.Amount && Currency == other.Currency;

    public override bool Equals(object? obj) => Equals(obj as Money);
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
    public override string ToString() => $"{Amount:N2} {Currency}";
}