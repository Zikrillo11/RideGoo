using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using JwtSecurityToken = System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
using JwtSecurityTokenHandler = System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler;

namespace RideGoo.BLL.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiresAt) GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"]!;
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]!);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.MobilePhone, user.PhoneNumber),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}