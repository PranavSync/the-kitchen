using TheKitchen.Models.Entities;

namespace TheKitchen.Services.Interfaces
{
    public interface IShoppingListService
    {
        Task<ShoppingList> CreateShoppingListAsync(string userId, string name, List<(string name, string quantity, string category)> items);
        Task<ShoppingList> MergeShoppingListsAsync(string userId, List<int> recipeIds, string listName);
        Task<List<ShoppingList>> GetUserShoppingListsAsync(string userId);
        Task<ShoppingList?> GetShoppingListByIdAsync(int id);
    }
}