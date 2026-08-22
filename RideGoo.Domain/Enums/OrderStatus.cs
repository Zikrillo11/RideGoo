namespace RideGoo.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    Accepted = 2,
    DriverArrived = 3,
    InProgress = 4,
    Completed = 5,
    CancelledByCustomer = 6,
    CancelledByDriver = 7
}