namespace RideGoo.Shared.DTOs.Wallet;

public class WalletForResultDto
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "UZS";
}