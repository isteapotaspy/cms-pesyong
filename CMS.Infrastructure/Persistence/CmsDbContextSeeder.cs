using CMS.Domain.Entities;
using CMS.Domain.Entities.Menu;
using CMS.Domain.Entities.Packages;
using CMS.Domain.Entities.User;
using CMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CMS.Infrastructure.Persistence;

public static class CmsDbContextSeeder
{
    static MealType MealType = (CMS.Domain.Enums.MealType) MealType.Viand;

    public static async Task SeedAsync(CmsDbContext db, CancellationToken cancellationToken = default)
    {
        // Safe in development. 
        await db.Database.MigrateAsync(cancellationToken);

        var adminUser = await EnsureAdminUserAsync(db, cancellationToken);
        var customerProfile = await EnsureDemoCustomerAsync(db, cancellationToken);

        var cateringCategory = await EnsureMenuCategoryAsync(
            db, "Catering", "catering", cancellationToken);

        var shortOrdersCategory = await EnsureMenuCategoryAsync(
            db, "Short Orders", "short-orders", cancellationToken);

        var kakaninCategory = await EnsureMenuCategoryAsync(
            db, "Authentic Kakanin", "authentic-kakanin", cancellationToken);

        // Meals
        var chickenInasal = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = cateringCategory.Id,
            Name = "Chicken Inasal",
            Description = "Grilled marinated chicken with signature smoky flavor.",
            MealType = MealType.Viand,
            BasePrice = 250m,
            StockQuantity = 100,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-one",
            IsAvailable = true,
            IsViandOption = true
        }, cancellationToken);

        var porkAdobo = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = shortOrdersCategory.Id,
            Name = "Pork Adobo",
            Description = "Classic Filipino pork adobo simmered in soy sauce and vinegar.",
            MealType = MealType.Viand,
            BasePrice = 240m,
            StockQuantity = 100,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-three",
            IsAvailable = true,
            IsViandOption = true
        }, cancellationToken);

        var beefCaldereta = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = cateringCategory.Id,
            Name = "Beef Caldereta",
            Description = "Rich tomato-based beef stew with vegetables.",
            MealType = MealType.Viand,
            BasePrice = 280m,
            StockQuantity = 80,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-two",
            IsAvailable = true,
            IsViandOption = true
        }, cancellationToken);

        var porkSisig = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = shortOrdersCategory.Id,
            Name = "Pork Sisig",
            Description = "Sizzling chopped pork with onion and chili.",
            MealType = MealType.Viand,
            BasePrice = 220m,
            StockQuantity = 90,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-three",
            IsAvailable = true,
            IsViandOption = true
        }, cancellationToken);

        var pancitBihon = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = shortOrdersCategory.Id,
            Name = "Pancit Bihon",
            Description = "Stir-fried rice noodles with vegetables and meat.",
            MealType = MealType.SideDish,
            BasePrice = 180m,
            StockQuantity = 70,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-two",
            IsAvailable = true,
            IsViandOption = false
        }, cancellationToken);

        var lecheFlan = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = cateringCategory.Id,
            Name = "Leche Flan",
            Description = "Smooth caramel custard dessert.",
            MealType = MealType.Dessert,
            BasePrice = 120m,
            StockQuantity = 50,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-four",
            IsAvailable = true,
            IsViandOption = false
        }, cancellationToken);

        var mangoFloat = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = cateringCategory.Id,
            Name = "Mango Float",
            Description = "Chilled mango graham dessert.",
            MealType = MealType.Dessert,
            BasePrice = 140m,
            StockQuantity = 50,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-four",
            IsAvailable = true,
            IsViandOption = false
        }, cancellationToken);

        var icedTea = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = cateringCategory.Id,
            Name = "Iced Tea",
            Description = "Refreshing house-blend iced tea.",
            MealType = MealType.Beverage,
            BasePrice = 80m,
            StockQuantity = 100,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-one",
            IsAvailable = true,
            IsViandOption = false
        }, cancellationToken);

        var pineappleJuice = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = cateringCategory.Id,
            Name = "Pineapple Juice",
            Description = "Sweet chilled pineapple juice.",
            MealType = MealType.Beverage,
            BasePrice = 90m,
            StockQuantity = 100,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-two",
            IsAvailable = true,
            IsViandOption = false
        }, cancellationToken);

        var specialBiko = await EnsureMealAsync(db, new Meal
        {
            MenuCategoryId = kakaninCategory.Id,
            Name = "Special Biko",
            Description = "Sticky rice dessert topped with rich coconut latik.",
            MealType = MealType.Kakanin,
            BasePrice = 160m,
            StockQuantity = 40,
            MinOrderQuantity = 1,
            ImageUrl = "food-image-four",
            IsAvailable = true,
            IsViandOption = false
        }, cancellationToken);

        // Packages
        var fiestaPackage = await EnsurePackageAsync(db, new Package
        {
            MenuCategoryId = cateringCategory.Id,
            Title = "Grand Fiesta Package A",
            Description = "Complete feast for celebrations with customizable viands, dessert, and beverage.",
            CardSummary = "A flexible catering package for family gatherings and events.",
            Badge = "Popular",
            Notice = "24-48h Notice",
            ServesLabel = "10-50 Pax",
            InclusionText = "Complete utensils included",
            ImageUrl = "food-image-one",
            Rating = 4.8m,
            ReviewCount = 124,
            IsAvailable = true,
            IsCustomizable = true
        }, cancellationToken);

        var boodlePackage = await EnsurePackageAsync(db, new Package
        {
            MenuCategoryId = cateringCategory.Id,
            Title = "Seafood Boodle Fight",
            Description = "Crabs, prawns, mussels, grilled fish, and squid served on banana leaves.",
            CardSummary = "Seafood-heavy feast perfect for group celebrations.",
            Badge = "Top Rated",
            Notice = "Best Seller",
            ServesLabel = "10-20 Pax",
            InclusionText = "Complete utensils included",
            ImageUrl = "food-image-two",
            Rating = 4.9m,
            ReviewCount = 98,
            IsAvailable = true,
            IsCustomizable = false
        }, cancellationToken);

        var bikoTrayPackage = await EnsurePackageAsync(db, new Package
        {
            MenuCategoryId = kakaninCategory.Id,
            Title = "Special Biko with Latik",
            Description = "Traditional kakanin tray for gifting and dessert tables.",
            CardSummary = "Freshly made biko topped with coconut latik.",
            Badge = "Chef's Choice",
            Notice = "Freshly Made",
            ServesLabel = "Medium Tray",
            InclusionText = "Packed ready for gifting",
            ImageUrl = "food-image-four",
            Rating = 4.9m,
            ReviewCount = 47,
            IsAvailable = true,
            IsCustomizable = false
        }, cancellationToken);

        // Sizes
        await EnsurePackageSizeAsync(db, fiestaPackage.Id, "10 Persons", "4 Main Courses", 10, 2800m, cancellationToken);
        await EnsurePackageSizeAsync(db, fiestaPackage.Id, "20 Persons", "5 Main Courses", 20, 4500m, cancellationToken);
        await EnsurePackageSizeAsync(db, fiestaPackage.Id, "50 Persons", "7 Main Courses", 50, 9800m, cancellationToken);

        await EnsurePackageSizeAsync(db, boodlePackage.Id, "10 Persons", "Seafood Feast", 10, 8900m, cancellationToken);
        await EnsurePackageSizeAsync(db, boodlePackage.Id, "20 Persons", "Expanded Spread", 20, 14500m, cancellationToken);

        await EnsurePackageSizeAsync(db, bikoTrayPackage.Id, "Medium Tray", "Great for sharing", 8, 280m, cancellationToken);

        // Add-ons
        var extraSauce = await EnsurePackageAddonAsync(db, fiestaPackage.Id,
            "Extra Lechon Sauce", "500ml of our signature liver sauce", 150m, cancellationToken);

        var dessertTray = await EnsurePackageAddonAsync(db, fiestaPackage.Id,
            "Premium Dessert Tray", "Add assorted Filipino sweets", 350m, cancellationToken);

        var shrimpTray = await EnsurePackageAddonAsync(db, boodlePackage.Id,
            "Garlic Butter Shrimp Tray", "Extra tray for seafood lovers", 550m, cancellationToken);

        // Customization rules for Grand Fiesta Package A
        var chooseViands = await EnsurePackageSelectionRuleAsync(db, new PackageSelectionRule
        {
            PackageId = fiestaPackage.Id,
            Title = "Choose 2 Viands",
            Description = "Select two viand dishes for this package.",
            SelectionType = PackageSelectionType.ChooseMany,
            AllowedMealType = MealType.Viand,
            MinSelections = 2,
            MaxSelections = 2,
            IsRequired = true,
            DisplayOrder = 1
        }, cancellationToken);

        var chooseDessert = await EnsurePackageSelectionRuleAsync(db, new PackageSelectionRule
        {
            PackageId = fiestaPackage.Id,
            Title = "Choose 1 Dessert",
            Description = "Select one dessert for this package.",
            SelectionType = PackageSelectionType.ChooseOne,
            AllowedMealType = (MealType)MealType.Dessert,
            MinSelections = 1,
            MaxSelections = 1,
            IsRequired = true,
            DisplayOrder = 2
        }, cancellationToken);

        var chooseDrink = await EnsurePackageSelectionRuleAsync(db, new PackageSelectionRule
        {
            PackageId = fiestaPackage.Id,
            Title = "Choose 1 Beverage",
            Description = "Select one beverage for this package.",
            SelectionType = PackageSelectionType.ChooseOne,
            AllowedMealType = MealType.Beverage,
            MinSelections = 1,
            MaxSelections = 1,
            IsRequired = true,
            DisplayOrder = 3
        }, cancellationToken);

        // Rule options
        await EnsurePackageSelectionOptionAsync(db, chooseViands.Id, chickenInasal.Id, 0m, false, cancellationToken);
        await EnsurePackageSelectionOptionAsync(db, chooseViands.Id, porkAdobo.Id, 0m, true, cancellationToken);
        await EnsurePackageSelectionOptionAsync(db, chooseViands.Id, beefCaldereta.Id, 30m, false, cancellationToken);
        await EnsurePackageSelectionOptionAsync(db, chooseViands.Id, porkSisig.Id, 0m, false, cancellationToken);

        await EnsurePackageSelectionOptionAsync(db, chooseDessert.Id, lecheFlan.Id, 0m, true, cancellationToken);
        await EnsurePackageSelectionOptionAsync(db, chooseDessert.Id, mangoFloat.Id, 20m, false, cancellationToken);

        await EnsurePackageSelectionOptionAsync(db, chooseDrink.Id, icedTea.Id, 0m, true, cancellationToken);
        await EnsurePackageSelectionOptionAsync(db, chooseDrink.Id, pineappleJuice.Id, 10m, false, cancellationToken);
    }

    private static async Task<AppUser> EnsureAdminUserAsync(CmsDbContext db, CancellationToken cancellationToken)
    {
        var existing = await db.AppUsers
            .FirstOrDefaultAsync(x => x.UserName == "admin", cancellationToken);

        if (existing is not null)
            return existing;

        var admin = new AppUser
        {
            UserName = "admin",
            Email = "admin@pesyong.local",
            FirstName = "System",
            LastName = "Admin",
            PasswordHash = "TEMP_ADMIN_HASH_REPLACE_LATER",
            Role = UserRole.Admin,
            IsActive = true
        };

        db.AppUsers.Add(admin);
        await db.SaveChangesAsync(cancellationToken);

        return admin;
    }

    private static async Task<CustomerProfile> EnsureDemoCustomerAsync(CmsDbContext db, CancellationToken cancellationToken)
    {
        var existingUser = await db.AppUsers
            .FirstOrDefaultAsync(x => x.UserName == "customer.demo", cancellationToken);

        if (existingUser is null)
        {
            existingUser = new AppUser
            {
                UserName = "customer.demo",
                Email = "customer.demo@pesyong.local",
                FirstName = "Demo",
                LastName = "Customer",
                PasswordHash = "TEMP_CUSTOMER_HASH_REPLACE_LATER",
                Role = UserRole.Customer,
                IsActive = true
            };

            db.AppUsers.Add(existingUser);
            await db.SaveChangesAsync(cancellationToken);
        }

        var profile = await db.CustomerProfiles
            .FirstOrDefaultAsync(x => x.AppUserId == existingUser.Id, cancellationToken);

        if (profile is null)
        {
            profile = new CustomerProfile
            {
                AppUserId = existingUser.Id,
                MobileNumber = "09171234567"
            };

            db.CustomerProfiles.Add(profile);
            await db.SaveChangesAsync(cancellationToken);
        }

        var hasAddress = await db.Addresses.AnyAsync(
            x => x.CustomerProfileId == profile.Id && x.IsDefault,
            cancellationToken);

        if (!hasAddress)
        {
            db.Addresses.Add(new Address
            {
                CustomerProfileId = profile.Id,
                StreetAddress = "Unit 402, Blue Residences",
                Barangay = "Loyola Heights",
                City = "Quezon City",
                Landmark = "Near Ateneo Gate 3",
                Latitude = null,
                Longitude = null,
                IsDefault = true
            });

            await db.SaveChangesAsync(cancellationToken);
        }

        return profile;
    }

    private static async Task<MenuCategory> EnsureMenuCategoryAsync(
        CmsDbContext db,
        string name,
        string slug,
        CancellationToken cancellationToken)
    {
        var existing = await db.MenuCategories
            .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);

        if (existing is not null)
            return existing;

        var category = new MenuCategory
        {
            Name = name,
            Slug = slug,
            IsActive = true
        };

        db.MenuCategories.Add(category);
        await db.SaveChangesAsync(cancellationToken);

        return category;
    }

    private static async Task<Meal> EnsureMealAsync(
        CmsDbContext db,
        Meal seed,
        CancellationToken cancellationToken)
    {
        var existing = await db.Meals
            .FirstOrDefaultAsync(x => x.Name == seed.Name, cancellationToken);

        if (existing is not null)
            return existing;

        db.Meals.Add(seed);
        await db.SaveChangesAsync(cancellationToken);

        return seed;
    }

    private static async Task<Package> EnsurePackageAsync(
        CmsDbContext db,
        Package seed,
        CancellationToken cancellationToken)
    {
        var existing = await db.Packages
            .FirstOrDefaultAsync(x => x.Title == seed.Title, cancellationToken);

        if (existing is not null)
            return existing;

        db.Packages.Add(seed);
        await db.SaveChangesAsync(cancellationToken);

        return seed;
    }

    private static async Task<PackageSize> EnsurePackageSizeAsync(
        CmsDbContext db,
        int packageId,
        string label,
        string subtitle,
        int paxCount,
        decimal price,
        CancellationToken cancellationToken)
    {
        var existing = await db.PackageSizes
            .FirstOrDefaultAsync(
                x => x.PackageId == packageId && x.Label == label,
                cancellationToken);

        if (existing is not null)
            return existing;

        var size = new PackageSize
        {
            PackageId = packageId,
            Label = label,
            Subtitle = subtitle,
            PaxCount = paxCount,
            Price = price
        };

        db.PackageSizes.Add(size);
        await db.SaveChangesAsync(cancellationToken);

        return size;
    }

    private static async Task<PackageAddon> EnsurePackageAddonAsync(
        CmsDbContext db,
        int packageId,
        string name,
        string description,
        decimal price,
        CancellationToken cancellationToken)
    {
        var existing = await db.PackageAddons
            .FirstOrDefaultAsync(
                x => x.PackageId == packageId && x.Name == name,
                cancellationToken);

        if (existing is not null)
            return existing;

        var addon = new PackageAddon
        {
            PackageId = packageId,
            Name = name,
            Description = description,
            Price = price,
            IsAvailable = true
        };

        db.PackageAddons.Add(addon);
        await db.SaveChangesAsync(cancellationToken);

        return addon;
    }

    private static async Task<PackageSelectionRule> EnsurePackageSelectionRuleAsync(
        CmsDbContext db,
        PackageSelectionRule seed,
        CancellationToken cancellationToken)
    {
        var existing = await db.PackageSelectionRules
            .FirstOrDefaultAsync(
                x => x.PackageId == seed.PackageId && x.Title == seed.Title,
                cancellationToken);

        if (existing is not null)
            return existing;

        db.PackageSelectionRules.Add(seed);
        await db.SaveChangesAsync(cancellationToken);

        return seed;
    }

    private static async Task<PackageSelectionOption> EnsurePackageSelectionOptionAsync(
        CmsDbContext db,
        int ruleId,
        int mealId,
        decimal additionalPrice,
        bool isDefault,
        CancellationToken cancellationToken)
    {
        var existing = await db.PackageSelectionOptions
            .FirstOrDefaultAsync(
                x => x.PackageSelectionRuleId == ruleId && x.MealId == mealId,
                cancellationToken);

        if (existing is not null)
            return existing;

        var option = new PackageSelectionOption
        {
            PackageSelectionRuleId = ruleId,
            MealId = mealId,
            AdditionalPrice = additionalPrice,
            IsDefault = isDefault
        };

        db.PackageSelectionOptions.Add(option);
        await db.SaveChangesAsync(cancellationToken);

        return option;
    }
}