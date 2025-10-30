using Microsoft.EntityFrameworkCore;
using TheKitchen.Models.Data;
using TheKitchen.Models.Entities;
using TheKitchen.Services.Interfaces;

namespace TheKitchen.Services
{
    public class FridgeService : IFridgeService
    {
        private readonly ApplicationDbContext _context;

        public FridgeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FridgeItem>> GetUserFridgeAsync(string userId)
        {
            return await _context.FridgeItems
                .Where(f => f.UserId == userId)
                .Include(f => f.Ingredient)
                .ToListAsync();
        }

        public async Task AddToFridgeAsync(string userId, int ingredientId, string quantity)
        {
            var existingItem = await _context.FridgeItems
                .FirstOrDefaultAsync(f => f.UserId == userId && f.IngredientId == ingredientId);

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                existingItem.AddedDate = DateTime.UtcNow;
            }
            else
            {
                var fridgeItem = new FridgeItem
                {
                    UserId = userId,
                    IngredientId = ingredientId,
                    Quantity = quantity,
                    AddedDate = DateTime.UtcNow
                };
                _context.FridgeItems.Add(fridgeItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromFridgeAsync(int fridgeItemId)
        {
            var fridgeItem = await _context.FridgeItems.FindAsync(fridgeItemId);
            if (fridgeItem != null)
            {
                _context.FridgeItems.Remove(fridgeItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UserHasFridgeItemsAsync(string userId)
        {
            return await _context.FridgeItems.AnyAsync(f => f.UserId == userId);
        }

        public async Task<int> GetFridgeItemCountAsync(string userId)
        {
            return await _context.FridgeItems.CountAsync(f => f.UserId == userId);
        }
    }
}