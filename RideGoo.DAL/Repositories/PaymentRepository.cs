using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId) =>
        await Query().FirstOrDefaultAsync(p => p.OrderId == orderId);
}