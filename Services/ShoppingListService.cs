using Microsoft.EntityFrameworkCore;
using TheKitchen.Models.Data;
using TheKitchen.Models.Entities;
using TheKitchen.Services.Interfaces;

namespace TheKitchen.Services
{
    public class ShoppingListService : IShoppingListService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingListService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingList> CreateShoppingListAsync(string userId, string name, List<(string name, string quantity, string category)> items)
        {
            var shoppingList = new ShoppingList
            {
                UserId = userId,
                Name = name,
                CreatedDate = DateTime.UtcNow
            };

            _context.ShoppingLists.Add(shoppingList);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var shoppingListItem = new ShoppingListItem
                {
                    ShoppingListId = shoppingList.Id,
                    Name = item.name,
                    Quantity = item.quantity,
                    Category = item.category,
                    IsPurchased = false
                };
                _context.ShoppingListItems.Add(shoppingListItem);
            }

            await _context.SaveChangesAsync();
            return shoppingList;
        }

        public async Task<ShoppingList> MergeShoppingListsAsync(string userId, List<int> recipeIds, string listName)
        {
            var recipes = await _context.Recipes
                .Where(r => recipeIds.Contains(r.Id))
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();

            var mergedItems = new Dictionary<string, (string quantity, string category)>();

            foreach (var recipe in recipes)
            {
                foreach (var recipeIngredient in recipe.RecipeIngredients)
                {
                    var ingredientName = recipeIngredient.Ingredient.Name;
                    if (mergedItems.ContainsKey(ingredientName))
                    {
                        // For simplicity, we'll just keep the first quantity encountered
                        // In a real app, you might want to sum quantities
                        continue;
                    }
                    else
                    {
                        mergedItems[ingredientName] = (recipeIngredient.Quantity, recipeIngredient.Ingredient.Category);
                    }
                }
            }

            var shoppingListItems = mergedItems.Select(item => 
                (item.Key, item.Value.quantity, item.Value.category)).ToList();

            return await CreateShoppingListAsync(userId, listName, shoppingListItems);
        }

        public async Task<List<ShoppingList>> GetUserShoppingListsAsync(string userId)
        {
            return await _context.ShoppingLists
                .Where(sl => sl.UserId == userId)
                .Include(sl => sl.Items)
                .OrderByDescending(sl => sl.CreatedDate)
                .ToListAsync();
        }

        public async Task<ShoppingList?> GetShoppingListByIdAsync(int id)
        {
            return await _context.ShoppingLists
                .Include(sl => sl.Items)
                .FirstOrDefaultAsync(sl => sl.Id == id);
        }
    }
}