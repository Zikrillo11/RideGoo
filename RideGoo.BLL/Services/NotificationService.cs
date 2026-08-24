using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RideGoo.BLL.Interfaces;
using RideGoo.Domain.Entities;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Interfaces;
using RideGoo.Shared.DTOs.Notification;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<NotificationForResultDto>>> GetMyNotificationsAsync(Guid userId, PaginationParams paginationParams)
    {
        var query = _unitOfWork.Notifications.Query()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        var totalCount = await query.CountAsync();

        var notifications = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        return Result<PagedResult<NotificationForResultDto>>.Success(new PagedResult<NotificationForResultDto>
        {
            Items = _mapper.Map<List<NotificationForResultDto>>(notifications),
            PageNumber = paginationParams.PageNumber,
            PageSize = paginationParams.PageSize,
            TotalCount = totalCount
        });
    }

    public async Task<Result<bool>> MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification is null)
            return Result<bool>.Failure("Bildirishnoma topilmadi.");

        notification.MarkAsRead();
        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task CreateAsync(Guid userId, NotificationType type, string title, string message)
    {
        var notification = Notification.Create(userId, type, title, message);
        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }
}