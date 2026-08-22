using RideGoo.Domain.Common;
using RideGoo.Domain.Exceptions;

namespace RideGoo.Domain.Entities;

public class Vehicle : BaseEntity
{
    public Guid DriverId { get; private set; }
    public Driver Driver { get; private set; } = null!;

    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string PlateNumber { get; private set; } = string.Empty;
    public string Color { get; private set; } = string.Empty;
    public int Year { get; private set; }

    private Vehicle() { }

    public static Vehicle Create(Guid driverId, string brand, string model, string plateNumber, string color, int year)
    {
        if (year < 1990 || year > DateTime.UtcNow.Year)
            throw new DomainException($"Ishlab chiqarilgan yili 1990 dan {DateTime.UtcNow.Year} gacha bo'lishi kerak.");

        return new Vehicle
        {
            DriverId = driverId,
            Brand = brand,
            Model = model,
            PlateNumber = plateNumber,
            Color = color,
            Year = year
        };
    }

    public void UpdateDetails(string brand, string model, string plateNumber, string color, int year)
    {
        if (year < 1990 || year > DateTime.UtcNow.Year)
            throw new DomainException($"Ishlab chiqarilgan yili 1990 dan {DateTime.UtcNow.Year} gacha bo'lishi kerak.");

        Brand = brand;
        Model = model;
        PlateNumber = plateNumber;
        Color = color;
        Year = year;
        MarkAsUpdated();
    }
}