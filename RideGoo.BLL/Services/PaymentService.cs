using AutoMapper;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;
using RideGoo.Domain.Interfaces;
using RideGoo.Domain.ValueObjects;
using RideGoo.Shared.DTOs.Payment;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaymentForResultDto>> CreateAsync(PaymentForCreateDto dto)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(dto.OrderId);
        if (order is null)
            return Result<PaymentForResultDto>.Failure("Buyurtma topilmadi.");

        var existingPayment = await _unitOfWork.Payments.GetByOrderIdAsync(dto.OrderId);
        if (existingPayment is not null)
            return Result<PaymentForResultDto>.Failure("Bu buyurtma uchun to'lov allaqachon mavjud.");

        try
        {
            var payment = Payment.Create(dto.OrderId, Money.Create(dto.Amount), Enum.Parse<PaymentMethod>(dto.Method));

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentForResultDto>.Success(_mapper.Map<PaymentForResultDto>(payment));
        }
        catch (DomainException ex)
        {
            return Result<PaymentForResultDto>.Failure(ex.Message);
        }
    }

    public async Task<Result<PaymentForResultDto>> GetByOrderIdAsync(Guid orderId)
    {
        var payment = await _unitOfWork.Payments.GetByOrderIdAsync(orderId);
        if (payment is null)
            return Result<PaymentForResultDto>.Failure("To'lov topilmadi.");

        return Result<PaymentForResultDto>.Success(_mapper.Map<PaymentForResultDto>(payment));
    }

    public async Task<Result<PaymentForResultDto>> UpdateAsync(Guid id, PaymentForUpdateDto dto)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(id);
        if (payment is null)
            return Result<PaymentForResultDto>.Failure("To'lov topilmadi.");

        try
        {
            payment.ChangeMethod(Enum.Parse<PaymentMethod>(dto.Method));

            if (dto.IsPaid)
                payment.MarkAsPaid();

            _unitOfWork.Payments.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return Result<PaymentForResultDto>.Success(_mapper.Map<PaymentForResultDto>(payment));
        }
        catch (DomainException ex)
        {
            return Result<PaymentForResultDto>.Failure(ex.Message);
        }
    }
}