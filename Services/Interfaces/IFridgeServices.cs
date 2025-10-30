using TheKitchen.Models.Entities;

namespace TheKitchen.Services.Interfaces
{
    public interface IFridgeService
    {
        Task<List<FridgeItem>> GetUserFridgeAsync(string userId);
        Task AddToFridgeAsync(string userId, int ingredientId, string quantity);
        Task RemoveFromFridgeAsync(int fridgeItemId);
        Task<bool> UserHasFridgeItemsAsync(string userId);
        Task<int> GetFridgeItemCountAsync(string userId);
    }
}