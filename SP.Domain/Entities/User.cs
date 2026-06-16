using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    
    public required string Username { get; set; }
    
    public required string PasswordHash { get; set; }
    
    public required string Email { get; set; }
    
    public string? Role { get; set; }
    public string? AvatarUrl { get; set; }
    
    [Column("WhatsAppUrl")]
    public string? WhatsAppUrl { get; set; }
    
    public List<RefreshToken> RefreshToken { get; set; } = new();
}