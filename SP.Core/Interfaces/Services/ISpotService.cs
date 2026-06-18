using SP.Domain.Entities;

namespace SP.Core.Interfaces.Services;

public interface ISpotService
{
    Task<Spot?> GetSpotByIdAsync(Guid id);
    Task<IEnumerable<Spot>> GetAllSpotsAsync();
    Task<IEnumerable<Spot>> GetSpotsByUserIdAsync(Guid userId);
    Task<(bool Success, string Message, Spot? Spot)> CreateSpotAsync(Spot spot);
    Task<bool> UpdateSpotAsync(Spot spot);
    Task<bool> DeleteSpotAsync(Guid id, Guid userId);
    Task<IEnumerable<Spot>> GetFavoriteSpotsByUserIdAsync(Guid userId);
    Task<(bool IsFavorite, string Message)> ToggleFavoriteSpotAsync(Guid userId, Guid spotId);
}