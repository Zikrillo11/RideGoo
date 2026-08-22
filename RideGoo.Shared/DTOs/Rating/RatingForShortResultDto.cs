namespace RideGoo.Shared.DTOs.Rating;

public class RatingForShortResultDto
{
    public Guid Id { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}