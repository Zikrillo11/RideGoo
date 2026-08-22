namespace RideGoo.Shared.DTOs.Order;

public class OrderForUpdateDto
{
    public string Status { get; set; } = string.Empty;
    public decimal? FinalPrice { get; set; }
    public string? CancellationReason { get; set; }
}