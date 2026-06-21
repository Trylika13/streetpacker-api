using System;

namespace SP.Domain.Entities;

public class Ad
{
    public Guid AdId { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float Price { get; set; }
    public string LocationArea { get; set; } = string.Empty;
    public string ContactLink { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    
    public List<Tag> Tags { get; set; } = new List<Tag>();

}