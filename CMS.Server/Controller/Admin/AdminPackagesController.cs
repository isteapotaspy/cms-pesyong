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

        if (!TryBuildSelectionRules(request.SelectionRules, packageId: 0, out var rules, out var errorMessage))
        {
            return BadRequest(errorMessage);
        }

        var package = new Package
        {
            MenuCategoryId = request.MenuCategoryId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            CardSummary = request.CardSummary.Trim(),
            Badge = request.Badge.Trim(),
            Notice = request.Notice.Trim(),
            ServesLabel = request.ServesLabel.Trim(),
            InclusionText = request.InclusionText.Trim(),
            ImageUrl = request.ImageUrl.Trim(),
            Rating = request.Rating,
            ReviewCount = request.ReviewCount,
            IsAvailable = request.IsAvailable,
            IsCustomizable = request.IsCustomizable,
            Sizes = BuildSizes(request.Sizes, packageId: 0),
            Addons = BuildAddons(request.Addons, packageId: 0),
            SelectionRules = rules
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

        if (!TryBuildSelectionRules(request.SelectionRules, id, out var newRules, out var errorMessage))
        {
            return BadRequest(errorMessage);
        }

        var package = await _dbContext.Packages
            .Include(existing => existing.Sizes)
            .Include(existing => existing.Addons)
            .Include(existing => existing.SelectionRules)
                .ThenInclude(rule => rule.Options)
            .FirstOrDefaultAsync(existing => existing.Id == id);

        if (package is null)
        {
            return NotFound();
        }

        var oldOptions = package.SelectionRules
            .SelectMany(rule => rule.Options)
            .ToList();

        _dbContext.PackageSelectionOptions.RemoveRange(oldOptions);
        _dbContext.PackageSelectionRules.RemoveRange(package.SelectionRules);
        _dbContext.PackageSizes.RemoveRange(package.Sizes);
        _dbContext.PackageAddons.RemoveRange(package.Addons);

        await _dbContext.SaveChangesAsync();

        package.MenuCategoryId = request.MenuCategoryId;
        package.Title = request.Title.Trim();
        package.Description = request.Description.Trim();
        package.CardSummary = request.CardSummary.Trim();
        package.Badge = request.Badge.Trim();
        package.Notice = request.Notice.Trim();
        package.ServesLabel = request.ServesLabel.Trim();
        package.InclusionText = request.InclusionText.Trim();
        package.ImageUrl = request.ImageUrl.Trim();
        package.Rating = request.Rating;
        package.ReviewCount = request.ReviewCount;
        package.IsAvailable = request.IsAvailable;
        package.IsCustomizable = request.IsCustomizable;

        package.Sizes = BuildSizes(request.Sizes, id);
        package.Addons = BuildAddons(request.Addons, id);
        package.SelectionRules = newRules;

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
            .Include(existing => existing.Addons)
            .Include(existing => existing.SelectionRules)
                .ThenInclude(rule => rule.Options)
            .FirstOrDefaultAsync(existing => existing.Id == id);

        if (package is null)
        {
            return NotFound();
        }

        var options = package.SelectionRules
            .SelectMany(rule => rule.Options)
            .ToList();

        _dbContext.PackageSelectionOptions.RemoveRange(options);
        _dbContext.PackageSelectionRules.RemoveRange(package.SelectionRules);
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
            .Include(package => package.Addons)
            .Include(package => package.SelectionRules)
                .ThenInclude(rule => rule.Options)
                    .ThenInclude(option => option.Meal);
    }

    private static List<PackageSize> BuildSizes(IEnumerable<PackageSizeRequest> requests, int packageId)
    {
        return requests.Select(request => new PackageSize
        {
            PackageId = packageId,
            Label = request.Label.Trim(),
            Subtitle = request.Subtitle.Trim(),
            PaxCount = request.PaxCount,
            Price = request.Price
        }).ToList();
    }

    private static List<PackageAddon> BuildAddons(IEnumerable<PackageAddonRequest> requests, int packageId)
    {
        return requests.Select(request => new PackageAddon
        {
            PackageId = packageId,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            IsAvailable = request.IsAvailable
        }).ToList();
    }

    private static bool TryBuildSelectionRules(
        IEnumerable<PackageSelectionRuleRequest> requests,
        int packageId,
        out List<PackageSelectionRule> rules,
        out string? errorMessage)
    {
        rules = new List<PackageSelectionRule>();
        errorMessage = null;

        foreach (var request in requests)
        {
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
                PackageId = packageId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                SelectionType = selectionType,
                AllowedMealType = allowedMealType,
                MinSelections = request.MinSelections,
                MaxSelections = request.MaxSelections,
                IsRequired = request.IsRequired,
                DisplayOrder = request.DisplayOrder,
                Options = request.Options.Select(option => new PackageSelectionOption
                {
                    MealId = option.MealId,
                    AdditionalPrice = option.AdditionalPrice,
                    IsDefault = option.IsDefault
                }).ToList()
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
                    Price = size.Price
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
                .ToList(),

            SelectionRules = package.SelectionRules
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
                    Options = rule.Options
                        .OrderBy(option => option.Meal.Name)
                        .Select(option => new PackageSelectionOptionDto
                        {
                            Id = option.Id,
                            MealId = option.MealId,
                            MealName = option.Meal?.Name ?? string.Empty,
                            AdditionalPrice = option.AdditionalPrice,
                            IsDefault = option.IsDefault
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}