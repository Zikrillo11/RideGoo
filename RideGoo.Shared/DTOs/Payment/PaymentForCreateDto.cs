namespace RideGoo.Shared.DTOs.Payment;

public class PaymentForCreateDto
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = "Cash";
}