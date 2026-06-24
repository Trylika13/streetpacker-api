using SP.Core.Interfaces.Repositories;
using SP.Core.Interfaces.Services;
using SP.Domain.Entities;

namespace SP.Core.Services;

public class AdService : IAdService
{
    private readonly IAdRepository _adRepository;

    public AdService(IAdRepository adRepository)
    {
        _adRepository = adRepository;
    }

    public async Task<IEnumerable<Ad>> GetAllAdsAsync(int page, int pageSize)
    {
        return await _adRepository.GetAllAdsAsync(page, pageSize);
    }

    public async Task<(bool success, string errorMessage, Ad? ad)> CreateAdAsync(Ad ad, List<Guid> tagIds)
    {
        if (tagIds != null && tagIds.Any())
        {
            var tagsFromDb = await _adRepository.GetTagsByIdsAsync(tagIds);
        
            foreach (var tag in tagsFromDb)
            {
                ad.Tags.Add(tag); // EF Core va intercepter ça pour remplir "Ad_Tags" tout seul
            }
        }

        await _adRepository.CreateAdAsync(ad);
    
        return (true, "", ad);
    }

    public async Task<bool> DeleteAdAsync(Guid userId, Guid id)
    {
        var ad = await _adRepository.GetAdByIdAsync(id);

        if (ad == null || ad.UserId != userId)
        {
            return false;
        }
        
        await _adRepository.DeleteAdAsync(ad);
        return true;
    }

    public async Task<Ad?> GetAdByIdAsync(Guid id)
    {
        return await _adRepository.GetAdByIdAsync(id);
    }

    public async Task<bool> UpdateAdAsync(Ad ad)
    {
        await _adRepository.UpdateAdAsync(ad);
        return true;
    }

    public async Task<IEnumerable<Ad>> GetAdsByUserIdAsync(Guid userId)
    {
        return await _adRepository.GetAdByUserIdAsync(userId);
    }
    
    public async Task<IEnumerable<Ad>> GetFavoriteAdsByUserIdAsync(Guid userId)
    {
        return await _adRepository.GetFavoriteAdsByUserIdAsync(userId);
    }

    public async Task<(bool IsFavorite, string Message)> ToggleFavoriteAdAsync(Guid userId, Guid adId)
    {
        var isAdded = await _adRepository.ToggleFavoriteAdAsync(userId, adId);

        if (isAdded)
        {
            return (true, "Annonce ajoutée aux favoris.");
        }

        return (false, "Annonce retirée des favoris.");
    }
    public async Task<IEnumerable<Tag>> GetAdTagsAsync()
    {
        return await _adRepository.GetTagsByTypeAsync("ad");
    }
}    