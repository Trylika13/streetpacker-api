namespace SP.Presentation.Dtos;

public class UserRegistrationDto
{
    public required string Email { get; set; }
    
    public required string Password { get; set; }
    
    public required string Username { get; set; }
}