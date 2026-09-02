using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Shared.DTOs.Driver;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class DriverService : IDriverService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DriverService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<DriverForResultDto>> CreateAsync(DriverForCreateDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
        if (user is null)
            return Result<DriverForResultDto>.Failure("Foydalanuvchi topilmadi.");

        var existingDriver = await _unitOfWork.Drivers.GetByUserIdAsync(dto.UserId);
        if (existingDriver is not null)
            return Result<DriverForResultDto>.Failure("Bu foydalanuvchi allaqachon haydovchi sifatida ro'yxatdan o'tgan.");

        try
        {
            var driver = Driver.Create(dto.UserId, dto.LicenseNumber);
            user.PromoteToDriver();

            await _unitOfWork.Drivers.AddAsync(driver);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.Drivers.Query()
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == driver.Id);

            return Result<DriverForResultDto>.Success(_mapper.Map<DriverForResultDto>(created));
        }
        catch (DomainException ex)
        {
            return Result<DriverForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<DriverForResultDto>> GetByIdAsync(Guid id)
    {
        var driver = await _unitOfWork.Drivers.Query()
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver is null)
            return Result<DriverForResultDto>.Failure("Haydovchi topilmadi.");

        return Result<DriverForResultDto>.Success(_mapper.Map<DriverForResultDto>(driver));
    }

    public async Task<Result<DriverForResultDto>> GetByUserIdAsync(Guid userId)
    {
        var driver = await _unitOfWork.Drivers.Query()
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (driver is null)
            return Result<DriverForResultDto>.Failure("Sizda haydovchi profili topilmadi.");

        return Result<DriverForResultDto>.Success(_mapper.Map<DriverForResultDto>(driver));
    }

    public async Task<Result<PagedResult<DriverForShortResultDto>>> GetOnlineDriversAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Drivers.Query()
            .Include(d => d.User)
            .Where(d => d.Status == DriverStatus.Online)
            .OrderByDescending(d => d.AverageRating);

        var totalCount = await query.CountAsync();

        var drivers = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return Result<PagedResult<DriverForShortResultDto>>.Success(new PagedResult<DriverForShortResultDto>
        {
            Items = _mapper.Map<List<DriverForShortResultDto>>(drivers),
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<DriverForResultDto>> UpdateAsync(Guid id, DriverForUpdateDto dto)
    {
        var driver = await _unitOfWork.Drivers.Query()
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (driver is null)
            return Result<DriverForResultDto>.Failure("Haydovchi topilmadi.");

        try
        {
            driver.UpdateLicenseNumber(dto.LicenseNumber);

            if (dto.Status == "Online") driver.GoOnline();
            else if (dto.Status == "Offline") driver.GoOffline();

            _unitOfWork.Drivers.Update(driver);
            await _unitOfWork.SaveChangesAsync();

            return Result<DriverForResultDto>.Success(_mapper.Map<DriverForResultDto>(driver));
        }
        catch (DomainException ex)
        {
            return Result<DriverForResultDto>.Failure(ex.Message);
        }
    }
}