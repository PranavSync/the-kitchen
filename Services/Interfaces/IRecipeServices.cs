using TheKitchen.Models.Entities;

namespace TheKitchen.Services.Interfaces
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetFeaturedRecipesAsync();
        Task<List<Recipe>> GetRecipesByIngredientsAsync(List<int> ingredientIds);
        Task<Recipe?> GetRecipeByIdAsync(int id);
        Task<List<Recipe>> GetUserRecipesAsync(string userId);
        Task<Recipe> CreateRecipeAsync(Recipe recipe, List<int> ingredientIds, List<string> quantities);
        Task<List<Recipe>> SearchRecipesAsync(string searchTerm, string? category = null);
        Task UpdateRecipeAsync(Recipe recipe);
        Task DeleteRecipeAsync(int id);
    }
}