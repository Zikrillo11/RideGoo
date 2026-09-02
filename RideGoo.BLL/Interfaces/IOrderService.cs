using RideGoo.Shared.DTOs.Order;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface IOrderService
{
    Task<Result<OrderForResultDto>> CreateAsync(Guid customerId, OrderForCreateDto dto);
    Task<Result<OrderForResultDto>> GetByIdAsync(Guid id);
    Task<Result<PagedResult<OrderForShortResultDto>>> GetByCustomerIdAsync(Guid customerId, PaginationParams paginationParams);
    Task<Result<PagedResult<OrderForShortResultDto>>> GetByDriverIdAsync(Guid driverId, PaginationParams paginationParams);
    Task<Result<PagedResult<OrderForShortResultDto>>> GetPendingOrdersAsync(PaginationParams paginationParams);
    Task<Result<OrderForResultDto>> AcceptOrderAsync(Guid orderId, Guid driverId);
    Task<Result<OrderForResultDto>> UpdateStatusAsync(Guid orderId, OrderForUpdateDto dto);
}