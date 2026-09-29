using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Shared.DTOs.Auth;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<Result<AuthForResultDto>> RegisterAsync(AuthForRegisterDto dto)
    {
        try
        {
            var alreadyExists = await _unitOfWork.Users.ExistsByPhoneNumberAsync(dto.PhoneNumber);
            if (alreadyExists)
                return Result<AuthForResultDto>.Failure("Bu telefon raqam bilan foydalanuvchi allaqachon ro'yxatdan o'tgan.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = User.Register(dto.FullName, dto.PhoneNumber, passwordHash, dto.Email);

            await _unitOfWork.Users.AddAsync(user);

            var wallet = Wallet.CreateFor(user.Id);
            await _unitOfWork.Wallets.AddAsync(wallet);

            await _unitOfWork.SaveChangesAsync();

            var (token, expiresAt) = _jwtService.GenerateToken(user);

            return Result<AuthForResultDto>.Success(new AuthForResultDto
            {
                Token = token,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                ExpiresAt = expiresAt
            });
        }
        catch (DomainException ex)
        {
            return Result<AuthForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<AuthForResultDto>> LoginAsync(AuthForLoginDto dto)
    {
        var user = await _unitOfWork.Users.GetByPhoneNumberAsync(dto.PhoneNumber);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Result<AuthForResultDto>.Failure("Telefon raqam yoki parol noto'g'ri.");

        if (!user.IsActive)
            return Result<AuthForResultDto>.Failure("Hisobingiz bloklangan.");

        var (token, expiresAt) = _jwtService.GenerateToken(user);

        return Result<AuthForResultDto>.Success(new AuthForResultDto
        {
            Token = token,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt
        });
    }

    public async Task<Result<AuthForResultDto>> LoginWithTelegramAsync(string phoneNumber, long telegramChatId)
    {
        var user = await _unitOfWork.Users.GetByPhoneNumberAsync(phoneNumber);

        if (user is null)
            return Result<AuthForResultDto>.Failure("Bu raqam bilan hisob topilmadi. Avval Website orqali royxatdan oting.");

        if (!user.IsActive)
            return Result<AuthForResultDto>.Failure("Hisobingiz bloklangan.");

        user.LinkTelegram(telegramChatId);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        var (token, expiresAt) = _jwtService.GenerateToken(user);

        return Result<AuthForResultDto>.Success(new AuthForResultDto
        {
            Token = token,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt
        });
    }

    public async Task<Result<bool>> ChangePasswordAsync(Guid userId, AuthForChangePasswordDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user is null)
            return Result<bool>.Failure("Foydalanuvchi topilmadi.");

        if (!BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.PasswordHash))
            return Result<bool>.Failure("Joriy parol noto'g'ri.");

        try
        {
            var newHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ChangePassword(newHash);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (DomainException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}