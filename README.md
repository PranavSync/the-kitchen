# The Kitchen - Recipe Management System 🍳

Welcome to **The Kitchen** - your personal recipe companion! This is a beautiful, functional web application that helps you discover, create, and manage recipes while making grocery shopping a breeze.

![The Kitchen Banner](https://via.placeholder.com/800x400/8B4513/FFFFFF?text=The+Kitchen+-+Your+Recipe+Companion)

## 🌟 What is The Kitchen?

Imagine having a smart cookbook that not only stores your favorite recipes but also tells you what you can cook right now with ingredients you already have! That's exactly what The Kitchen does.

### ✨ Key Features That Make Cooking Fun:

- **🔐 Secure Accounts** - Create your personal account and keep your recipes safe
- **📖 Recipe Collection** - Browse through a growing collection of delicious recipes
- **➕ Share Your Creations** - Add your own recipes with beautiful photos
- **✏️ Easy Editing** - Update your recipes anytime
- **🗑️ Clean Management** - Remove recipes you no longer need
- **🏠 Smart Fridge** - Track what ingredients you have at home
- **🛒 Smart Shopping** - Generate shopping lists for new recipes
- **📱 Beautiful Design** - Enjoy a warm, cozy interface that feels like home

---

## 🖼️ See It In Action

### Homepage - Your Cooking Dashboard
![Homepage](https://via.placeholder.com/600x400/FFF8DC/8B4513?text=Homepage+with+Featured+Recipes)
*Welcome to your kitchen! Browse featured recipes and quick actions.*

### Recipe Collection
![Recipes Page](https://via.placeholder.com/600x400/FFF8DC/8B4513?text=Browse+All+Recipes)
*Discover new recipes from our community collection*

### Recipe Details
![Recipe Details](https://via.placeholder.com/600x400/FFF8DC/8B4513?text=Recipe+Details+with+Ingredients)
*View complete recipe instructions and ingredients*

### My Fridge - Smart Ingredient Tracking
![My Fridge](https://via.placeholder.com/600x400/FFF8DC/8B4513?text=My+Fridge+-+Track+Ingredients)
*See what ingredients you have and find matching recipes*

### Create Beautiful Recipes
![Create Recipe](https://via.placeholder.com/600x400/FFF8DC/8B4513?text=Create+New+Recipe+Form)
*Share your culinary creations with the community*

---

## 🚀 Getting Started - Let's Cook!

Follow these simple steps to get The Kitchen running on your computer:

### Prerequisites - What You'll Need:

1. **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **MySQL Server** - [Download here](https://dev.mysql.com/downloads/mysql/)
3. **Git** - [Download here](https://git-scm.com/downloads)

### 🛠️ Installation Steps:

#### Step 1: Get the Code
```bash
# Copy this code to your command prompt
git clone https://github.com/PranavSync/the-kitchen.git
cd the-kitchen
```

#### Step 2: Setup the Database

1. **Open MySQL** (MySQL Workbench or command line)
2. **Create a new database**:
```sql
CREATE DATABASE TheKitchen;
```

3. **Update the connection settings** in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TheKitchen;User=root;Password=your_mysql_password;"
  }
}
```
*Replace `your_mysql_password` with your actual MySQL password*

#### Step 3: Build the Database Structure
```bash
# Run these commands one by one:
dotnet ef database update
```

#### Step 4: Launch The Kitchen!
```bash
dotnet run
```

#### Step 5: Open Your Browser
Visit: `https://localhost:7000` or `http://localhost:5264`

---

## 👨‍🍳 Your First Cooking Session

### For New Users:
1. **Click "Register"** to create your account
2. **Setup your virtual fridge** - Add 3+ ingredients you have at home
3. **Click "Cook Now"** - See recipes you can make immediately!
4. **Browse recipes** - Get inspired by community creations
5. **Share your first recipe** - Click "Share Recipe" to contribute

### For Returning Users:
1. **Login** to your account
2. **Manage your fridge** - Keep your ingredients updated
3. **Create shopping lists** - Plan your grocery trips
4. **Edit your recipes** - Improve and update your creations

---

## 🏗️ Project Structure

```
TheKitchen/
├── 📁 Controllers/     # Handles user requests
├── 📁 Models/          # Data structures and recipes
├── 📁 Views/           # Beautiful pages you see
├── 📁 Services/        # Cooking logic and smart features
├── 📁 wwwroot/         # Images, CSS, and styling
├── 📄 Program.cs       # Application startup
└── 📄 README.md        # This file!
```

---

## 🛠️ Built With Love Using:

- **ASP.NET Core 8.0** - Robust backend framework
- **Entity Framework Core** - Smart database management
- **MySQL** - Reliable data storage
- **Bootstrap 5** - Beautiful, responsive design
- **ASP.NET Identity** - Secure user accounts

---

## 🤝 Contributing to Our Kitchen

Found a bug? Have a feature idea? We'd love your help!

1. **Fork** the repository
2. **Create** a feature branch
3. **Cook up** your changes
4. **Submit** a Pull Request

---

## 📝 Academic Note

This project was developed as part of academic coursework to demonstrate:
- ✅ User authentication and authorization
- ✅ Database design and integration
- ✅ Complete CRUD operations
- ✅ Full-stack web development skills

---

## 📞 Need Help in the Kitchen?

- **Developer**: Pranav
- **Course**: Academic Project
- **Repository**: [github.com/PranavSync/the-kitchen](https://github.com/PranavSync/the-kitchen)

---

## 🎉 Welcome to The Kitchen!

We're excited to have you join our cooking community! Whether you're a seasoned chef or just starting your culinary journey, The Kitchen is here to make cooking more enjoyable, organized, and creative.

**Happy cooking!** 👨‍🍳👩‍🍳

---

*"Good food is the foundation of genuine happiness." - Auguste Escoffier*

---

**Note**: For detailed technical documentation, database schema, and API information, please check the `DOCUMENTATION.md` file.