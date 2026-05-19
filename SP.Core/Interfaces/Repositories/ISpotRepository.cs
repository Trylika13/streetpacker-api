using SP.Domain.Entities;

namespace SP.Core.Interfaces.Repositories;

public interface ISpotRepository
{
    Task<Spot?> GetByIdAsync(Guid id);
    Task<IEnumerable<Spot>> GetAllAsync();
    Task<IEnumerable<Spot>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Spot spot);
    Task UpdateAsync(Spot spot);
    Task DeleteAsync(Spot spot);
}