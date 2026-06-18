namespace SP.Domain.Entities;

public class FavoriteAd
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid AdId { get; set; } 
    public Ad Ad { get; set; }
}