namespace TheKitchen.Models.ViewModels
{
    public class RecipeViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public int Servings { get; set; }
        public string Difficulty { get; set; } = "Easy";
        public string? ImagePath { get; set; }
        public string? VideoUrl { get; set; }
        public List<IngredientViewModel> Ingredients { get; set; } = new();
        public List<string> Categories { get; set; } = new();
    }

    public class IngredientViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Quantity { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}