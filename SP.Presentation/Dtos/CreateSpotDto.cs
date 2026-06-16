namespace SP.Presentation.Dtos;

public class CreateSpotDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
    
    public List<Guid> TagIds { get; set; } = new List<Guid>();
}