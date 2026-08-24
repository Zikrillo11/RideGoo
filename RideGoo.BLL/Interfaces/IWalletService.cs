using RideGoo.Shared.DTOs.Wallet;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IWalletService
{
    Task<Result<WalletForResultDto>> GetMyWalletAsync(Guid userId);
    Task<Result<WalletForResultDto>> TopUpAsync(Guid userId, WalletTopUpDto dto);
    Task<Result<List<WalletTransactionForResultDto>>> GetTransactionsAsync(Guid userId);
}