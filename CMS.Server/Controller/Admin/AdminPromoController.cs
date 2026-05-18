using CMS.Contracts.Admin.Promos;
using CMS.Domain.Entities.Payment;
using CMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Controllers.Admin;

[ApiController]
[Route("api/admin/promos")]
public sealed class PromosController : ControllerBase
{
    private readonly CmsDbContext _db;

    public PromosController(CmsDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<PromoDto>>> GetAll()
    {
        var now = DateTime.UtcNow;

        var promos = await _db.Promos
            .AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new PromoDto
            {
                Id = x.Id,
                DateCreated = x.CreatedAtUtc,
                DateUpdated = x.UpdatedAtUtc,
                Code = x.Code,
                Description = x.Description,
                DiscountPercentageValue = x.DiscountPercentageValue,
                MinimumOrderAmount = x.MinimumOrderAmount,
                UsageLimit = x.UsageLimit,
                UsedCount = x.UsedCount,
                ValidFromUtc = x.ValidFromUtc,
                ValidUntilUtc = x.ValidUntilUtc,
                IsActive =
                    now >= x.ValidFromUtc &&
                    now <= x.ValidUntilUtc &&
                    (!x.UsageLimit.HasValue || x.UsedCount < x.UsageLimit.Value)
            })
            .ToListAsync();

        return Ok(promos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PromoDto>> GetById(int id)
    {
        var now = DateTime.UtcNow;

        var promo = await _db.Promos
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new PromoDto
            {
                Id = x.Id,
                DateCreated = x.CreatedAtUtc,
                DateUpdated = x.UpdatedAtUtc,
                Code = x.Code,
                Description = x.Description,
                DiscountPercentageValue = x.DiscountPercentageValue,
                MinimumOrderAmount = x.MinimumOrderAmount,
                UsageLimit = x.UsageLimit,
                UsedCount = x.UsedCount,
                ValidFromUtc = x.ValidFromUtc,
                ValidUntilUtc = x.ValidUntilUtc,
                IsActive =
                    now >= x.ValidFromUtc &&
                    now <= x.ValidUntilUtc &&
                    (!x.UsageLimit.HasValue || x.UsedCount < x.UsageLimit.Value)
            })
            .FirstOrDefaultAsync();

        if (promo is null)
            return NotFound();

        return Ok(promo);
    }

    [HttpPost]
    public async Task<ActionResult<PromoDto>> Create(CreatePromoRequest request)
    {
        var validationError = ValidateRequest(request.Code,
            request.DiscountPercentageValue,
            request.MinimumOrderAmount,
            request.UsageLimit,
            request.UsedCount,
            request.ValidFromUtc,
            request.ValidUntilUtc);

        if (validationError is not null)
            return BadRequest(validationError);

        var promo = new Promo
        {
            Code = request.Code.Trim(),
            Description = request.Description.Trim(),
            DiscountPercentageValue = request.DiscountPercentageValue,
            MinimumOrderAmount = request.MinimumOrderAmount,
            UsageLimit = request.UsageLimit,
            UsedCount = request.UsedCount,
            ValidFromUtc = request.ValidFromUtc,
            ValidUntilUtc = request.ValidUntilUtc
        };

        _db.Promos.Add(promo);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = promo.Id },
            ToDto(promo));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PromoDto>> Update(int id, UpdatePromoRequest request)
    {
        var promo = await _db.Promos.FirstOrDefaultAsync(x => x.Id == id);

        if (promo is null)
            return NotFound();

        var validationError = ValidateRequest(request.Code,
            request.DiscountPercentageValue,
            request.MinimumOrderAmount,
            request.UsageLimit,
            request.UsedCount,
            request.ValidFromUtc,
            request.ValidUntilUtc);

        if (validationError is not null)
            return BadRequest(validationError);

        promo.Code = request.Code.Trim();
        promo.Description = request.Description.Trim();
        promo.DiscountPercentageValue = request.DiscountPercentageValue;
        promo.MinimumOrderAmount = request.MinimumOrderAmount;
        promo.UsageLimit = request.UsageLimit;
        promo.UsedCount = request.UsedCount;
        promo.ValidFromUtc = request.ValidFromUtc;
        promo.ValidUntilUtc = request.ValidUntilUtc;

        await _db.SaveChangesAsync();

        return Ok(ToDto(promo));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var promo = await _db.Promos.FirstOrDefaultAsync(x => x.Id == id);

        if (promo is null)
            return NotFound();

        _db.Promos.Remove(promo);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static PromoDto ToDto(Promo promo)
    {
        var now = DateTime.UtcNow;

        return new PromoDto
        {
            Id = promo.Id,
            DateCreated = promo.CreatedAtUtc,
            DateUpdated = promo.UpdatedAtUtc,
            Code = promo.Code,
            Description = promo.Description,
            DiscountPercentageValue = promo.DiscountPercentageValue,
            MinimumOrderAmount = promo.MinimumOrderAmount,
            UsageLimit = promo.UsageLimit,
            UsedCount = promo.UsedCount,
            ValidFromUtc = promo.ValidFromUtc,
            ValidUntilUtc = promo.ValidUntilUtc,
            IsActive =
                now >= promo.ValidFromUtc &&
                now <= promo.ValidUntilUtc &&
                (!promo.UsageLimit.HasValue || promo.UsedCount < promo.UsageLimit.Value)
        };
    }

    private static string? ValidateRequest(
        string code,
        decimal discountPercentageValue,
        decimal? minimumOrderAmount,
        int? usageLimit,
        int usedCount,
        DateTime validFromUtc,
        DateTime validUntilUtc)
    {
        if (string.IsNullOrWhiteSpace(code))
            return "Promo code is required.";

        if (discountPercentageValue < 0 || discountPercentageValue > 100)
            return "Discount percentage must be between 0 and 100.";

        if (minimumOrderAmount.HasValue && minimumOrderAmount.Value < 0)
            return "Minimum order amount cannot be negative.";

        if (usageLimit.HasValue && usageLimit.Value < 0)
            return "Usage limit cannot be negative.";

        if (usedCount < 0)
            return "Used count cannot be negative.";

        if (validUntilUtc < validFromUtc)
            return "Valid Until must be later than or equal to Valid From.";

        return null;
    }
}