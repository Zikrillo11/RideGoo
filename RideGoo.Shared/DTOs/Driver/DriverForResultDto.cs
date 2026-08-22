using RideGoo.Shared.DTOs.Vehicle;

namespace RideGoo.Shared.DTOs.Driver;

public class DriverForResultDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double? CurrentLatitude { get; set; }
    public double? CurrentLongitude { get; set; }
    public double AverageRating { get; set; }
    public int TotalTrips { get; set; }
    public VehicleForShortResultDto? Vehicle { get; set; }
}