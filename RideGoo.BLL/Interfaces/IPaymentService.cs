using RideGoo.Shared.DTOs.Payment;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentForResultDto>> CreateAsync(PaymentForCreateDto dto);
    Task<Result<PaymentForResultDto>> GetByOrderIdAsync(Guid orderId);
    Task<Result<PaymentForResultDto>> UpdateAsync(Guid id, PaymentForUpdateDto dto);
}