using SP.Domain.Entities;

namespace SP.Core.Interfaces.Services;

public interface ISpotService
{
    Task<Spot?> GetSpotByIdAsync(Guid id);
    Task<IEnumerable<Spot>> GetAllSpotsAsync();
    
    Task<(bool Success, string Message, Spot? Spot)> CreateSpotAsync(Spot spot);
    
    Task<bool> UpdateSpotAsync(Spot spot, Guid userId);
    Task<bool> DeleteSpotAsync(Guid id, Guid userId);
}