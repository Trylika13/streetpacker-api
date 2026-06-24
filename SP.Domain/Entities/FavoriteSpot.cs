namespace SP.Domain.Entities;

public class FavoriteSpot
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid SpotsId { get; set; } 
    public Spot Spot { get; set; }
}