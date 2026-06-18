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
    
    public async Task<IEnumerable<Ad>> GetFavoriteAdsByUserIdAsync(Guid userId)
    {
        return await _context.Set<FavoriteAd>()
            .Where(fa => fa.UserId == userId)
            .Include(fa => fa.Ad) // Jointure pour charger l'objet Annonce
            .Select(fa => fa.Ad)
            .ToListAsync();
    }

    public async Task<bool> ToggleFavoriteAdAsync(Guid userId, Guid adId)
    {
        // On cherche si l'annonce est déjà en favori
        var existing = await _context.Set<FavoriteAd>()
            .FirstOrDefaultAsync(fa => fa.UserId == userId && fa.AdId == adId);

        if (existing != null)
        {
            _context.Set<FavoriteAd>().Remove(existing);
            await _context.SaveChangesAsync();
            return false; // Retiré
        }

        var newFavorite = new FavoriteAd
        {
            UserId = userId,
            AdId = adId
        };

        _context.Set<FavoriteAd>().Add(newFavorite);
        await _context.SaveChangesAsync();
        return true; // Ajouté
    }
}