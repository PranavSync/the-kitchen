using Microsoft.EntityFrameworkCore;
using TheKitchen.Models.Data;
using TheKitchen.Models.Entities;
using TheKitchen.Services.Interfaces;

namespace TheKitchen.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;

        public RecipeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetFeaturedRecipesAsync()
        {
            return await _context.Recipes
                .Where(r => r.IsFeatured && r.IsPublic)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Categories)
                .Take(6)
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetRecipesByIngredientsAsync(List<int> ingredientIds)
        {
            if (!ingredientIds.Any())
                return new List<Recipe>();

            return await _context.Recipes
                .Where(r => r.IsPublic)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Categories)
                // Find recipes where all required ingredients are in the fridge
                .Where(r => r.RecipeIngredients.All(ri => ingredientIds.Contains(ri.IngredientId)))
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Recipe>> GetUserRecipesAsync(string userId)
        {
            return await _context.Recipes
                .Where(r => r.UserId == userId)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Categories)
                .ToListAsync();
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe, List<int> ingredientIds, List<string> quantities)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            // Add recipe ingredients
            for (int i = 0; i < ingredientIds.Count; i++)
            {
                var recipeIngredient = new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = ingredientIds[i],
                    Quantity = quantities[i]
                };
                _context.RecipeIngredients.Add(recipeIngredient);
            }

            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<List<Recipe>> SearchRecipesAsync(string searchTerm, string? category = null)
        {
            var query = _context.Recipes
                .Where(r => r.IsPublic)
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Categories)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Title.Contains(searchTerm) ||
                                        r.Description.Contains(searchTerm) ||
                                        r.Instructions.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(r => r.Categories.Any(c => c.Name == category));
            }

            return await query.ToListAsync();
        }
        
                public async Task UpdateRecipeAsync(Recipe recipe)
        {
            _context.Entry(recipe).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRecipeAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
    }
}