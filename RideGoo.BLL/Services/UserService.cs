using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Shared.DTOs.User;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserForResultDto>> GetByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
            return Result<UserForResultDto>.Failure("Foydalanuvchi topilmadi.");

        return Result<UserForResultDto>.Success(_mapper.Map<UserForResultDto>(user));
    }

    public async Task<Result<PagedResult<UserForShortResultDto>>> GetAllAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Users.Query().OrderByDescending(u => u.CreatedAt);
        var totalCount = await query.CountAsync();

        var users = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return Result<PagedResult<UserForShortResultDto>>.Success(new PagedResult<UserForShortResultDto>
        {
            Items = _mapper.Map<List<UserForShortResultDto>>(users),
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<UserForResultDto>> UpdateAsync(Guid id, UserForUpdateDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
            return Result<UserForResultDto>.Failure("Foydalanuvchi topilmadi.");

        try
        {
            user.UpdateProfile(dto.FullName, dto.Email);

            if (dto.IsActive) user.Activate();
            else user.Deactivate();

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<UserForResultDto>.Success(_mapper.Map<UserForResultDto>(user));
        }
        catch (DomainException ex)
        {
            return Result<UserForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
            return Result<bool>.Failure("Foydalanuvchi topilmadi.");

        user.SoftDelete();
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<UserForResultDto>> CreateAsync(UserForCreateDto dto)
    {
        var alreadyExists = await _unitOfWork.Users.ExistsByPhoneNumberAsync(dto.PhoneNumber);
        if (alreadyExists)
            return Result<UserForResultDto>.Failure("Bu telefon raqam bilan foydalanuvchi allaqachon mavjud.");

        try
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = dto.Role == "Admin"
                ? Domain.Entities.User.RegisterAdmin(dto.FullName, dto.PhoneNumber, passwordHash)
                : Domain.Entities.User.Register(dto.FullName, dto.PhoneNumber, passwordHash, dto.Email);

            await _unitOfWork.Users.AddAsync(user);

            var wallet = Domain.Entities.Wallet.CreateFor(user.Id);
            await _unitOfWork.Wallets.AddAsync(wallet);

            if (dto.Role == "Driver")
            {
                user.PromoteToDriver();
                var driver = Domain.Entities.Driver.Create(user.Id, dto.LicenseNumber ?? "N/A");
                await _unitOfWork.Drivers.AddAsync(driver);
            }

            await _unitOfWork.SaveChangesAsync();

            return Result<UserForResultDto>.Success(_mapper.Map<UserForResultDto>(user));
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return Result<UserForResultDto>.Failure(ex.Message);
        }
    }
}