using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
}