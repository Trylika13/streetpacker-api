using SP.Core.Interfaces.Repositories;
using SP.Core.Interfaces.Services;
using SP.Domain.Entities;

namespace SP.Core.Services;

public class SpotService : ISpotService
{
    private readonly ISpotRepository _spotRepository;
    
    public SpotService(ISpotRepository spotRepository)
    {
        _spotRepository = spotRepository;
    }

    public async Task<Spot?> GetSpotByIdAsync(Guid id)
    {
        return await _spotRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Spot>> GetSpotsByUserIdAsync(Guid userId)
    {
        return await _spotRepository.GetByUserIdAsync(userId);
    }

    public async Task<(bool Success, string Message, Spot? Spot)> CreateSpotAsync(Spot spot)
    {
        var userSpots = await _spotRepository.GetByUserIdAsync(spot.UserId);
        var spotsToday = userSpots.Count(s => s.CreatedAt.Date == DateTime.UtcNow.Date);

        if (spotsToday >= 5)
        {
            return (false, "Limite de 5 spots par jour atteinte. Repose-toi, backpacker !", null);
        }
        spot.FreshnessScore = 100;
        spot.LastVerifiedAt = DateTime.UtcNow;
        
        await _spotRepository.AddAsync(spot);
        
        return (true, "Spot créé avec succès!" , spot);
    }

    public async Task<bool> UpdateSpotAsync(Spot spot)
    {
        await _spotRepository.UpdateAsync(spot);
        
        return true;
    }

    public async Task<bool> DeleteSpotAsync(Guid id, Guid userId)
    {
        var spot = await _spotRepository.GetByIdAsync(id);
    
        if (spot == null || spot.UserId != userId) 
        {
            return false;
        }
    
        await _spotRepository.DeleteAsync(spot);
        return true;
    }

    public async Task<IEnumerable<Spot>> GetFavoriteSpotsByUserIdAsync(Guid userId)
    {
        return await _spotRepository.GetFavoriteSpotsByUserIdAsync(userId);    }

    public async Task<(bool IsFavorite, string Message)> ToggleFavoriteSpotAsync(Guid userId, Guid spotId)
    {
        // On laisse le repo faire le taf en DB
        var isAdded = await _spotRepository.ToggleFavoriteSpotAsync(userId, spotId);

        if (isAdded)
        {
            return (true, "Spot ajouté aux favoris.");
        }
    
        return (false, "Spot retiré des favoris.");    }

    public async Task<IEnumerable<Spot>> GetAllSpotsAsync()
    {
        return await _spotRepository.GetAllAsync();
    }
    
    public async Task<Spot> VoteSpotAsync(Guid spotId, bool isUpvote)
    {
        // 1. Récupérer l'entité
        var spot = await _spotRepository.GetByIdAsync(spotId);
        if (spot == null) throw new KeyNotFoundException("Spot introuvable");

        // 2. Calcul des points (+25 / -25)
        int points = isUpvote ? 25 : -25;
        int newScore = spot.FreshnessScore + points;

        // 3. Sécurité des bornes [0 - 100]
        spot.FreshnessScore = Math.Clamp(newScore, 0, 100);

        // 4. Persistance
        await _spotRepository.UpdateAsync(spot);

        // 5. On renvoie l'entité modifiée au contrôleur
        return spot;
    }
}   