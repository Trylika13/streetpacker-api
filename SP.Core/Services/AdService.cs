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

    public async Task<IEnumerable<Ad>> GetAllAdsAsync()
    {
        return await _adRepository.GetAllAdsAsync();
    }

    public async Task<(bool success, string errorMessage, Ad? ad)> CreateAdAsync(Ad ad)
    {
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
}    