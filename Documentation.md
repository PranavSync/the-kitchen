# The Kitchen - Technical Documentation

## 📋 Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Database Design](#database-design)
4. [Authentication System](#authentication-system)
5. [CRUD Operations](#crud-operations)
6. [Services & Business Logic](#services--business-logic)
7. [Security Features](#security-features)
8. [API Endpoints](#api-endpoints)
9. [File Structure](#file-structure)
10. [Configuration](#configuration)

---

## 🏗️ Project Overview

**The Kitchen** is a full-stack ASP.NET Core MVC web application designed as a comprehensive recipe management system. It demonstrates modern web development practices including user authentication, database integration, and complete CRUD operations.

### Academic Requirements Fulfilled
- ✅ **User Authentication & Authorization**
- ✅ **Database Integration with MySQL**
- ✅ **Complete CRUD Operations**
- ✅ **RESTful API Design**
- ✅ **Frontend-Backend Integration**

---

## 🏛️ Architecture

### MVC Pattern Implementation
The application follows the Model-View-Controller architecture pattern:

```csharp
// Controller (Handles HTTP requests)
public class RecipeController : Controller
{
    private readonly IRecipeService _recipeService;
    
    public async Task<IActionResult> Index()
    {
        var recipes = await _recipeService.GetFeaturedRecipesAsync();
        return View(recipes);
    }
}

// Model (Data structure)
public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    // ... other properties
}

// View (Razor template)
@model List<Recipe>
@foreach(var recipe in Model)
{
    <div class="recipe-card">@recipe.Title</div>
}
```

### Dependency Injection Setup
```csharp
// Program.cs
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IFridgeService, FridgeService>();
builder.Services.AddScoped<IShoppingListService, ShoppingListService>();
builder.Services.AddScoped<IImageService, ImageService>();
```

---

## 🗃️ Database Design

### Entity Relationship Diagram
```
AspNetUsers (1) ──────── (Many) Recipes (Many) ──────── (Many) Ingredients
     │                           │                           │
     │                           │                           │
(Many) FridgeItems         (Many) RecipeIngredients     (Many) Categories
     │                           │                           │
     │                           │                           │
(Many) ShoppingLists       (Many) ShoppingListItems     (Many) RecipeCategories
```

### Core Tables Structure

#### Users & Authentication (ASP.NET Identity)
```sql
CREATE TABLE `AspNetUsers` (
    `Id` VARCHAR(255) PRIMARY KEY,
    `UserName` VARCHAR(256),
    `Email` VARCHAR(256),
    `PasswordHash` LONGTEXT,
    `SecurityStamp` LONGTEXT,
    -- ... other identity fields
);
```

#### Recipes Table
```sql
CREATE TABLE `Recipes` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Title` VARCHAR(100) NOT NULL,
    `Description` VARCHAR(500),
    `Instructions` TEXT NOT NULL,
    `PrepTime` INT NOT NULL,
    `CookTime` INT NOT NULL,
    `Servings` INT NOT NULL,
    `Difficulty` VARCHAR(20) DEFAULT 'Easy',
    `ImagePath` VARCHAR(500),
    `VideoUrl` VARCHAR(500),
    `IsPublic` BOOLEAN DEFAULT TRUE,
    `IsFeatured` BOOLEAN DEFAULT FALSE,
    `UserId` VARCHAR(255) NOT NULL,
    `CreatedAt` DATETIME NOT NULL,
    `UpdatedAt` DATETIME NOT NULL,
    FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers`(`Id`) ON DELETE CASCADE
);
```

#### Many-to-Many Relationships
```sql
-- Recipes to Ingredients (Junction Table)
CREATE TABLE `RecipeIngredients` (
    `RecipeId` INT NOT NULL,
    `IngredientId` INT NOT NULL,
    `Quantity` VARCHAR(50) NOT NULL,
    PRIMARY KEY (`RecipeId`, `IngredientId`),
    FOREIGN KEY (`RecipeId`) REFERENCES `Recipes`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`IngredientId`) REFERENCES `Ingredients`(`Id`) ON DELETE CASCADE
);

-- Recipes to Categories (Junction Table)  
CREATE TABLE `RecipeCategories` (
    `RecipesId` INT NOT NULL,
    `CategoriesId` INT NOT NULL,
    PRIMARY KEY (`RecipesId`, `CategoriesId`),
    FOREIGN KEY (`RecipesId`) REFERENCES `Recipes`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`CategoriesId`) REFERENCES `Categories`(`Id`) ON DELETE CASCADE
);
```

### Entity Framework Context
```csharp
public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<FridgeItem> FridgeItems { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure many-to-many relationships
        builder.Entity<RecipeIngredient>()
            .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            
        builder.Entity<Recipe>()
            .HasMany(r => r.Categories)
            .WithMany(c => c.Recipes)
            .UsingEntity(j => j.ToTable("RecipeCategories"));
    }
}
```

---

## 🔐 Authentication System

### ASP.NET Core Identity Implementation
```csharp
// Program.cs - Identity Configuration
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();
```

### Authorization Attributes
```csharp
// Controller-level authorization
[Authorize]
public class FridgeController : Controller
{
    // All actions require authentication
}

// Action-level authorization  
public class RecipeController : Controller
{
    [Authorize]
    public IActionResult Create()
    {
        // Only authenticated users can access
    }
    
    public async Task<IActionResult> Index()
    {
        // Public access allowed
    }
}
```

### User Ownership Validation
```csharp
public async Task<IActionResult> Edit(int id)
{
    var recipe = await _recipeService.GetRecipeByIdAsync(id);
    var user = await _userManager.GetUserAsync(User);
    
    // Ensure users can only edit their own recipes
    if (recipe.UserId != user?.Id)
    {
        return Forbid();
    }
    
    return View(recipe);
}
```

---

## 📝 CRUD Operations

### Create Operation
```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Recipe recipe, List<int> ingredientIds, List<string> quantities)
{
    if (ModelState.IsValid)
    {
        var user = await _userManager.GetUserAsync(User);
        recipe.UserId = user.Id;
        
        await _recipeService.CreateRecipeAsync(recipe, ingredientIds, quantities);
        return RedirectToAction(nameof(Index));
    }
    return View(recipe);
}
```

### Read Operations
```csharp
// Get all recipes
public async Task<IActionResult> Index(string search, string category)
{
    var recipes = await _recipeService.SearchRecipesAsync(search ?? "", category);
    return View(recipes);
}

// Get single recipe
public async Task<IActionResult> Details(int id)
{
    var recipe = await _recipeService.GetRecipeByIdAsync(id);
    if (recipe == null) return NotFound();
    return View(recipe);
}

// Get user's recipes
[Authorize]
public async Task<IActionResult> MyRecipes()
{
    var user = await _userManager.GetUserAsync(User);
    var recipes = await _recipeService.GetUserRecipesAsync(user.Id);
    return View(recipes);
}
```

### Update Operation
```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Recipe recipe)
{
    if (id != recipe.Id) return NotFound();
    
    var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
    var user = await _userManager.GetUserAsync(User);
    
    // Authorization check
    if (existingRecipe.UserId != user?.Id) return Forbid();
    
    if (ModelState.IsValid)
    {
        existingRecipe.Title = recipe.Title;
        existingRecipe.Instructions = recipe.Instructions;
        // ... update other properties
        existingRecipe.UpdatedAt = DateTime.UtcNow;
        
        await _recipeService.UpdateRecipeAsync(existingRecipe);
        return RedirectToAction(nameof(Details), new { id = existingRecipe.Id });
    }
    return View(recipe);
}
```

### Delete Operation
```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
{
    var recipe = await _recipeService.GetRecipeByIdAsync(id);
    var user = await _userManager.GetUserAsync(User);
    
    if (recipe.UserId != user?.Id) return Forbid();
    
    // Delete associated image
    if (!string.IsNullOrEmpty(recipe.ImagePath))
    {
        var imageService = HttpContext.RequestServices.GetService<IImageService>();
        imageService.DeleteImage(recipe.ImagePath);
    }
    
    await _recipeService.DeleteRecipeAsync(id);
    return RedirectToAction(nameof(MyRecipes));
}
```

---

## ⚙️ Services & Business Logic

### Recipe Service
```csharp
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
```

### Smart Fridge Logic
```csharp
public async Task<List<Recipe>> GetRecipesByIngredientsAsync(List<int> ingredientIds)
{
    if (!ingredientIds.Any())
        return new List<Recipe>();

    return await _context.Recipes
        .Where(r => r.IsPublic)
        .Include(r => r.RecipeIngredients)
        .ThenInclude(ri => ri.Ingredient)
        .Include(r => r.Categories)
        // Find recipes where all required ingredients are in the fridge
        .Where(r => r.RecipeIngredients.All(ri => ingredientIds.Contains(ri.IngredientId)))
        .ToListAsync();
}
```

### Image Service
```csharp
public async Task<string?> SaveImageAsync(IFormFile imageFile, string subFolder = "recipes")
{
    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", subFolder);
    if (!Directory.Exists(uploadsFolder))
    {
        Directory.CreateDirectory(uploadsFolder);
    }

    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
        await imageFile.CopyToAsync(fileStream);
    }

    return $"/images/{subFolder}/{uniqueFileName}";
}
```

---

## 🛡️ Security Features

### Input Validation
```csharp
public class Recipe
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Instructions { get; set; } = string.Empty;

    [Range(0, 500)]
    public int PrepTime { get; set; }
}
```

### Anti-Forgery Tokens
```html
<form asp-action="Create" method="post">
    @Html.AntiForgeryToken()
    <!-- Form fields -->
</form>
```

### File Upload Security
```csharp
// Validate file type and size
if (ImageFile != null && ImageFile.Length > 0)
{
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
    var extension = Path.GetExtension(ImageFile.FileName).ToLower();
    
    if (!allowedExtensions.Contains(extension))
    {
        ModelState.AddModelError("ImageFile", "Invalid file type.");
    }
    
    if (ImageFile.Length > 5 * 1024 * 1024) // 5MB
    {
        ModelState.AddModelError("ImageFile", "File size too large.");
    }
}
```

---

## 🌐 API Endpoints

### Recipe Endpoints
| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/Recipe` | Browse all recipes | Optional |
| GET | `/Recipe/Details/{id}` | View recipe details | Optional |
| GET | `/Recipe/Create` | Create recipe form | Required |
| POST | `/Recipe/Create` | Submit new recipe | Required |
| GET | `/Recipe/Edit/{id}` | Edit recipe form | Required (Owner) |
| POST | `/Recipe/Edit/{id}` | Update recipe | Required (Owner) |
| POST | `/Recipe/Delete/{id}` | Delete recipe | Required (Owner) |
| GET | `/Recipe/MyRecipes` | User's recipes | Required |

### Fridge Endpoints
| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/Fridge` | Manage fridge | Required |
| POST | `/Fridge/AddToFridge` | Add ingredient | Required |
| POST | `/Fridge/RemoveFromFridge` | Remove ingredient | Required |
| GET | `/Fridge/CookNow` | Find matching recipes | Required |

### Shopping List Endpoints
| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| GET | `/ShoppingList` | View lists | Required |
| GET | `/ShoppingList/Generate/{recipeId}` | Create list form | Required |
| POST | `/ShoppingList/CreateForRecipe` | Generate list | Required |

### Authentication Endpoints
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/Identity/Account/Register` | Registration form |
| POST | `/Identity/Account/Register` | Create account |
| GET | `/Identity/Account/Login` | Login form |
| POST | `/Identity/Account/Login` | Authenticate user |
| POST | `/Identity/Account/Logout` | Logout user |

---

## 📁 File Structure

```
TheKitchen/
├── 📁 Controllers/
│   ├── HomeController.cs          # Homepage and static pages
│   ├── RecipeController.cs        # Recipe CRUD operations
│   ├── FridgeController.cs        # Ingredient management
│   └── ShoppingListController.cs  # Shopping list management
│
├── 📁 Models/
│   ├── 📁 Entities/               # Database models
│   │   ├── Recipe.cs
│   │   ├── Ingredient.cs
│   │   ├── FridgeItem.cs
│   │   ├── ShoppingList.cs
│   │   └── Category.cs
│   │
│   ├── 📁 ViewModels/             # View-specific models
│   └── 📁 Data/
│       └── ApplicationDbContext.cs # Database context
│
├── 📁 Services/
│   ├── 📁 Interfaces/             # Service contracts
│   │   ├── IRecipeService.cs
│   │   ├── IFridgeService.cs
│   │   ├── IShoppingListService.cs
│   │   └── IImageService.cs
│   │
│   ├── RecipeService.cs           # Recipe business logic
│   ├── FridgeService.cs           # Fridge management
│   ├── ShoppingListService.cs     # List operations
│   └── ImageService.cs            # File handling
│
├── 📁 Views/                      # Razor templates
│   ├── 📁 Shared/                 # Layout and partials
│   ├── 📁 Home/                   # Homepage views
│   ├── 📁 Recipe/                 # Recipe management views
│   ├── 📁 Fridge/                 # Fridge management views
│   └── 📁 ShoppingList/           # Shopping list views
│
├── 📁 Areas/                      # Identity area
│   └── 📁 Identity/
│       └── 📁 Pages/
│           └── 📁 Account/
│               ├── Login.cshtml
│               ├── Register.cshtml
│               └── Logout.cshtml
│
├── 📁 wwwroot/                    # Static assets
│   ├── 📁 css/
│   │   └── site.css              # Custom styles
│   ├── 📁 js/
│   │   └── site.js               # Client-side scripts
│   └── 📁 images/
│       ├── 📁 recipes/           # Uploaded recipe images
│       └── 📁 icons/             # UI icons
│
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration
└── TheKitchen.csproj             # Project configuration
```

---

## ⚙️ Configuration

### Application Settings
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TheKitchen;User=root;Password=;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Entity Framework Configuration
```csharp
// Program.cs
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

### Static File Configuration
```csharp
app.UseStaticFiles(); // Serves wwwroot files
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
```

---

## 🚀 Deployment Notes

### Database Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Apply to database
dotnet ef database update
```

### Production Considerations
- Use environment-specific app settings
- Configure production database connection
- Set up proper file storage for images
- Implement HTTPS enforcement
- Configure logging and monitoring

---

## 📊 Performance Optimizations

### Eager Loading
```csharp
var recipes = await _context.Recipes
    .Include(r => r.RecipeIngredients)
    .ThenInclude(ri => ri.Ingredient)
    .Include(r => r.Categories)
    .ToListAsync();
```

### Pagination Ready
```csharp
public async Task<List<Recipe>> GetRecipesPaginatedAsync(int page, int pageSize)
{
    return await _context.Recipes
        .Where(r => r.IsPublic)
        .OrderBy(r => r.Title)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
}
```

---

## 🔮 Future Enhancements

### Potential Features
- Recipe rating and review system
- Meal planning calendar
- Nutritional information tracking
- Social features (follow users, share recipes)
- Advanced search filters
- Recipe scaling (adjust servings)
- Mobile application
- API for third-party integrations

### Technical Improvements
- Caching implementation
- Background jobs for image processing
- Real-time notifications
- Advanced security features
- Performance monitoring
- Automated testing suite

---

## 📞 Support & Maintenance

For technical support or questions about this implementation, please refer to the source code documentation or create an issue in the project repository.

---

**Documentation Version**: 1.0  
**Last Updated**: 2024  
**Project Repository**: [github.com/PranavSync/the-kitchen](https://github.com/PranavSync/the-kitchen)

---

*This documentation provides comprehensive technical details about The Kitchen recipe management system. For user-facing documentation, please refer to the README.md file.*