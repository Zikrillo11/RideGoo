namespace RideGoo.Shared.DTOs.User;

public class UserForCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = "Customer"; // Customer | Driver | Admin
    public string? LicenseNumber { get; set; } // Faqat Role=Driver bo'lsa kerak
}