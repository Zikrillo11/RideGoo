using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;
using Xunit;

namespace RideGoo.Domain.Tests;

public class OrderTests
{
    // Yordamchi metod: har bir testda qaytadan yozmaslik uchun namunaviy buyurtma yaratadi
    private static Order CreateSampleOrder()
    {
        var fromLocation = GeoLocation.Create(41.311, 69.279);
        var toLocation = GeoLocation.Create(41.320, 69.290);
        var price = Money.Create(7000m);

        return Order.Create(
            customerId: Guid.NewGuid(),
            fromAddress: "Toshkent, Amir Temur ko'chasi",
            fromLocation: fromLocation,
            toAddress: "Toshkent, Mustaqillik maydoni",
            toLocation: toLocation,
            estimatedPrice: price,
            distanceKm: 1.5,
            source: OrderSource.Website,
            paymentMethod: PaymentMethod.Cash);
    }

    // Yordamchi metod: "online" (buyurtma qabul qilishga tayyor) haydovchi yaratadi
    private static Driver CreateOnlineDriver()
    {
        var driver = Driver.Create(Guid.NewGuid(), "AA1234567");
        var vehicle = Vehicle.Create(driver.Id, "Chevrolet", "Cobalt", "01A123AA", "Oq", 2020);

        driver.AssignVehicle(vehicle);
        driver.GoOnline();

        return driver;
    }

    [Fact]
    public void Create_YangiBuyurtma_PendingHolatdaYaratiladi()
    {
        var order = CreateSampleOrder();

        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Accept_OnlineHaydovchiBilan_QabulQilinadi()
    {
        var order = CreateSampleOrder();
        var driver = CreateOnlineDriver();

        order.Accept(driver);

        Assert.Equal(OrderStatus.Accepted, order.Status);
        Assert.Equal(driver.Id, order.DriverId);
    }

    [Fact]
    public void Accept_AllaqachonQabulQilinganBuyurtmani_XatolikBeradi()
    {
        var order = CreateSampleOrder();
        var driver1 = CreateOnlineDriver();
        var driver2 = CreateOnlineDriver();

        order.Accept(driver1);

        Assert.Throws<DomainException>(() => order.Accept(driver2));
    }

    [Fact]
    public void FullFlow_PendingDanCompletedGacha_TogriIshlaydi()
    {
        var order = CreateSampleOrder();
        var driver = CreateOnlineDriver();

        order.Accept(driver);
        order.MarkDriverArrived();
        order.StartTrip();
        order.Complete(Money.Create(7000m));

        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.NotNull(order.CompletedAt);
    }

    [Fact]
    public void Complete_PendingHolatdagiBuyurtmani_XatolikBeradi()
    {
        var order = CreateSampleOrder();

        Assert.Throws<DomainException>(() => order.Complete(Money.Create(7000m)));
    }

    [Fact]
    public void Cancel_PendingBuyurtmani_BekorQilishMumkin()
    {
        var order = CreateSampleOrder();

        order.Cancel("Mijoz fikridan qaytdi", cancelledByCustomer: true);

        Assert.Equal(OrderStatus.CancelledByCustomer, order.Status);
    }

    [Fact]
    public void Cancel_AllaqachonYakunlanganBuyurtmani_XatolikBeradi()
    {
        var order = CreateSampleOrder();
        var driver = CreateOnlineDriver();

        order.Accept(driver);
        order.MarkDriverArrived();
        order.StartTrip();
        order.Complete(Money.Create(7000m));

        Assert.Throws<DomainException>(() => order.Cancel("Sabab", true));
    }
}