using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;

namespace RideGoo.Domain.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Order>> GetByDriverIdAsync(Guid driverId);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
}