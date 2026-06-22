namespace SP.Presentation.Dtos;

public class UpdateAdDto
{
    public string Title { get; set; } =  string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public string LocationArea { get; set; } = string.Empty;
    
    public float Price { get; set; }
    
    public string ContactLink { get; set; } = string.Empty;
    
    public string? ImageUrl { get; set; }
    
    public bool IsActive { get; set; }
}