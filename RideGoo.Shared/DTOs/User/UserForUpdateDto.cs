namespace RideGoo.Shared.DTOs.User;

public class UserForUpdateDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}