using Microsoft.EntityFrameworkCore;
using SP.Core.Interfaces.Repositories;
using SP.Domain.Entities;
using SP.Infrastructure.Data;

namespace SP.Infrastructure.Repositories;

public class SpotRepository : ISpotRepository
{
   private readonly AppDbContext _context;

   public SpotRepository(AppDbContext context)
   {
      _context = context;
   }

   public async Task<Spot?> GetByIdAsync(Guid id)
   {
      return await _context.Spots
         .Include(s => s.User)
         .FirstOrDefaultAsync(s => s.Id == id);
   }

   public async Task<IEnumerable<Spot>> GetAllAsync()
   {
      return await _context.Spots
         .Include(s => s.User)
         .Include(s => s.Tags)
         .OrderByDescending(s => s.CreatedAt)
         .ToListAsync();
   }

   public async Task<IEnumerable<Spot>> GetByUserIdAsync(Guid userId)
   {
      return await _context.Spots
         .Where(s => s.UserId == userId)
         .ToListAsync();
   }

   public async Task AddAsync(Spot spot)
   {
      await _context.Spots.AddAsync(spot);
      await _context.SaveChangesAsync();
   }

   public async Task UpdateAsync(Spot spot)
   {
      _context.Spots.Update(spot);
      await _context.SaveChangesAsync();
   }

   public async Task DeleteAsync(Spot spot)
   {
      _context.Spots.Remove(spot);
      await _context.SaveChangesAsync();
   }

   public async Task<IEnumerable<Spot>> GetFavoriteSpotsByUserIdAsync(Guid userId)
   {
      return await _context.Set<FavoriteSpot>()
         .Where(fs => fs.UserId == userId)
         .Include(fs => fs.Spot) // Jointure EF pour charger l'objet Spot
         .Select(fs => fs.Spot)
         .ToListAsync();   }

   public async Task<bool> ToggleFavoriteSpotAsync(Guid userId, Guid spotId)
   {
      // On cherche si la ligne existe 
      var existing = await _context.Set<FavoriteSpot>()
         .FirstOrDefaultAsync(fs => fs.UserId == userId && fs.SpotsId == spotId);

      if (existing != null)
      {
         // Si elle existe, on la dégage
         _context.Set<FavoriteSpot>().Remove(existing);
         await _context.SaveChangesAsync();
         return false; // false = retiré
      }

      // Si elle n'existe pas, on l'ajoute
      var newFavorite = new FavoriteSpot
      {
         UserId = userId,
         SpotsId = spotId
      };

      _context.Set<FavoriteSpot>().Add(newFavorite);
      await _context.SaveChangesAsync();
      return true; // true = ajouté
   }   
   public async Task<IEnumerable<Tag>> GetTagsByTypeAsync(string type)
   {
      return await _context.Tags
         .Where(t => t.Type.ToLower() == type.ToLower())
         .OrderBy(t => t.Name)
         .ToListAsync();
   }
   public async Task<List<Tag>> GetTagsByIdsAsync(List<Guid> tagIds)
   {
      return await _context.Tags
         .Where(t => tagIds.Contains(t.TagsId))
         .ToListAsync();
   }
   
   
}