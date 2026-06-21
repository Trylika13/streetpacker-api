namespace SP.Domain.Entities;

public class Tag
{
    public Guid TagsId { get; set; }
    public DateTime created_at { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    
    public List<Spot> Spots { get; set; } = new List<Spot>();
    public List<Ad> Ads { get; set; } = new List<Ad>();

    }