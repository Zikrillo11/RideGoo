using AutoMapper;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Shared.DTOs.Vehicle;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class VehicleService : IVehicleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public VehicleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<VehicleForResultDto>> CreateAsync(VehicleForCreateDto dto)
    {
        var driver = await _unitOfWork.Drivers.GetByIdAsync(dto.DriverId);
        if (driver is null)
            return Result<VehicleForResultDto>.Failure("Haydovchi topilmadi.");

        try
        {
            var vehicle = Vehicle.Create(dto.DriverId, dto.Brand, dto.Model, dto.PlateNumber, dto.Color, dto.Year);

            driver.AssignVehicle(vehicle);

            await _unitOfWork.Vehicles.AddAsync(vehicle);
            _unitOfWork.Drivers.Update(driver);
            await _unitOfWork.SaveChangesAsync();

            return Result<VehicleForResultDto>.Success(_mapper.Map<VehicleForResultDto>(vehicle));
        }
        catch (DomainException ex)
        {
            return Result<VehicleForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<VehicleForResultDto>> GetByIdAsync(Guid id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle is null)
            return Result<VehicleForResultDto>.Failure("Mashina topilmadi.");

        return Result<VehicleForResultDto>.Success(_mapper.Map<VehicleForResultDto>(vehicle));
    }

    public async Task<Result<VehicleForResultDto>> GetByDriverIdAsync(Guid driverId)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByDriverIdAsync(driverId);
        if (vehicle is null)
            return Result<VehicleForResultDto>.Failure("Bu haydovchida mashina topilmadi.");

        return Result<VehicleForResultDto>.Success(_mapper.Map<VehicleForResultDto>(vehicle));
    }

    public async Task<Result<VehicleForResultDto>> UpdateAsync(Guid id, VehicleForUpdateDto dto)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle is null)
            return Result<VehicleForResultDto>.Failure("Mashina topilmadi.");

        try
        {
            vehicle.UpdateDetails(dto.Brand, dto.Model, dto.PlateNumber, dto.Color, dto.Year);

            _unitOfWork.Vehicles.Update(vehicle);
            await _unitOfWork.SaveChangesAsync();

            return Result<VehicleForResultDto>.Success(_mapper.Map<VehicleForResultDto>(vehicle));
        }
        catch (DomainException ex)
        {
            return Result<VehicleForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle is null)
            return Result<bool>.Failure("Mashina topilmadi.");

        vehicle.SoftDelete();
        _unitOfWork.Vehicles.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }
}