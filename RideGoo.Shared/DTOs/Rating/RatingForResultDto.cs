namespace RideGoo.Shared.DTOs.Rating;

public class RatingForResultDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string RatedByUserName { get; set; } = string.Empty;
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}