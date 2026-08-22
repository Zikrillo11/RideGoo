using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface IPromoCodeRepository : IGenericRepository<PromoCode>
{
    Task<PromoCode?> GetByCodeAsync(string code);
}