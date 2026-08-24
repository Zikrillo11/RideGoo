using AutoMapper;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Domain.ValueObjects;
using RideGoo.Shared.DTOs.Wallet;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WalletService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WalletForResultDto>> GetMyWalletAsync(Guid userId)
    {
        var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(userId);
        if (wallet is null)
            return Result<WalletForResultDto>.Failure("Hamyon topilmadi.");

        return Result<WalletForResultDto>.Success(_mapper.Map<WalletForResultDto>(wallet));
    }

    public async Task<Result<WalletForResultDto>> TopUpAsync(Guid userId, WalletTopUpDto dto)
    {
        var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(userId);
        if (wallet is null)
            return Result<WalletForResultDto>.Failure("Hamyon topilmadi.");

        try
        {
            wallet.TopUp(Money.Create(dto.Amount), dto.Description);

            // Diqqat: .Update(wallet) chaqirilmaydi — wallet allaqachon EF tomonidan kuzatilmoqda,
            // shuning uchun SaveChangesAsync o'zgarishlarni avtomatik aniqlaydi.
            await _unitOfWork.SaveChangesAsync();

            return Result<WalletForResultDto>.Success(_mapper.Map<WalletForResultDto>(wallet));
        }
        catch (DomainException ex)
        {
            return Result<WalletForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<List<WalletTransactionForResultDto>>> GetTransactionsAsync(Guid userId)
    {
        var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(userId);
        if (wallet is null)
            return Result<List<WalletTransactionForResultDto>>.Failure("Hamyon topilmadi.");

        return Result<List<WalletTransactionForResultDto>>.Success(
            _mapper.Map<List<WalletTransactionForResultDto>>(wallet.Transactions.OrderByDescending(t => t.CreatedAt)));
    }
}