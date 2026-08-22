using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.ValueObjects;

namespace RideGoo.Domain.Entities;

public class Driver : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string LicenseNumber { get; private set; } = string.Empty;
    public DriverStatus Status { get; private set; } = DriverStatus.Offline;

    public GeoLocation? CurrentLocation { get; private set; }
    public DateTime? LastLocationUpdate { get; private set; }

    public double AverageRating { get; private set; } = 5.0;
    public int TotalTrips { get; private set; }

    public Vehicle? Vehicle { get; private set; }

    private readonly List<Order> _ordersAsDriver = new();
    public IReadOnlyCollection<Order> OrdersAsDriver => _ordersAsDriver.AsReadOnly();

    private Driver() { }

    public static Driver Create(Guid userId, string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new DomainException("Guvohnoma raqami bo'sh bo'lishi mumkin emas.");

        return new Driver
        {
            UserId = userId,
            LicenseNumber = licenseNumber,
            Status = DriverStatus.Offline,
            AverageRating = 5.0
        };
    }

    public void AssignVehicle(Vehicle vehicle)
    {
        if (Vehicle is not null)
            throw new DomainException("Haydovchida allaqachon mashina biriktirilgan.");

        Vehicle = vehicle;
        MarkAsUpdated();
    }

    public void UpdateLicenseNumber(string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new DomainException("Guvohnoma raqami bo'sh bo'lishi mumkin emas.");

        LicenseNumber = licenseNumber;
        MarkAsUpdated();
    }

    public void GoOnline()
    {
        if (Vehicle is null)
            throw new DomainException("Mashina biriktirilmagan haydovchi online bo'la olmaydi.");

        Status = DriverStatus.Online;
        MarkAsUpdated();
    }

    public void GoOffline()
    {
        if (Status == DriverStatus.OnTrip)
            throw new DomainException("Safar davomida haydovchi offline bo'la olmaydi.");

        Status = DriverStatus.Offline;
        MarkAsUpdated();
    }

    public void StartTrip()
    {
        if (Status != DriverStatus.Online)
            throw new DomainException("Faqat online haydovchi safarni boshlashi mumkin.");

        Status = DriverStatus.OnTrip;
        MarkAsUpdated();
    }

    public void FinishTrip()
    {
        Status = DriverStatus.Online;
        TotalTrips += 1;
        MarkAsUpdated();
    }

    public void UpdateLocation(GeoLocation location)
    {
        CurrentLocation = location;
        LastLocationUpdate = DateTime.UtcNow;
    }

    public void RecalculateAverageRating(IEnumerable<int> allScores)
    {
        var scores = allScores.ToList();
        AverageRating = scores.Count == 0 ? 5.0 : Math.Round(scores.Average(), 2);
        MarkAsUpdated();
    }
}