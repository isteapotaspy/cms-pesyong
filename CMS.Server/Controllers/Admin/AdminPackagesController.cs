using CMS.Contracts.Admin.Package;
using CMS.Domain.Entities.Packages;
using CMS.Domain.Enums;
using CMS.Infrastructure.Persistence;
using CMS.Server.Services.Statistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Controllers.Admin;

[ApiController]
[Route("api/admin/packages")]
public sealed class AdminPackagesController : ControllerBase
{
    private readonly CmsDbContext _dbContext;
    private readonly IStatBroadcaster _statBroadcaster;

    public AdminPackagesController(
        CmsDbContext dbContext,
        IStatBroadcaster statBroadcaster)
    {
        _dbContext = dbContext;
        _statBroadcaster = statBroadcaster;
    }

    [HttpGet]
    public async Task<ActionResult<List<PackageDto>>> GetAll()
    {
        var packages = await PackageQuery()
            .OrderBy(package => package.Title)
            .ToListAsync();

        return Ok(packages.Select(ToDto).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PackageDto>> GetById(int id)
    {
        var package = await PackageQuery()
            .FirstOrDefaultAsync(package => package.Id == id);

        if (package is null)
        {
            return NotFound();
        }

        return Ok(ToDto(package));
    }

    [HttpGet("lookups")]
    public async Task<ActionResult<PackageLookupDto>> GetLookups()
    {
        var result = new PackageLookupDto
        {
            MenuCategories = await _dbContext.MenuCategories
                .AsNoTracking()
                .OrderBy(category => category.Name)
                .Select(category => new LookupItemDto
                {
                    Id = category.Id,
                    Name = category.Name
                })
                .ToListAsync(),

            Meals = await _dbContext.Meals
                .AsNoTracking()
                .OrderBy(meal => meal.Name)
                .Select(meal => new LookupItemDto
                {
                    Id = meal.Id,
                    Name = meal.Name
                })
                .ToListAsync(),

            PackageSelectionTypes = Enum.GetNames<PackageSelectionType>().ToList(),
            MealTypes = Enum.GetNames<MealType>().ToList()
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PackageDto>> Create(CreatePackageRequest request)
    {
        var headerValidation = await ValidatePackageHeaderAsync(
            request.MenuCategoryId,
            request.Title);

        if (!headerValidation.IsValid)
        {
            return BadRequest(headerValidation.ErrorMessage);
        }

        var sizesBuildResult = await BuildSizesAsync(request.Sizes);

        if (!sizesBuildResult.IsValid)
        {
            return BadRequest(sizesBuildResult.ErrorMessage);
        }

        var package = new Package
        {
            MenuCategoryId = request.MenuCategoryId,

            Title = Clean(request.Title),
            Description = Clean(request.Description),
            CardSummary = Clean(request.CardSummary),
            Badge = Clean(request.Badge),
            Notice = Clean(request.Notice),
            ServesLabel = Clean(request.ServesLabel),
            InclusionText = Clean(request.InclusionText),
            ImageUrl = Clean(request.ImageUrl),

            Rating = request.Rating,
            ReviewCount = request.ReviewCount,
            IsAvailable = request.IsAvailable,
            IsCustomizable = request.IsCustomizable,

            Sizes = sizesBuildResult.Sizes,
            Addons = BuildAddons(request.Addons)
        };

        _dbContext.Packages.Add(package);
        await _dbContext.SaveChangesAsync();

        await _statBroadcaster.BroadcastDashboardStatsAsync();

        var createdPackage = await PackageQuery()
            .FirstAsync(existing => existing.Id == package.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPackage.Id },
            ToDto(createdPackage));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PackageDto>> Update(
        int id,
        UpdatePackageRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("Route id and request id do not match.");
        }

        var headerValidation = await ValidatePackageHeaderAsync(
            request.MenuCategoryId,
            request.Title);

        if (!headerValidation.IsValid)
        {
            return BadRequest(headerValidation.ErrorMessage);
        }

        var sizesBuildResult = await BuildSizesAsync(request.Sizes);

        if (!sizesBuildResult.IsValid)
        {
            return BadRequest(sizesBuildResult.ErrorMessage);
        }

        var package = await _dbContext.Packages
            .Include(existing => existing.Sizes)
                .ThenInclude(size => size.SelectionRules)
                    .ThenInclude(rule => rule.Options)
            .Include(existing => existing.Addons)
            .FirstOrDefaultAsync(existing => existing.Id == id);

        if (package is null)
        {
            return NotFound();
        }

        var oldRules = package.Sizes
            .SelectMany(size => size.SelectionRules)
            .ToList();

        var oldOptions = oldRules
            .SelectMany(rule => rule.Options)
            .ToList();

        _dbContext.PackageSelectionOptions.RemoveRange(oldOptions);
        _dbContext.PackageSelectionRules.RemoveRange(oldRules);
        _dbContext.PackageSizes.RemoveRange(package.Sizes);
        _dbContext.PackageAddons.RemoveRange(package.Addons);

        package.MenuCategoryId = request.MenuCategoryId;

        package.Title = Clean(request.Title);
        package.Description = Clean(request.Description);
        package.CardSummary = Clean(request.CardSummary);
        package.Badge = Clean(request.Badge);
        package.Notice = Clean(request.Notice);
        package.ServesLabel = Clean(request.ServesLabel);
        package.InclusionText = Clean(request.InclusionText);
        package.ImageUrl = Clean(request.ImageUrl);

        package.Rating = request.Rating;
        package.ReviewCount = request.ReviewCount;
        package.IsAvailable = request.IsAvailable;
        package.IsCustomizable = request.IsCustomizable;

        package.Sizes = sizesBuildResult.Sizes;
        package.Addons = BuildAddons(request.Addons);

        await _dbContext.SaveChangesAsync();

        await _statBroadcaster.BroadcastDashboardStatsAsync();

        var updatedPackage = await PackageQuery()
            .FirstAsync(existing => existing.Id == id);

        return Ok(ToDto(updatedPackage));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var package = await _dbContext.Packages
            .Include(existing => existing.Sizes)
                .ThenInclude(size => size.SelectionRules)
                    .ThenInclude(rule => rule.Options)
            .Include(existing => existing.Addons)
            .FirstOrDefaultAsync(existing => existing.Id == id);

        if (package is null)
        {
            return NotFound();
        }

        var rules = package.Sizes
            .SelectMany(size => size.SelectionRules)
            .ToList();

        var options = rules
            .SelectMany(rule => rule.Options)
            .ToList();

        _dbContext.PackageSelectionOptions.RemoveRange(options);
        _dbContext.PackageSelectionRules.RemoveRange(rules);
        _dbContext.PackageSizes.RemoveRange(package.Sizes);
        _dbContext.PackageAddons.RemoveRange(package.Addons);
        _dbContext.Packages.Remove(package);

        await _dbContext.SaveChangesAsync();

        await _statBroadcaster.BroadcastDashboardStatsAsync();

        return NoContent();
    }

    private IQueryable<Package> PackageQuery()
    {
        return _dbContext.Packages
            .AsNoTracking()
            .Include(package => package.MenuCategory)
            .Include(package => package.Sizes)
                .ThenInclude(size => size.SelectionRules)
                    .ThenInclude(rule => rule.Options)
                        .ThenInclude(option => option.Meal)
            .Include(package => package.Addons);
    }

    private async Task<PackageHeaderValidationResult> ValidatePackageHeaderAsync(
        int menuCategoryId,
        string title)
    {
        if (menuCategoryId <= 0)
        {
            return PackageHeaderValidationResult.Fail("Menu category is required.");
        }

        var categoryExists = await _dbContext.MenuCategories
            .AsNoTracking()
            .AnyAsync(category => category.Id == menuCategoryId);

        if (!categoryExists)
        {
            return PackageHeaderValidationResult.Fail(
                $"Menu category id {menuCategoryId} was not found.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return PackageHeaderValidationResult.Fail("Package title is required.");
        }

        return PackageHeaderValidationResult.Success();
    }

    private async Task<PackageSizesBuildResult> BuildSizesAsync(
        IEnumerable<PackageSizeRequest>? requests)
    {
        var sizes = new List<PackageSize>();

        foreach (var request in requests ?? Enumerable.Empty<PackageSizeRequest>())
        {
            if (string.IsNullOrWhiteSpace(request.Label))
            {
                return PackageSizesBuildResult.Fail(
                    "Each package size must have a label.");
            }

            if (request.PaxCount <= 0)
            {
                return PackageSizesBuildResult.Fail(
                    $"Package size '{request.Label}' must have a pax count greater than zero.");
            }

            if (request.Price < 0)
            {
                return PackageSizesBuildResult.Fail(
                    $"Package size '{request.Label}' cannot have a negative price.");
            }

            var rulesBuildResult = await BuildSelectionRulesAsync(
                request.SelectionRules);

            if (!rulesBuildResult.IsValid)
            {
                return PackageSizesBuildResult.Fail(
                    rulesBuildResult.ErrorMessage);
            }

            sizes.Add(new PackageSize
            {
                Label = Clean(request.Label),
                Subtitle = Clean(request.Subtitle),
                PaxCount = request.PaxCount,
                Price = request.Price,
                IsAvailable = true,
                SelectionRules = rulesBuildResult.Rules
            });
        }

        return PackageSizesBuildResult.Success(sizes);
    }

    private async Task<PackageRulesBuildResult> BuildSelectionRulesAsync(
        IEnumerable<PackageSelectionRuleRequest>? requests)
    {
        var rules = new List<PackageSelectionRule>();

        foreach (var request in requests ?? Enumerable.Empty<PackageSelectionRuleRequest>())
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return PackageRulesBuildResult.Fail(
                    "Each selection rule must have a title.");
            }

            if (!Enum.TryParse(
                    request.SelectionType,
                    ignoreCase: true,
                    out PackageSelectionType selectionType))
            {
                return PackageRulesBuildResult.Fail(
                    $"Invalid package selection type: {request.SelectionType}");
            }

            if (!Enum.TryParse(
                    request.AllowedMealType,
                    ignoreCase: true,
                    out MealType allowedMealType))
            {
                return PackageRulesBuildResult.Fail(
                    $"Invalid meal type: {request.AllowedMealType}");
            }

            if (request.MinSelections < 0)
            {
                return PackageRulesBuildResult.Fail(
                    $"Rule '{request.Title}' cannot have negative MinSelections.");
            }

            if (request.MaxSelections < request.MinSelections)
            {
                return PackageRulesBuildResult.Fail(
                    $"Rule '{request.Title}' has MaxSelections lower than MinSelections.");
            }

            if (request.IsRequired && request.MinSelections <= 0)
            {
                return PackageRulesBuildResult.Fail(
                    $"Required rule '{request.Title}' must require at least one selection.");
            }

            var options = new List<PackageSelectionOption>();

            foreach (var optionRequest in request.Options ?? Enumerable.Empty<PackageSelectionOptionRequest>())
            {
                if (optionRequest.MealId <= 0)
                {
                    return PackageRulesBuildResult.Fail(
                        $"Rule '{request.Title}' contains an invalid meal option.");
                }

                var meal = await _dbContext.Meals
                    .AsNoTracking()
                    .FirstOrDefaultAsync(existingMeal => existingMeal.Id == optionRequest.MealId);

                if (meal is null)
                {
                    return PackageRulesBuildResult.Fail(
                        $"Meal id {optionRequest.MealId} was not found for rule '{request.Title}'.");
                }

                if (meal.MealType != allowedMealType)
                {
                    return PackageRulesBuildResult.Fail(
                        $"Meal '{meal.Name}' is '{meal.MealType}', but rule '{request.Title}' only allows '{allowedMealType}'.");
                }

                if (optionRequest.AdditionalPrice < 0)
                {
                    return PackageRulesBuildResult.Fail(
                        $"Option meal '{meal.Name}' cannot have a negative additional price.");
                }

                options.Add(new PackageSelectionOption
                {
                    MealId = optionRequest.MealId,
                    AdditionalPrice = optionRequest.AdditionalPrice,
                    IsDefault = optionRequest.IsDefault,
                    IsActive = true
                });
            }

            if (request.IsRequired && options.Count < request.MinSelections)
            {
                return PackageRulesBuildResult.Fail(
                    $"Rule '{request.Title}' needs at least {request.MinSelections} option(s).");
            }

            rules.Add(new PackageSelectionRule
            {
                Title = Clean(request.Title),
                Description = Clean(request.Description),
                SelectionType = selectionType,
                AllowedMealType = allowedMealType,
                MinSelections = request.MinSelections,
                MaxSelections = request.MaxSelections,
                IsRequired = request.IsRequired,
                DisplayOrder = request.DisplayOrder,
                IsActive = true,
                Options = options
            });
        }

        return PackageRulesBuildResult.Success(rules);
    }

    private static List<PackageAddon> BuildAddons(
        IEnumerable<PackageAddonRequest>? requests)
    {
        return (requests ?? Enumerable.Empty<PackageAddonRequest>())
            .Where(request => !string.IsNullOrWhiteSpace(request.Name))
            .Select(request => new PackageAddon
            {
                Name = Clean(request.Name),
                Description = Clean(request.Description),
                Price = request.Price < 0 ? 0 : request.Price,
                IsAvailable = request.IsAvailable
            })
            .ToList();
    }

    private static PackageDto ToDto(Package package)
    {
        return new PackageDto
        {
            Id = package.Id,
            CreatedAt = package.CreatedAtUtc,
            UpdatedAt = package.UpdatedAtUtc,

            MenuCategoryId = package.MenuCategoryId,
            MenuCategoryName = package.MenuCategory?.Name ?? string.Empty,

            Title = package.Title,
            Description = package.Description,
            CardSummary = package.CardSummary,
            Badge = package.Badge,
            Notice = package.Notice,
            ServesLabel = package.ServesLabel,
            InclusionText = package.InclusionText,
            ImageUrl = package.ImageUrl,

            Rating = package.Rating,
            ReviewCount = package.ReviewCount,
            IsAvailable = package.IsAvailable,
            IsCustomizable = package.IsCustomizable,

            Sizes = package.Sizes
                .OrderBy(size => size.PaxCount)
                .Select(size => new PackageSizeDto
                {
                    Id = size.Id,
                    Label = size.Label,
                    Subtitle = size.Subtitle,
                    PaxCount = size.PaxCount,
                    Price = size.Price,
                    SelectionRules = MapSelectionRules(size.SelectionRules)
                })
                .ToList(),

            Addons = package.Addons
                .OrderBy(addon => addon.Name)
                .Select(addon => new PackageAddonDto
                {
                    Id = addon.Id,
                    Name = addon.Name,
                    Description = addon.Description,
                    Price = addon.Price,
                    IsAvailable = addon.IsAvailable
                })
                .ToList()
        };
    }

    private static List<PackageSelectionRuleDto> MapSelectionRules(
        IEnumerable<PackageSelectionRule>? rules)
    {
        return (rules ?? Enumerable.Empty<PackageSelectionRule>())
            .OrderBy(rule => rule.DisplayOrder)
            .Select(rule => new PackageSelectionRuleDto
            {
                Id = rule.Id,
                Title = rule.Title,
                Description = rule.Description,
                SelectionType = rule.SelectionType.ToString(),
                AllowedMealType = rule.AllowedMealType.ToString(),
                MinSelections = rule.MinSelections,
                MaxSelections = rule.MaxSelections,
                IsRequired = rule.IsRequired,
                DisplayOrder = rule.DisplayOrder,

                Options = (rule.Options ?? Enumerable.Empty<PackageSelectionOption>())
                    .OrderBy(option => option.Meal != null ? option.Meal.Name : string.Empty)
                    .Select(option => new PackageSelectionOptionDto
                    {
                        Id = option.Id,
                        MealId = option.MealId,
                        MealName = option.Meal != null ? option.Meal.Name : string.Empty,
                        AdditionalPrice = option.AdditionalPrice,
                        IsDefault = option.IsDefault
                    })
                    .ToList()
            })
            .ToList();
    }

    private static string Clean(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private sealed record PackageHeaderValidationResult(
        bool IsValid,
        string? ErrorMessage)
    {
        public static PackageHeaderValidationResult Success()
        {
            return new PackageHeaderValidationResult(true, null);
        }

        public static PackageHeaderValidationResult Fail(string? errorMessage)
        {
            return new PackageHeaderValidationResult(false, errorMessage);
        }
    }

    private sealed record PackageSizesBuildResult(
        bool IsValid,
        List<PackageSize> Sizes,
        string? ErrorMessage)
    {
        public static PackageSizesBuildResult Success(List<PackageSize> sizes)
        {
            return new PackageSizesBuildResult(true, sizes, null);
        }

        public static PackageSizesBuildResult Fail(string? errorMessage)
        {
            return new PackageSizesBuildResult(false, new List<PackageSize>(), errorMessage);
        }
    }

    private sealed record PackageRulesBuildResult(
        bool IsValid,
        List<PackageSelectionRule> Rules,
        string? ErrorMessage)
    {
        public static PackageRulesBuildResult Success(List<PackageSelectionRule> rules)
        {
            return new PackageRulesBuildResult(true, rules, null);
        }

        public static PackageRulesBuildResult Fail(string? errorMessage)
        {
            return new PackageRulesBuildResult(false, new List<PackageSelectionRule>(), errorMessage);
        }
    }
}