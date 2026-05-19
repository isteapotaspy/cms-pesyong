using CMS.Contracts.Admin.Package;
using CMS.Contracts.Customer.Menu;
using CMS.Domain.Entities.Packages;
using CMS.Domain.Enums;
using CMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Controller.Admin;

[ApiController]
[Route("api/admin/packages")]
public sealed class AdminPackagesController : ControllerBase
{
    private readonly CmsDbContext _dbContext;
   
    public AdminPackagesController(CmsDbContext dbContext)
    {
        _dbContext = dbContext;
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
        if (request.MenuCategoryId <= 0)
        {
            return BadRequest("Menu category is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Package title is required.");
        }

        if (!TryBuildSizes(request.Sizes, packageId: null, out var sizes, out var errorMessage))
        {
            return BadRequest(errorMessage);
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
            Sizes = sizes,
            Addons = BuildAddons(request.Addons, packageId: null)
        };

        _dbContext.Packages.Add(package);
        await _dbContext.SaveChangesAsync();

        var createdPackage = await PackageQuery()
            .FirstAsync(existing => existing.Id == package.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdPackage.Id },
            ToDto(createdPackage));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PackageDto>> Update(int id, UpdatePackageRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("Route id and request id do not match.");
        }

        if (request.MenuCategoryId <= 0)
        {
            return BadRequest("Menu category is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Package title is required.");
        }

        if (!TryBuildSizes(request.Sizes, packageId: id, out var newSizes, out var errorMessage))
        {
            return BadRequest(errorMessage);
        }

        var newAddons = BuildAddons(request.Addons, packageId: id);

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

        package.Sizes = newSizes;
        package.Addons = newAddons;

        await _dbContext.SaveChangesAsync();

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

    private static bool TryBuildSizes(
        IEnumerable<PackageSizeRequest>? requests,
        int? packageId,
        out List<PackageSize> sizes,
        out string? errorMessage)
    {
        sizes = new List<PackageSize>();
        errorMessage = null;

        foreach (var request in requests ?? Enumerable.Empty<PackageSizeRequest>())
        {
            if (string.IsNullOrWhiteSpace(request.Label))
            {
                errorMessage = "Each package size must have a label.";
                return false;
            }

            if (request.PaxCount <= 0)
            {
                errorMessage = $"Package size '{request.Label}' must have a pax count greater than zero.";
                return false;
            }

            if (request.Price < 0)
            {
                errorMessage = $"Package size '{request.Label}' cannot have a negative price.";
                return false;
            }

            if (!TryBuildSelectionRules(request.SelectionRules, out var rules, out errorMessage))
            {
                return false;
            }

            var size = new PackageSize
            {
                Label = Clean(request.Label),
                Subtitle = Clean(request.Subtitle),
                PaxCount = request.PaxCount,
                Price = request.Price,
                SelectionRules = rules
            };

            if (packageId.HasValue)
            {
                size.PackageId = packageId.Value;
            }

            sizes.Add(size);
        }

        return true;
    }

    private static List<PackageAddon> BuildAddons(
        IEnumerable<PackageAddonRequest>? requests,
        int? packageId)
    {
        return (requests ?? Enumerable.Empty<PackageAddonRequest>())
            .Select(request =>
            {
                var addon = new PackageAddon
                {
                    Name = Clean(request.Name),
                    Description = Clean(request.Description),
                    Price = request.Price,
                    IsAvailable = request.IsAvailable
                };

                if (packageId.HasValue)
                {
                    addon.PackageId = packageId.Value;
                }

                return addon;
            })
            .ToList();
    }

    private static bool TryBuildSelectionRules(
        IEnumerable<PackageSelectionRuleRequest>? requests,
        out List<PackageSelectionRule> rules,
        out string? errorMessage)
    {
        rules = new List<PackageSelectionRule>();
        errorMessage = null;

        foreach (var request in requests ?? Enumerable.Empty<PackageSelectionRuleRequest>())
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                errorMessage = "Each selection rule must have a title.";
                return false;
            }

            if (request.MaxSelections < request.MinSelections)
            {
                errorMessage = $"Rule '{request.Title}' has MaxSelections lower than MinSelections.";
                return false;
            }

            if (!Enum.TryParse(request.SelectionType, ignoreCase: true, out PackageSelectionType selectionType))
            {
                errorMessage = $"Invalid package selection type: {request.SelectionType}";
                return false;
            }

            if (!Enum.TryParse(request.AllowedMealType, ignoreCase: true, out MealType allowedMealType))
            {
                errorMessage = $"Invalid meal type: {request.AllowedMealType}";
                return false;
            }

            var rule = new PackageSelectionRule
            {
                Title = Clean(request.Title),
                Description = Clean(request.Description),
                SelectionType = selectionType,
                AllowedMealType = allowedMealType,
                MinSelections = request.MinSelections,
                MaxSelections = request.MaxSelections,
                IsRequired = request.IsRequired,
                DisplayOrder = request.DisplayOrder,
                Options = (request.Options ?? Enumerable.Empty<PackageSelectionOptionRequest>())
                    .Select(option => new PackageSelectionOption
                    {
                        MealId = option.MealId,
                        AdditionalPrice = option.AdditionalPrice,
                        IsDefault = option.IsDefault
                    })
                    .ToList()
            };

            rules.Add(rule);
        }

        return true;
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
}