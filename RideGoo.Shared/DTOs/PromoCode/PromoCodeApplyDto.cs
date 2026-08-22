namespace RideGoo.Shared.DTOs.PromoCode;

public class PromoCodeApplyDto
{
    public Guid OrderId { get; set; }
    public string Code { get; set; } = string.Empty;
}