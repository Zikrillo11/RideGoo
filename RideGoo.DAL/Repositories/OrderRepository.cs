using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId) =>
        await Query().Where(o => o.CustomerId == customerId)
                      .OrderByDescending(o => o.CreatedAt)
                      .ToListAsync();

    public async Task<IEnumerable<Order>> GetByDriverIdAsync(Guid driverId) =>
        await Query().Where(o => o.DriverId == driverId)
                      .OrderByDescending(o => o.CreatedAt)
                      .ToListAsync();

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status) =>
        await Query().Where(o => o.Status == status).ToListAsync();
}