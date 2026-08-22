namespace RideGoo.Shared.DTOs.User;

public class UserForShortResultDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}