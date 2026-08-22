using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;

namespace RideGoo.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { }

    public static Notification Create(Guid userId, NotificationType type, string title, string message)
    {
        return new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        if (IsRead) return;

        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}