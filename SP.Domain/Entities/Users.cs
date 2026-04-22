using System;

namespace SP.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    
    public required string Username { get; set; }
    
    public required string PasswordHash { get; set; }
    
    public required string Email { get; set; }
    
    public string? Role { get; set; }
}