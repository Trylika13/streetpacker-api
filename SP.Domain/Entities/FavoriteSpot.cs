namespace SP.Domain.Entities;

public class FavoriteSpot
{
    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid SpotsId { get; set; } // Ou int selon le type de ton ID de spot
    public Spot Spot { get; set; }
}