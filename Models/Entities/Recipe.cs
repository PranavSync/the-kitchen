using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheKitchen.Models.Entities
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Instructions { get; set; } = string.Empty;

        public int PrepTime { get; set; } // in minutes
        public int CookTime { get; set; } // in minutes
        public int Servings { get; set; }

        [StringLength(20)]
        public string Difficulty { get; set; } = "Easy"; // Easy, Medium, Hard

        public string? ImagePath { get; set; }
        public string? VideoUrl { get; set; }

        public bool IsPublic { get; set; } = true;
        public bool IsFeatured { get; set; } = false;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Navigation properties
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
    }

    public class Ingredient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // Produce, Dairy, Spices, etc.

        // Navigation properties
        public List<RecipeIngredient> RecipeIngredients { get; set; } = new();
    }

    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Quantity { get; set; } = string.Empty; // "2 cups", "1 tbsp", etc.
    }

    public class FridgeItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; } = null!;

        [StringLength(50)]
        public string Quantity { get; set; } = string.Empty;

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
    }

    public class ShoppingList
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;

        public List<ShoppingListItem> Items { get; set; } = new();
    }

    public class ShoppingListItem
    {
        [Key]
        public int Id { get; set; }

        public int ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Quantity { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public bool IsPurchased { get; set; } = false;
    }

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public List<Recipe> Recipes { get; set; } = new();
    }
}