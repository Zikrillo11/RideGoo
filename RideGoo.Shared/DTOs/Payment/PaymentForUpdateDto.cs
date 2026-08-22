namespace RideGoo.Shared.DTOs.Payment;

public class PaymentForUpdateDto
{
    public bool IsPaid { get; set; }
    public string Method { get; set; } = string.Empty;
}