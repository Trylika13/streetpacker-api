using Microsoft.EntityFrameworkCore;
using SP.Core.Interfaces.Repositories;
using SP.Domain.Entities;
using SP.Infrastructure.Data;

namespace SP.Infrastructure.Repositories;

public class AdRepository : IAdRepository
{
    private readonly AppDbContext _context;
    
    public AdRepository(AppDbContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<Ad>> GetAllAdsAsync()
    {
        return await _context.Ads
            .Include(a => a.User) // Si tu as configuré la relation de navigation dans ton entité
            .ToListAsync();
    }

    public async Task CreateAdAsync(Ad ad)
    {
        await _context.Ads.AddAsync(ad);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAdAsync(Ad ad)
    {
        _context.Ads.Remove(ad);
        await _context.SaveChangesAsync();
    }

    public async Task<Ad?> GetAdByIdAsync(Guid id)
    {
        return await _context.Ads
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AdId == id);
    }

    public async Task UpdateAdAsync(Ad ad)
    {
        _context.Ads.Update(ad);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Ad>> GetAdByUserIdAsync(Guid userId)
    {
        return await _context.Ads
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }
}