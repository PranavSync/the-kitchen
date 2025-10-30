using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheKitchen.Services.Interfaces;
using TheKitchen.Models.Entities;
using TheKitchen.Models.Data;
using Microsoft.AspNetCore.Authorization;

namespace TheKitchen.Controllers
{
    [Authorize]
    public class ShoppingListController : Controller
    {
        private readonly IShoppingListService _shoppingListService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IRecipeService _recipeService;
        private readonly ApplicationDbContext _context;

        public ShoppingListController(IShoppingListService shoppingListService, 
                                    UserManager<IdentityUser> userManager, 
                                    IRecipeService recipeService,
                                    ApplicationDbContext context)
        {
            _shoppingListService = shoppingListService;
            _userManager = userManager;
            _recipeService = recipeService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var shoppingLists = await _shoppingListService.GetUserShoppingListsAsync(user.Id);
            return View(shoppingLists);
        }

        public async Task<IActionResult> Generate(int recipeId)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);
            if (recipe == null) return NotFound();

            return View(recipe);
        }

        [HttpPost]
        public async Task<IActionResult> CreateForRecipe(int recipeId, string listName)
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user?.Id ?? "demo-user";

            var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);
            if (recipe == null) return NotFound();

            var items = recipe.RecipeIngredients.Select(ri => 
                (ri.Ingredient.Name, ri.Quantity, ri.Ingredient.Category)).ToList();

            await _shoppingListService.CreateShoppingListAsync(userId, listName, items);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Merge()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MergeLists(List<int> recipeIds, string listName)
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user?.Id ?? "demo-user";

            await _shoppingListService.MergeShoppingListsAsync(userId, recipeIds, listName);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var shoppingList = await _shoppingListService.GetShoppingListByIdAsync(id);
            if (shoppingList == null) return NotFound();

            return View(shoppingList);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleItemPurchased([FromBody] ToggleItemRequest request)
        {
            try
            {
                var item = await _context.ShoppingListItems.FindAsync(request.ItemId);
                if (item != null)
                {
                    item.IsPurchased = request.IsPurchased;
                    await _context.SaveChangesAsync();
                    return Ok(new { success = true });
                }
                return NotFound(new { success = false, error = "Item not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllPurchased([FromBody] MarkAllRequest request)
        {
            try
            {
                var items = await _context.ShoppingListItems
                    .Where(i => i.ShoppingListId == request.ListId)
                    .ToListAsync();

                foreach (var item in items)
                {
                    item.IsPurchased = true;
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllUnpurchased([FromBody] MarkAllRequest request)
        {
            try
            {
                var items = await _context.ShoppingListItems
                    .Where(i => i.ShoppingListId == request.ListId)
                    .ToListAsync();

                foreach (var item in items)
                {
                    item.IsPurchased = false;
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomItem([FromBody] AddCustomItemRequest request)
        {
            try
            {
                var item = new ShoppingListItem
                {
                    ShoppingListId = request.ListId,
                    Name = request.Name,
                    Quantity = request.Quantity,
                    Category = request.Category,
                    IsPurchased = false
                };

                _context.ShoppingListItems.Add(item);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }

    // Request models
    public class ToggleItemRequest
    {
        public int ItemId { get; set; }
        public bool IsPurchased { get; set; }
    }

    public class MarkAllRequest
    {
        public int ListId { get; set; }
    }

    public class AddCustomItemRequest
    {
        public int ListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}