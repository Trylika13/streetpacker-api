using SP.Domain.Entities;
using SP.Presentation.Dtos;

namespace SP.Presentation.Mappers;

public static class AdMapper
{
    public static AdDto ToDto(Ad entity)
    {
        return new AdDto
        {
            AdId = entity.AdId,
            Title = entity.Title,
            Description = entity.Description,
            Price = entity.Price,
            ContactLink = entity.ContactLink,
            IsActive = entity.IsActive,
            UserId = entity.UserId,
            LocationArea = entity.LocationArea,
            CreatedAt = entity.created_at,
            
            Tags = entity.Tags?.Select(t => t.Name).ToList() ?? new List<string>()
        };
    }

    public static Ad ToEntity(CreateAdDto dto, Guid userId, string contactLink)
    {
        return new Ad
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            ContactLink = contactLink, 
            UserId = userId,
            LocationArea = dto.LocationArea,
            
            Tags = new List<Tag>()
        };
    }
}