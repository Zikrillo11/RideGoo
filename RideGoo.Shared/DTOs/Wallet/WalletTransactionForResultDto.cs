namespace RideGoo.Shared.DTOs.Wallet;

public class WalletTransactionForResultDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}