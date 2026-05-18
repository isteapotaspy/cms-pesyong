using CMS.Contracts.Admin.Deliveries;
using CMS.Domain.Entities.Payment;
using CMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Controllers.Admin;

[ApiController]
[Route("api/admin/deliveries")]
public class AdminDeliveriesController : ControllerBase
{
    private readonly CmsDbContext _db;

    public AdminDeliveriesController(CmsDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DeliveryDto>>> GetAll()
    {
        var deliveries = await _db.Deliveries
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => ToDto(x))
            .ToListAsync();

        return Ok(deliveries);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeliveryDto>> GetById(int id)
    {
        var delivery = await _db.Deliveries
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => ToDto(x))
            .FirstOrDefaultAsync();

        if (delivery is null)
            return NotFound();

        return Ok(delivery);
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryDto>> Create(CreateDeliveryRequest request)
    {
        if (request.OrderId <= 0)
            return BadRequest("OrderId is required.");

        var orderExists = await _db.Orders.AnyAsync(x => x.Id == request.OrderId);

        if (!orderExists)
            return BadRequest("The selected order does not exist.");

        var delivery = new Delivery
        {
            OrderId = request.OrderId,
            RiderUserId = request.RiderUserId,
            Status = request.Status,
            DeliveryAddressSnapshot = request.DeliveryAddressSnapshot,
            ShippingCost = request.ShippingCost,
            TrackingNumber = request.TrackingNumber,
            CurrentLocation = request.CurrentLocation,
            EstimatedDeliveryDateUtc = request.EstimatedDeliveryDateUtc,
            ActualDeliveryDateUtc = request.ActualDeliveryDateUtc
        };

        _db.Deliveries.Add(delivery);
        await _db.SaveChangesAsync();

        var dto = ToDto(delivery);

        return CreatedAtAction(nameof(GetById), new { id = delivery.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DeliveryDto>> Update(int id, UpdateDeliveryRequest request)
    {
        if (request.OrderId <= 0)
            return BadRequest("OrderId is required.");

        var delivery = await _db.Deliveries.FirstOrDefaultAsync(x => x.Id == id);

        if (delivery is null)
            return NotFound();

        var orderExists = await _db.Orders.AnyAsync(x => x.Id == request.OrderId);

        if (!orderExists)
            return BadRequest("The selected order does not exist.");

        delivery.OrderId = request.OrderId;
        delivery.RiderUserId = request.RiderUserId;
        delivery.Status = request.Status;
        delivery.DeliveryAddressSnapshot = request.DeliveryAddressSnapshot;
        delivery.ShippingCost = request.ShippingCost;
        delivery.TrackingNumber = request.TrackingNumber;
        delivery.CurrentLocation = request.CurrentLocation;
        delivery.EstimatedDeliveryDateUtc = request.EstimatedDeliveryDateUtc;
        delivery.ActualDeliveryDateUtc = request.ActualDeliveryDateUtc;
        delivery.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(ToDto(delivery));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var delivery = await _db.Deliveries.FirstOrDefaultAsync(x => x.Id == id);

        if (delivery is null)
            return NotFound();

        _db.Deliveries.Remove(delivery);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static DeliveryDto ToDto(Delivery delivery)
    {
        return new DeliveryDto
        {
            Id = delivery.Id,
            OrderId = delivery.OrderId,
            RiderUserId = delivery.RiderUserId,
            Status = delivery.Status,
            DeliveryAddressSnapshot = delivery.DeliveryAddressSnapshot,
            ShippingCost = delivery.ShippingCost,
            TrackingNumber = delivery.TrackingNumber,
            CurrentLocation = delivery.CurrentLocation,
            EstimatedDeliveryDateUtc = delivery.EstimatedDeliveryDateUtc,
            ActualDeliveryDateUtc = delivery.ActualDeliveryDateUtc,
            CreatedAtUtc = delivery.CreatedAtUtc,
            UpdatedAtUtc = delivery.UpdatedAtUtc
        };
    }
}