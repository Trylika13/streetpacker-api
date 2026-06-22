namespace SP.Presentation.Dtos;

public class CreateAdDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Price { get; set; }
    public string LocationArea { get; set; } = string.Empty;
    public string ContactLink { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    
    public List<Guid> TagIds { get; set; } = new List<Guid>();
    
}