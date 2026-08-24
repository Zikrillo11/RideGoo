using RideGoo.Domain.Entities;

namespace RideGoo.BLL.Interfaces;

public interface IJwtService
{
    (string token, DateTime expiresAt) GenerateToken(User user);
}