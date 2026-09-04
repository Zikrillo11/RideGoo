using RideGoo.Shared.DTOs.Withdrawal;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IWithdrawalService
{
    Task<Result<WithdrawalForResultDto>> CreateAsync(Guid driverUserId, WithdrawalForCreateDto dto);
    Task<Result<List<WithdrawalForResultDto>>> GetMyRequestsAsync(Guid driverUserId);
    Task<Result<List<WithdrawalForResultDto>>> GetPendingAsync();
    Task<Result<WithdrawalForResultDto>> ProcessAsync(Guid requestId, WithdrawalForProcessDto dto);
}