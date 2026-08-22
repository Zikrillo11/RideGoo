namespace RideGoo.Shared.DTOs.PromoCode;

public class PromoCodeForCreateDto
{
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = "Percentage";   // Percentage | FixedAmount
    public decimal DiscountValue { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public int MaxUsageCount { get; set; }
}