namespace RideGoo.Shared.DTOs.Order;

public class OrderForShortResultDto
{
    public Guid Id { get; set; }
    public string FromAddress { get; set; } = string.Empty;
    public string ToAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}