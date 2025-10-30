using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TheKitchen.Services.Interfaces;
using TheKitchen.Models.Entities;

namespace TheKitchen.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IFridgeService _fridgeService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(IRecipeService recipeService, IFridgeService fridgeService, UserManager<IdentityUser> userManager)
        {
            _recipeService = recipeService;
            _fridgeService = fridgeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Remove the fridge setup redirect for now
            // Users can manually go to fridge setup if they want
            
            try
            {
                var featuredRecipes = await _recipeService.GetFeaturedRecipesAsync();
                return View(featuredRecipes);
            }
            catch (Exception)
            {
                // If there's an error loading recipes, return empty list
                return View(new List<Recipe>());
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}