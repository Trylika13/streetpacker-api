namespace SP.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; } // Pour pouvoir "tuer" une session
    public bool IsActive => Revoked == null && DateTime.UtcNow < Expires;
    
    
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
}
