namespace RideGoo.Shared.DTOs.Vehicle;

public class VehicleForResultDto
{
    public Guid Id { get; set; }
    public Guid DriverId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Year { get; set; }
}