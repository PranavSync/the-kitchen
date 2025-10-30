using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TheKitchen.Services.Interfaces;
using TheKitchen.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace TheKitchen.Controllers
{
    [Authorize]
    public class FridgeController : Controller
    {
        private readonly IFridgeService _fridgeService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRecipeService _recipeService;

        public FridgeController(IFridgeService fridgeService, UserManager<IdentityUser> userManager, IRecipeService recipeService)
        {
            _fridgeService = fridgeService;
            _userManager = userManager;
            _recipeService = recipeService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge(); // Redirect to login if not authenticated

            var fridgeItems = await _fridgeService.GetUserFridgeAsync(user.Id);
            return View(fridgeItems);
        }

        public async Task<IActionResult> SetupFridge()
        {
            // Temporary: Use a default user ID for demo
            var user = await _userManager.GetUserAsync(User);
            string userId = user?.Id ?? "demo-user";

            var hasItems = await _fridgeService.UserHasFridgeItemsAsync(userId);
            if (hasItems)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToFridge([FromBody] AddToFridgeRequest request)
        {
            try
            {
                // Temporary: Use a default user ID for demo
                var user = await _userManager.GetUserAsync(User);
                string userId = user?.Id ?? "demo-user";

                await _fridgeService.AddToFridgeAsync(userId, request.IngredientId, request.Quantity);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFridge([FromBody] RemoveFromFridgeRequest request)
        {
            try
            {
                await _fridgeService.RemoveFromFridgeAsync(request.FridgeItemId);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        public async Task<IActionResult> CookNow()
        {
            // Temporary: Use a default user ID for demo
            var user = await _userManager.GetUserAsync(User);
            string userId = user?.Id ?? "demo-user";

            var fridgeItems = await _fridgeService.GetUserFridgeAsync(userId);
            var ingredientIds = fridgeItems.Select(f => f.IngredientId).ToList();

            var availableRecipes = await _recipeService.GetRecipesByIngredientsAsync(ingredientIds);
            return View(availableRecipes);
        }

        [HttpPost]
        public async Task<IActionResult> SetupFridge(List<int> ingredientIds, List<string> quantities)
        {
            // Temporary: Use a default user ID for demo
            var user = await _userManager.GetUserAsync(User);
            string userId = user?.Id ?? "demo-user";

            if (ingredientIds != null && ingredientIds.Count >= 3)
            {
                for (int i = 0; i < ingredientIds.Count; i++)
                {
                    await _fridgeService.AddToFridgeAsync(userId, ingredientIds[i], quantities[i]);
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Please add at least 3 ingredients");
            return View();
        }
    }

    // Add these request models for the JSON endpoints
    public class AddToFridgeRequest
    {
        public int IngredientId { get; set; }
        public string Quantity { get; set; } = string.Empty;
    }

    public class RemoveFromFridgeRequest
    {
        public int FridgeItemId { get; set; }
    }
}