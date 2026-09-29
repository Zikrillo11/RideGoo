using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;
using Xunit;

namespace RideGoo.Domain.Tests;

public class GeoLocationTests
{
    [Fact]
    public void Create_TogriQiymatlar_MuvaffaqiyatliYaratiladi()
    {
        // Arrange & Act
        var location = GeoLocation.Create(41.311, 69.279);

        // Assert
        Assert.Equal(41.311, location.Latitude);
        Assert.Equal(69.279, location.Longitude);
    }

    [Theory]
    [InlineData(91, 0)]
    [InlineData(-91, 0)]
    public void Create_NotogriLatitude_XatolikBeradi(double latitude, double longitude)
    {
        Assert.Throws<DomainException>(() => GeoLocation.Create(latitude, longitude));
    }

    [Theory]
    [InlineData(0, 181)]
    [InlineData(0, -181)]
    public void Create_NotogriLongitude_XatolikBeradi(double latitude, double longitude)
    {
        Assert.Throws<DomainException>(() => GeoLocation.Create(latitude, longitude));
    }

    [Fact]
    public void DistanceToKm_BirXilNuqta_NolgaTeng()
    {
        var location = GeoLocation.Create(41.311, 69.279);

        var distance = location.DistanceToKm(location);

        Assert.Equal(0, distance, precision: 3);
    }

    [Fact]
    public void DistanceToKm_ToshkentVaSamarqand_TaxminanTogriHisoblanadi()
    {
        var tashkent = GeoLocation.Create(41.2995, 69.2401);
        var samarkand = GeoLocation.Create(39.6270, 66.9750);

        var distance = tashkent.DistanceToKm(samarkand);

        // Haqiqiy masofa taxminan 250-280 km atrofida (to'g'ri chiziq bo'yicha)
        Assert.InRange(distance, 250, 280);
    }
}