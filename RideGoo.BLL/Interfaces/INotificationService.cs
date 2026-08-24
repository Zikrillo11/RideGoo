using RideGoo.Domain.Enums;
using RideGoo.Shared.DTOs.Notification;
using RideGoo.Shared.Params;
using RideGoo.Shared.Wrappers;

namespace RideGoo.BLL.Interfaces;

public interface INotificationService
{
    Task<Result<PagedResult<NotificationForResultDto>>> GetMyNotificationsAsync(Guid userId, PaginationParams paginationParams);
    Task<Result<bool>> MarkAsReadAsync(Guid notificationId);
    Task CreateAsync(Guid userId, NotificationType type, string title, string message);
}