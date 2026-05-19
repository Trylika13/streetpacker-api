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
}