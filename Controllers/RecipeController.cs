using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TheKitchen.Services.Interfaces;
using TheKitchen.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace TheKitchen.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly UserManager<IdentityUser> _userManager;

        public RecipeController(IRecipeService recipeService, UserManager<IdentityUser> userManager)
        {
            _recipeService = recipeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string search, string category)
        {
            ViewData["SearchTerm"] = search;
            ViewData["Category"] = category;

            var recipes = await _recipeService.SearchRecipesAsync(search ?? "", category);
            return View(recipes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string Title, 
            string Description, 
            string Instructions, 
            int PrepTime, 
            int CookTime, 
            int Servings, 
            string Difficulty, 
            bool IsPublic = true,
            IFormFile ImageFile = null,
            string VideoUrl = null)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                string imagePath = null;
                
                // Handle image upload
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var imageService = HttpContext.RequestServices.GetService<IImageService>();
                    imagePath = await imageService.SaveImageAsync(ImageFile, "recipes");
                    
                    // Debug: Log the image path
                    Console.WriteLine($"Image saved at: {imagePath}");
                    Console.WriteLine($"Image file name: {ImageFile.FileName}");
                    Console.WriteLine($"Image size: {ImageFile.Length} bytes");
                }

                // Create recipe object
                var recipe = new Recipe
                {
                    Title = Title,
                    Description = Description ?? "",
                    Instructions = Instructions,
                    PrepTime = PrepTime,
                    CookTime = CookTime,
                    Servings = Servings,
                    Difficulty = Difficulty,
                    IsPublic = IsPublic,
                    ImagePath = imagePath,
                    VideoUrl = VideoUrl,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Default ingredients
                var ingredientIds = new List<int> { 3 };
                var quantities = new List<string> { "2 items" };

                await _recipeService.CreateRecipeAsync(recipe, ingredientIds, quantities);
                
                TempData["SuccessMessage"] = $"Recipe '{Title}' shared successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating recipe: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the recipe. Please try again.");
                
                var recipe = new Recipe 
                { 
                    Title = Title,
                    Description = Description,
                    Instructions = Instructions,
                    PrepTime = PrepTime,
                    CookTime = CookTime,
                    Servings = Servings,
                    Difficulty = Difficulty,
                    IsPublic = IsPublic,
                    VideoUrl = VideoUrl
                };
                return View(recipe);
            }
        }

        [Authorize]
        public async Task<IActionResult> MyRecipes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var recipes = await _recipeService.GetUserRecipesAsync(user.Id);
            return View(recipes);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (recipe.UserId != user?.Id)
            {
                return Forbid(); // Users can only edit their own recipes
            }

            return View(recipe);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string Title,
            string Description,
            string Instructions,
            int PrepTime,
            int CookTime,
            int Servings,
            string Difficulty,
            bool IsPublic = true,
            IFormFile ImageFile = null,
            string VideoUrl = null)
        {
            try
            {
                var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
                if (existingRecipe == null)
                {
                    return NotFound();
                }

                var user = await _userManager.GetUserAsync(User);
                if (existingRecipe.UserId != user?.Id)
                {
                    return Forbid();
                }

                // Handle image upload if new image is provided
                string imagePath = existingRecipe.ImagePath;
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var imageService = HttpContext.RequestServices.GetService<IImageService>();
                    
                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(existingRecipe.ImagePath))
                    {
                        imageService.DeleteImage(existingRecipe.ImagePath);
                    }
                    
                    // Save new image
                    imagePath = await imageService.SaveImageAsync(ImageFile, "recipes");
                }

                // Update recipe properties
                existingRecipe.Title = Title;
                existingRecipe.Description = Description ?? "";
                existingRecipe.Instructions = Instructions;
                existingRecipe.PrepTime = PrepTime;
                existingRecipe.CookTime = CookTime;
                existingRecipe.Servings = Servings;
                existingRecipe.Difficulty = Difficulty;
                existingRecipe.IsPublic = IsPublic;
                existingRecipe.ImagePath = imagePath;
                existingRecipe.VideoUrl = VideoUrl;
                existingRecipe.UpdatedAt = DateTime.UtcNow;

                await _recipeService.UpdateRecipeAsync(existingRecipe);
                
                TempData["SuccessMessage"] = $"Recipe '{Title}' updated successfully!";
                return RedirectToAction(nameof(Details), new { id = existingRecipe.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating recipe: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while updating the recipe. Please try again.");
                
                var recipe = new Recipe 
                { 
                    Id = id,
                    Title = Title,
                    Description = Description,
                    Instructions = Instructions,
                    PrepTime = PrepTime,
                    CookTime = CookTime,
                    Servings = Servings,
                    Difficulty = Difficulty,
                    IsPublic = IsPublic,
                    VideoUrl = VideoUrl
                };
                return View(recipe);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(id);
                if (recipe == null)
                {
                    TempData["ErrorMessage"] = "Recipe not found.";
                    return RedirectToAction(nameof(MyRecipes));
                }

                var user = await _userManager.GetUserAsync(User);
                if (recipe.UserId != user?.Id)
                {
                    TempData["ErrorMessage"] = "You can only delete your own recipes.";
                    return RedirectToAction(nameof(MyRecipes));
                }

                // Delete associated image if it exists
                if (!string.IsNullOrEmpty(recipe.ImagePath))
                {
                    var imageService = HttpContext.RequestServices.GetService<IImageService>();
                    imageService.DeleteImage(recipe.ImagePath);
                }

                await _recipeService.DeleteRecipeAsync(id);
                
                TempData["SuccessMessage"] = $"Recipe '{recipe.Title}' deleted successfully!";
                return RedirectToAction(nameof(MyRecipes));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting recipe: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the recipe. Please try again.";
                return RedirectToAction(nameof(MyRecipes));
            }
        }

        private async Task<bool> RecipeExists(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            return recipe != null;
        }

        public async Task<IActionResult> TestImage()
        {
            // Get recent recipes to test image display
            var recipes = await _recipeService.SearchRecipesAsync("");
            ViewBag.RecentRecipes = recipes.Take(5).ToList();
            return View();
        }
    }
}