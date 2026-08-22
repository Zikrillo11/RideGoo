using RideGoo.Domain.Common;
using RideGoo.Domain.Exceptions;

namespace RideGoo.Domain.Entities;

public class Rating : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;

    public Guid RatedByUserId { get; private set; }
    public User RatedByUser { get; private set; } = null!;

    public int Score { get; private set; }
    public string? Comment { get; private set; }

    private Rating() { }

    public static Rating Create(Guid orderId, Guid ratedByUserId, int score, string? comment)
    {
        if (score is < 1 or > 5)
            throw new DomainException("Baho 1 dan 5 gacha bo'lishi kerak.");

        if (comment is { Length: > 500 })
            throw new DomainException("Izoh 500 belgidan oshmasligi kerak.");

        return new Rating
        {
            OrderId = orderId,
            RatedByUserId = ratedByUserId,
            Score = score,
            Comment = comment
        };
    }
}