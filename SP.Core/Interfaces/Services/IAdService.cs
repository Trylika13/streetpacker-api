using SP.Domain.Entities;

namespace SP.Core.Interfaces.Services;

public interface IAdService
{
    Task<IEnumerable<Ad>> GetAllAdsAsync();
    Task<(bool success, string errorMessage, Ad? ad)> CreateAdAsync(Ad ad, List<Guid> tagIds);    Task<bool> DeleteAdAsync(Guid userId, Guid id);
    Task<Ad?> GetAdByIdAsync(Guid id);
    Task<bool> UpdateAdAsync(Ad ad);
    Task<IEnumerable<Ad>> GetAdsByUserIdAsync(Guid userId);
    
    Task<IEnumerable<Ad>> GetFavoriteAdsByUserIdAsync(Guid userId);
    Task<(bool IsFavorite, string Message)> ToggleFavoriteAdAsync(Guid userId, Guid adId);
    Task<IEnumerable<Tag>> GetAdTagsAsync();
}