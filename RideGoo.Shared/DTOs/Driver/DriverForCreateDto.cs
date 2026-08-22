namespace RideGoo.Shared.DTOs.Driver;

public class DriverForCreateDto
{
    public Guid UserId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
}