using SP.Domain.Entities;
using SP.Presentation.Dtos;

namespace SP.Presentation.Mappers;

public static class SpotMapper
{
    public static Spot ToEntity(CreateSpotDto dto, Guid userId)
    {
        return new Spot
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            ImageUrl = dto.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            LastVerifiedAt = DateTime.UtcNow,
        
            Tags = new List<Tag>() 
        };
    }
    
    public static SpotDto ToDto(Spot entity)
    {
        return new SpotDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            ImageUrl = entity.ImageUrl,
            FreshnessScore = (int)entity.FreshnessScore,
            Username = entity.User?.Username ?? "Anonyme", // On récupère juste le pseudo
            CreatedAt = entity.CreatedAt,
            
            Tags = entity.Tags?.Select(t => t.Name).ToList() ?? new List<string>()
        };
    }
    
}

