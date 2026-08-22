using RideGoo.Domain.Entities;

namespace RideGoo.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByPhoneNumberAsync(string phoneNumber);
    Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);
}