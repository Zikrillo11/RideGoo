namespace RideGoo.Shared.DTOs.Vehicle;

public class VehicleForUpdateDto
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int Year { get; set; }
}