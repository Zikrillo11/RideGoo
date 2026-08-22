using Microsoft.EntityFrameworkCore;
using RideGoo.DAL.Data;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Interfaces;

namespace RideGoo.DAL.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber) =>
        await Query().FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

    public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber) =>
        await Query().AnyAsync(u => u.PhoneNumber == phoneNumber);
}