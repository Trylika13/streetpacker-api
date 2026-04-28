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

    public async Task<bool> UpdateSpotAsync(Spot updatedSpot,  Guid userId)
    {
        var existingSpot = await _spotRepository.GetByIdAsync(updatedSpot.Id);
        
        if (existingSpot == null)
        {
            Console.WriteLine($"[DEBUG] Spot {updatedSpot.Id} INTROUVABLE en base.");
            return false;
        }

        Console.WriteLine($"[DEBUG] ID du Token : {userId}");
        Console.WriteLine($"[DEBUG] ID du Proprio en base : {existingSpot.UserId}");

        if (existingSpot.UserId != userId) 
        {
            Console.WriteLine("[DEBUG] MATCH ÉCHOUÉ : C'est pas le même User.");
            return false;
        }
        
        existingSpot.Title = updatedSpot.Title;
        existingSpot.Description = updatedSpot.Description;
        existingSpot.FreshnessScore = updatedSpot.FreshnessScore;
        existingSpot.Latitude = updatedSpot.Latitude;
        existingSpot.Longitude = updatedSpot.Longitude;
        existingSpot.ImageUrl = updatedSpot.ImageUrl;
        existingSpot.LastVerifiedAt = DateTime.UtcNow;
        
        await _spotRepository.UpdateAsync(existingSpot);
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

    public async Task<IEnumerable<Spot>> GetAllSpotsAsync()
    {
        return await _spotRepository.GetAllAsync();
    }
}   