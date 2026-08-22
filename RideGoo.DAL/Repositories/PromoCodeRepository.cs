using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class PromoCodeRepository : GenericRepository<PromoCode>, IPromoCodeRepository
{
    public PromoCodeRepository(AppDbContext context) : base(context) { }

    public async Task<PromoCode?> GetByCodeAsync(string code) =>
        await Query().Include(p => p.Redemptions)
                      .FirstOrDefaultAsync(p => p.Code == code.ToUpper());
}