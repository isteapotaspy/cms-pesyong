using CMS.Contracts.Admin.Meals;
using CMS.Domain.Entities.Menu;
using CMS.Domain.Enums;
using CMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Controllers.Admin;

[ApiController]
[Route("api/admin/meals")]
public sealed class AdminMealsController : ControllerBase
{
    private readonly CmsDbContext _db;

    public AdminMealsController(CmsDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<MealDto>>> GetMeals()
    {
        var meals = await _db.Meals
            .AsNoTracking()
            .Include(x => x.MenuCategory)
            .OrderBy(x => x.Name)
            .Select(x => new MealDto
            {
                Id = x.Id,
                CategoryId = x.MenuCategoryId,
                CategoryName = x.MenuCategory.Name,

                Name = x.Name,
                Description = x.Description,
                MealType = x.MealType.ToString(),

                BasePrice = x.BasePrice,
                StockQuantity = x.StockQuantity,
                MinOrderQuantity = x.MinOrderQuantity,

                ImageUrl = x.ImageUrl,
                IsAvailable = x.IsAvailable,
                IsViandOption = x.IsViandOption
            })
            .ToListAsync();

        return Ok(meals);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MealDto>> GetMealById(int id)
    {
        var meal = await _db.Meals
            .AsNoTracking()
            .Include(x => x.MenuCategory)
            .Where(x => x.Id == id)
            .Select(x => new MealDto
            {
                Id = x.Id,
                CategoryId = x.MenuCategoryId,
                CategoryName = x.MenuCategory.Name,

                Name = x.Name,
                Description = x.Description,
                MealType = x.MealType.ToString(),

                BasePrice = x.BasePrice,
                StockQuantity = x.StockQuantity,
                MinOrderQuantity = x.MinOrderQuantity,

                ImageUrl = x.ImageUrl,
                IsAvailable = x.IsAvailable,
                IsViandOption = x.IsViandOption
            })
            .FirstOrDefaultAsync();

        if (meal is null)
        {
            return NotFound();
        }

        return Ok(meal);
    }

    [HttpPost]
    public async Task<ActionResult<MealDto>> CreateMeal(CreateMealRequest request)
    {
        if (!Enum.TryParse<MealType>(request.MealType, true, out var mealType))
        {
            return BadRequest($"Invalid meal type: {request.MealType}");
        }

        var meal = new Meal
        {
            MenuCategoryId = request.CategoryId,

            Name = request.Name,
            Description = request.Description,
            MealType = mealType,

            BasePrice = request.BasePrice,
            StockQuantity = request.StockQuantity,
            MinOrderQuantity = request.MinOrderQuantity,

            ImageUrl = request.ImageUrl,
            IsAvailable = request.IsAvailable,
            IsViandOption = request.IsViandOption
        };

        _db.Meals.Add(meal);
        await _db.SaveChangesAsync();

        var createdMeal = await GetMealDtoById(meal.Id);

        return CreatedAtAction(
            nameof(GetMealById),
            new { id = meal.Id },
            createdMeal);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MealDto>> UpdateMeal(
        int id,
        UpdateMealRequest request)
    {
        var meal = await _db.Meals.FirstOrDefaultAsync(x => x.Id == id);

        if (meal is null)
        {
            return NotFound();
        }

        if (!Enum.TryParse<MealType>(request.MealType, true, out var mealType))
        {
            return BadRequest($"Invalid meal type: {request.MealType}");
        }

        meal.MenuCategoryId = request.CategoryId;

        meal.Name = request.Name;
        meal.Description = request.Description;
        meal.MealType = mealType;

        meal.BasePrice = request.BasePrice;
        meal.StockQuantity = request.StockQuantity;
        meal.MinOrderQuantity = request.MinOrderQuantity;

        meal.ImageUrl = request.ImageUrl;
        meal.IsAvailable = request.IsAvailable;
        meal.IsViandOption = request.IsViandOption;

        await _db.SaveChangesAsync();

        var updatedMeal = await GetMealDtoById(meal.Id);

        return Ok(updatedMeal);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMeal(int id)
    {
        var meal = await _db.Meals.FirstOrDefaultAsync(x => x.Id == id);

        if (meal is null)
        {
            return NotFound();
        }

        _db.Meals.Remove(meal);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private async Task<MealDto?> GetMealDtoById(int id)
    {
        return await _db.Meals
            .AsNoTracking()
            .Include(x => x.MenuCategory)
            .Where(x => x.Id == id)
            .Select(x => new MealDto
            {
                Id = x.Id,
                CategoryId = x.MenuCategoryId,
                CategoryName = x.MenuCategory.Name,

                Name = x.Name,
                Description = x.Description,
                MealType = x.MealType.ToString(),

                BasePrice = x.BasePrice,
                StockQuantity = x.StockQuantity,
                MinOrderQuantity = x.MinOrderQuantity,

                ImageUrl = x.ImageUrl,
                IsAvailable = x.IsAvailable,
                IsViandOption = x.IsViandOption
            })
            .FirstOrDefaultAsync();
    }
}