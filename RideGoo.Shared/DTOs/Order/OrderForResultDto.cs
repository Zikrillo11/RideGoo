namespace RideGoo.Shared.DTOs.Order;

public class OrderForResultDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? DriverName { get; set; }

    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public decimal EstimatedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public double DistanceKm { get; set; }
    public string Source { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}