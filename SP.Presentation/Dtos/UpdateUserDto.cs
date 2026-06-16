namespace SP.Presentation.Dtos; 

public class UpdateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? WhatsAppUrl { get; set; }
}