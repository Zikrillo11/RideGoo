using AutoMapper;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Domain.ValueObjects;
using RideGoo.Shared.DTOs.Withdrawal;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class WithdrawalService : IWithdrawalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WithdrawalService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WithdrawalForResultDto>> CreateAsync(Guid driverUserId, WithdrawalForCreateDto dto)
    {
        var driver = await _unitOfWork.Drivers.GetByUserIdAsync(driverUserId);
        if (driver is null)
            return Result<WithdrawalForResultDto>.Failure("Haydovchi profili topilmadi.");

        var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(driverUserId);
        if (wallet is null)
            return Result<WithdrawalForResultDto>.Failure("Hamyon topilmadi.");

        if (wallet.Balance.Amount < dto.Amount)
            return Result<WithdrawalForResultDto>.Failure("Hamyonda yetarli mablag' yo'q.");

        try
        {
            var request = WithdrawalRequest.Create(driver.Id, Money.Create(dto.Amount), dto.CardNumber);

            // Pulni darhol "muzlatamiz" — hamyondan yechib, so'rov holatiga o'tkazamiz.
            // Agar Admin rad etsa, pul qaytariladi.
            wallet.Pay(Money.Create(dto.Amount), "Pul yechish so'rovi");
            _unitOfWork.Wallets.Update(wallet);

            await _unitOfWork.WithdrawalRequests.AddAsync(request);
            await _unitOfWork.SaveChangesAsync();

            return Result<WithdrawalForResultDto>.Success(new WithdrawalForResultDto
            {
                Id = request.Id,
                DriverId = driver.Id,
                DriverName = driver.User?.FullName ?? "",
                Amount = request.Amount.Amount,
                Status = request.Status.ToString(),
                CardNumber = request.CardNumber,
                CreatedAt = request.CreatedAt
            });
        }
        catch (DomainException ex)
        {
            return Result<WithdrawalForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<List<WithdrawalForResultDto>>> GetMyRequestsAsync(Guid driverUserId)
    {
        var driver = await _unitOfWork.Drivers.GetByUserIdAsync(driverUserId);
        if (driver is null)
            return Result<List<WithdrawalForResultDto>>.Failure("Haydovchi profili topilmadi.");

        var requests = await _unitOfWork.WithdrawalRequests.GetByDriverIdAsync(driver.Id);

        return Result<List<WithdrawalForResultDto>>.Success(requests.Select(r => new WithdrawalForResultDto
        {
            Id = r.Id,
            DriverId = r.DriverId,
            DriverName = driver.User?.FullName ?? "",
            Amount = r.Amount.Amount,
            Status = r.Status.ToString(),
            CardNumber = r.CardNumber,
            AdminComment = r.AdminComment,
            CreatedAt = r.CreatedAt,
            ProcessedAt = r.ProcessedAt
        }).ToList());
    }

    public async Task<Result<List<WithdrawalForResultDto>>> GetPendingAsync()
    {
        var requests = await _unitOfWork.WithdrawalRequests.GetPendingAsync();

        return Result<List<WithdrawalForResultDto>>.Success(requests.Select(r => new WithdrawalForResultDto
        {
            Id = r.Id,
            DriverId = r.DriverId,
            DriverName = r.Driver?.User?.FullName ?? "",
            Amount = r.Amount.Amount,
            Status = r.Status.ToString(),
            CardNumber = r.CardNumber,
            AdminComment = r.AdminComment,
            CreatedAt = r.CreatedAt,
            ProcessedAt = r.ProcessedAt
        }).ToList());
    }

    public async Task<Result<WithdrawalForResultDto>> ProcessAsync(Guid requestId, WithdrawalForProcessDto dto)
    {
        var request = await _unitOfWork.WithdrawalRequests.GetByIdAsync(requestId);
        if (request is null)
            return Result<WithdrawalForResultDto>.Failure("So'rov topilmadi.");

        try
        {
            if (dto.Approve)
            {
                request.Approve(dto.AdminComment);
            }
            else
            {
                request.Reject(dto.AdminComment);

                // Rad etilsa — pulni haydovchi hamyoniga qaytaramiz
                var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);
                if (driver is not null)
                {
                    var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(driver.UserId);
                    if (wallet is not null)
                    {
                        wallet.Refund(request.Amount, "Rad etilgan pul yechish so'rovi qaytarildi");
                        _unitOfWork.Wallets.Update(wallet);
                    }
                }
            }

            _unitOfWork.WithdrawalRequests.Update(request);
            await _unitOfWork.SaveChangesAsync();

            return Result<WithdrawalForResultDto>.Success(new WithdrawalForResultDto
            {
                Id = request.Id,
                DriverId = request.DriverId,
                Amount = request.Amount.Amount,
                Status = request.Status.ToString(),
                CardNumber = request.CardNumber,
                AdminComment = request.AdminComment,
                CreatedAt = request.CreatedAt,
                ProcessedAt = request.ProcessedAt
            });
        }
        catch (DomainException ex)
        {
            return Result<WithdrawalForResultDto>.Failure(ex.Message);
        }
    }
}