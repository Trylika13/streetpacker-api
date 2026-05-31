namespace SP.Domain.Entities;

public class Tag
{
    public Guid TagsId { get; set; }
    public DateTime created_at { get; set; }
    public string Name { get; set; } = string.Empty;

    }