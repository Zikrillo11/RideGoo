using RideGoo.Domain.Common;
using RideGoo.Domain.Enums;
using RideGoo.Domain.Exceptions;

namespace RideGoo.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public long? TelegramChatId { get; private set; }

    public Driver? DriverProfile { get; private set; }
    public Wallet? Wallet { get; private set; }

    private readonly List<Order> _ordersAsCustomer = new();
    public IReadOnlyCollection<Order> OrdersAsCustomer => _ordersAsCustomer.AsReadOnly();

    private readonly List<Rating> _ratingsGiven = new();
    public IReadOnlyCollection<Rating> RatingsGiven => _ratingsGiven.AsReadOnly();

    private readonly List<Notification> _notifications = new();
    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

    private User() { }

    public static User Register(string fullName, string phoneNumber, string passwordHash, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Ism familiya bo'sh bo'lishi mumkin emas.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("Telefon raqam bo'sh bo'lishi mumkin emas.");

        return new User
        {
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            PasswordHash = passwordHash,
            Role = UserRole.Customer,
            IsActive = true
        };
    }

    public static User RegisterAdmin(string fullName, string phoneNumber, string passwordHash)
    {
        var user = Register(fullName, phoneNumber, passwordHash);
        user.Role = UserRole.Admin;
        return user;
    }

    public void PromoteToDriver()
    {
        if (Role == UserRole.Admin)
            throw new DomainException("Admin foydalanuvchini haydovchiga aylantirib bo'lmaydi.");

        Role = UserRole.Driver;
        MarkAsUpdated();
    }

    public void UpdateProfile(string fullName, string? email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Ism familiya bo'sh bo'lishi mumkin emas.");

        FullName = fullName;
        Email = email;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void LinkTelegram(long chatId)
    {
        TelegramChatId = chatId;
        MarkAsUpdated();
    }

    public void AttachWallet(Wallet wallet)
    {
        if (Wallet is not null)
            throw new DomainException("Foydalanuvchida allaqachon hamyon mavjud.");

        Wallet = wallet;
        MarkAsUpdated();
    }
}