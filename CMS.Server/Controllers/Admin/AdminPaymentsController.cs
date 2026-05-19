using CMS.Contracts.Admin.Payment;
using CMS.Domain.Entities.Payment;
using CMS.Domain.Enums;
using CMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Controllers.Admin;

[ApiController]
[Route("api/admin/payments")]
public sealed class AdminPaymentsController : ControllerBase
{
    private readonly CmsDbContext _db;

    public AdminPaymentsController(CmsDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<PaymentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var payments = await _db.Payments
            .AsNoTracking()
            .OrderByDescending(x => x.TimestampUtc)
            .Select(x => new PaymentDto
            {
                Id = x.Id,
                OrderId = x.OrderId,
                ExternalReference = x.ExternalReference,
                PaymentMethod = x.PaymentMethod.ToString(),
                PaymentStatus = x.PaymentStatus.ToString(),
                TimestampUtc = x.TimestampUtc,
                Amount = x.Amount,
                Description = x.Description,
                DateCreated = x.CreatedAtUtc,
                DateUpdated = x.UpdatedAtUtc,
                OrderDisplay = "Order #" + x.OrderId
            })
            .ToListAsync(cancellationToken);

        return Ok(payments);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PaymentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var payment = await _db.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (payment is null)
            return NotFound();

        return Ok(ToDto(payment));
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> Create(
        CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        if (!await _db.Orders.AnyAsync(x => x.Id == request.OrderId, cancellationToken))
            return BadRequest($"Order with ID {request.OrderId} does not exist.");

        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
            return BadRequest($"Invalid payment method: {request.PaymentMethod}");

        if (!Enum.TryParse<PaymentStatus>(request.PaymentStatus, true, out var paymentStatus))
            return BadRequest($"Invalid payment status: {request.PaymentStatus}");

        var now = DateTime.UtcNow;

        var payment = new Payment
        {
            OrderId = request.OrderId,
            ExternalReference = request.ExternalReference,
            PaymentMethod = paymentMethod,
            PaymentStatus = paymentStatus,
            TimestampUtc = NormalizeUtc(request.TimestampUtc),
            Amount = request.Amount,
            Description = request.Description,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(cancellationToken);

        var dto = ToDto(payment);

        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PaymentDto>> Update(
        int id,
        UpdatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var payment = await _db.Payments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (payment is null)
            return NotFound();

        if (!await _db.Orders.AnyAsync(x => x.Id == request.OrderId, cancellationToken))
            return BadRequest($"Order with ID {request.OrderId} does not exist.");

        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
            return BadRequest($"Invalid payment method: {request.PaymentMethod}");

        if (!Enum.TryParse<PaymentStatus>(request.PaymentStatus, true, out var paymentStatus))
            return BadRequest($"Invalid payment status: {request.PaymentStatus}");

        payment.OrderId = request.OrderId;
        payment.ExternalReference = request.ExternalReference;
        payment.PaymentMethod = paymentMethod;
        payment.PaymentStatus = paymentStatus;
        payment.TimestampUtc = NormalizeUtc(request.TimestampUtc);
        payment.Amount = request.Amount;
        payment.Description = request.Description;
        payment.UpdatedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return Ok(ToDto(payment));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var payment = await _db.Payments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (payment is null)
            return NotFound();

        _db.Payments.Remove(payment);
        await _db.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private static PaymentDto ToDto(Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            ExternalReference = payment.ExternalReference,
            PaymentMethod = payment.PaymentMethod.ToString(),
            PaymentStatus = payment.PaymentStatus.ToString(),
            TimestampUtc = payment.TimestampUtc,
            Amount = payment.Amount,
            Description = payment.Description,
            DateCreated = payment.CreatedAtUtc,
            DateUpdated = payment.UpdatedAtUtc,
            OrderDisplay = "Order #" + payment.OrderId
        };
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        if (value == default)
            return DateTime.UtcNow;

        if (value.Kind == DateTimeKind.Utc)
            return value;

        if (value.Kind == DateTimeKind.Local)
            return value.ToUniversalTime();

        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}