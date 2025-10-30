using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheKitchen.Models.Entities;

namespace TheKitchen.Models.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<FridgeItem> FridgeItems { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure RecipeIngredient many-to-many relationship
            builder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });

            builder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            builder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);

            // Seed initial data
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Vegetarian" },
                new Category { Id = 2, Name = "Non-Vegetarian" },
                new Category { Id = 3, Name = "Vegan" },
                new Category { Id = 4, Name = "Gluten-Free" },
                new Category { Id = 5, Name = "Desserts" },
                new Category { Id = 6, Name = "Quick Meals" }
            );

            builder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Flour", Category = "Baking" },
                new Ingredient { Id = 2, Name = "Sugar", Category = "Baking" },
                new Ingredient { Id = 3, Name = "Eggs", Category = "Dairy" },
                new Ingredient { Id = 4, Name = "Milk", Category = "Dairy" },
                new Ingredient { Id = 5, Name = "Tomatoes", Category = "Produce" },
                new Ingredient { Id = 6, Name = "Onions", Category = "Produce" },
                new Ingredient { Id = 7, Name = "Garlic", Category = "Produce" },
                new Ingredient { Id = 8, Name = "Chicken", Category = "Meat" },
                new Ingredient { Id = 9, Name = "Rice", Category = "Grains" },
                new Ingredient { Id = 10, Name = "Olive Oil", Category = "Condiments" }
            );
        }
    }
}