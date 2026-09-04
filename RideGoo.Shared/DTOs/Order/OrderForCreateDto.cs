namespace RideGoo.Shared.DTOs.Order;

public class OrderForCreateDto
{
    public string FromAddress { get; set; } = string.Empty;
    public double FromLatitude { get; set; }
    public double FromLongitude { get; set; }

    public string ToAddress { get; set; } = string.Empty;
    public double ToLatitude { get; set; }
    public double ToLongitude { get; set; }

    public string Source { get; set; } = "Website";
    public string? PromoCode { get; set; }
    public string PaymentMethod { get; set; } = "Cash"; // Cash | Card
}