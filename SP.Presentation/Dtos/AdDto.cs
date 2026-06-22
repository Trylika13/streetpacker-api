using SP.Domain.Entities;

namespace SP.Presentation.Dtos;

public class AdDto
{
    public Guid AdId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Price { get; set; }
    public string LocationArea { get; set; } = string.Empty;
    public string ContactLink { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
    
    public List<string> Tags { get; set; } = new List<string>();
    
    public UserPublicProfileDto? User { get; set; }

}
    