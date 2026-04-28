namespace SP.Presentation.Dtos;

public class SpotDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public int FreshnessScore { get; set; }
    public string Username { get; set; } = string.Empty; // Juste le nom, pas tout l'objet !
    public DateTime CreatedAt { get; set; }
}