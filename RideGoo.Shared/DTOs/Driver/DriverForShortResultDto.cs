namespace RideGoo.Shared.DTOs.Driver;

public class DriverForShortResultDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public string Status { get; set; } = string.Empty;
}