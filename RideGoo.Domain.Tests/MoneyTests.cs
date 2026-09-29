using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;
using Xunit;

namespace RideGoo.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Create_TogriSumma_MuvaffaqiyatliYaratiladi()
    {
        var money = Money.Create(50000m);

        Assert.Equal(50000m, money.Amount);
        Assert.Equal("UZS", money.Currency);
    }

    [Fact]
    public void Create_ManfiySumma_XatolikBeradi()
    {
        Assert.Throws<DomainException>(() => Money.Create(-100m));
    }

    [Fact]
    public void Subtract_YetarliMablagBorda_TogriNatijaQaytaradi()
    {
        var a = Money.Create(10000m);
        var b = Money.Create(3000m);

        var result = a.Subtract(b);

        Assert.Equal(7000m, result.Amount);
    }

    [Fact]
    public void Subtract_YetarliMablagYoqda_XatolikBeradi()
    {
        var a = Money.Create(1000m);
        var b = Money.Create(5000m);

        Assert.Throws<DomainException>(() => a.Subtract(b));
    }

    [Fact]
    public void ApplyPercentageDiscount_15Foiz_TogriHisoblanadi()
    {
        var money = Money.Create(10000m);

        var discounted = money.ApplyPercentageDiscount(15m);

        Assert.Equal(8500m, discounted.Amount);
    }

    [Fact]
    public void MultiplyBy_TogriHisoblanadi()
    {
        var money = Money.Create(2000m);

        var result = money.MultiplyBy(3);

        Assert.Equal(6000m, result.Amount);
    }
}