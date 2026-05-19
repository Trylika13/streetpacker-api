using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Domain.Entities;

public class Spot
{
    [Key]
    [Column("SpotsId")]
    public Guid Id { get; set; } = Guid.NewGuid(); 

    [Column("User_Id")]
    public Guid UserId { get; set; } 
    
    public User User { get; set; } = null!;
    
    public required string Title { get; set; } = string.Empty; 
    
    public string Description { get; set; } = string.Empty; 
    
    public required double Latitude { get; set; } 
    
    public required double Longitude { get; set; } 
    
    public string? ImageUrl { get; set; } 

    public int FreshnessScore { get; set; } = 100; 
    
    public DateTime LastVerifiedAt { get; set; } = DateTime.UtcNow; 
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}