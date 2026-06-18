using SP.Domain.Entities;

namespace SP.Core.Interfaces.Repositories;

public interface IAdRepository
{
    Task<IEnumerable<Ad>> GetAllAdsAsync();
    Task CreateAdAsync(Ad ad);
    Task DeleteAdAsync (Ad ad);
    Task<Ad?> GetAdByIdAsync(Guid id);
    Task UpdateAdAsync(Ad ad);
    Task<IEnumerable<Ad>>GetAdByUserIdAsync(Guid userId);
    
    Task<IEnumerable<Ad>> GetFavoriteAdsByUserIdAsync(Guid userId);
    Task<bool> ToggleFavoriteAdAsync(Guid userId, Guid adId);
    
    
}