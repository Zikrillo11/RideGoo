namespace RideGoo.Shared.DTOs.Rating;

public class RatingForCreateDto
{
    public Guid OrderId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}