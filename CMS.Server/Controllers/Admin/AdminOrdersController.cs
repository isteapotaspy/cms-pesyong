using CMS.Contracts.Admin.Orders;
using CMS.Contracts.Customer.Orders;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Enums;
using CMS.Infrastructure.Persistence;
using CMS.Server.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Controllers;

[ApiController]
[Route("api/admin/orders")]
public sealed class AdminOrdersController : ControllerBase
{
    private readonly CmsDbContext _db;
    private readonly IHubContext<OrderHub> _hubContext;

    public AdminOrdersController(
        CmsDbContext db,
        IHubContext<OrderHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await OrdersWithChildren()
            .AsNoTracking()
            .OrderByDescending(x => x.OrderedAtUtc)
            .ToListAsync();

        return Ok(orders.Select(MapToDto).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await OrdersWithChildren()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order is null)
            return NotFound();

        return Ok(MapToDto(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request)
    {
        var validation = ValidateAndParseEnums(
            request.Status,
            request.PaymentMethod,
            request.PaymentStatus,
            out var orderStatus,
            out var paymentMethod,
            out var paymentStatus);

        if (validation is not null)
            return validation;

        if (request.CustomerProfileId <= 0)
            return BadRequest("CustomerProfileId is required.");

        var order = new Order
        {
            OrderNumber = string.IsNullOrWhiteSpace(request.OrderNumber)
                ? GenerateOrderNumber()
                : request.OrderNumber.Trim(),

            CustomerProfileId = request.CustomerProfileId,
            AddressId = request.AddressId,

            OrderedAtUtc = request.OrderedAtUtc,
            DeliveryDate = request.DeliveryDate,
            DeliveryTimeSlot = request.DeliveryTimeSlot,

            Status = orderStatus,
            PaymentMethod = paymentMethod,
            PaymentStatus = paymentStatus,

            ContactNameSnapshot = request.ContactNameSnapshot,
            ContactEmailSnapshot = request.ContactEmailSnapshot,
            ContactMobileSnapshot = request.ContactMobileSnapshot,

            CustomerNotes = request.CustomerNotes,
            SpecialInstructions = request.SpecialInstructions,

            PromoCodeApplied = request.PromoCodeApplied,

            SubTotal = request.SubTotal,
            DeliveryFee = request.DeliveryFee,
            TaxAmount = request.TaxAmount,
            DiscountAmount = request.DiscountAmount,
            GrandTotal = request.GrandTotal,

            Items = request.Items.Select(MapToEntity).ToList()
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        var created = await OrdersWithChildren()
            .AsNoTracking()
            .FirstAsync(x => x.Id == order.Id);

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderDto>> Update(int id, UpdateOrderRequest request)
    {
        var order = await OrdersWithChildren()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order is null)
            return NotFound();

        var validation = ValidateAndParseEnums(
            request.Status,
            request.PaymentMethod,
            request.PaymentStatus,
            out var orderStatus,
            out var paymentMethod,
            out var paymentStatus);

        if (validation is not null)
            return validation;

        if (request.CustomerProfileId <= 0)
            return BadRequest("CustomerProfileId is required.");

        order.OrderNumber = string.IsNullOrWhiteSpace(request.OrderNumber)
            ? order.OrderNumber
            : request.OrderNumber.Trim();

        order.CustomerProfileId = request.CustomerProfileId;
        order.AddressId = request.AddressId;

        order.OrderedAtUtc = request.OrderedAtUtc;
        order.DeliveryDate = request.DeliveryDate;
        order.DeliveryTimeSlot = request.DeliveryTimeSlot;

        order.Status = orderStatus;
        order.PaymentMethod = paymentMethod;
        order.PaymentStatus = paymentStatus;

        order.ContactNameSnapshot = request.ContactNameSnapshot;
        order.ContactEmailSnapshot = request.ContactEmailSnapshot;
        order.ContactMobileSnapshot = request.ContactMobileSnapshot;

        order.CustomerNotes = request.CustomerNotes;
        order.SpecialInstructions = request.SpecialInstructions;

        order.PromoCodeApplied = request.PromoCodeApplied;

        order.SubTotal = request.SubTotal;
        order.DeliveryFee = request.DeliveryFee;
        order.TaxAmount = request.TaxAmount;
        order.DiscountAmount = request.DiscountAmount;
        order.GrandTotal = request.GrandTotal;

        ReplaceOrderItems(order, request.Items);

        await _db.SaveChangesAsync();

        await BroadcastOrderStatusUpdatedAsync(order);

        var updated = await OrdersWithChildren()
            .AsNoTracking()
            .FirstAsync(x => x.Id == id);

        return Ok(MapToDto(updated));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await OrdersWithChildren()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order is null)
            return NotFound();

        foreach (var item in order.Items)
        {
            _db.Set<OrderItemMealSelection>().RemoveRange(item.MealSelections);
            _db.Set<OrderItemAddonSelection>().RemoveRange(item.AddonSelections);
        }

        _db.Set<OrderItem>().RemoveRange(order.Items);
        _db.Orders.Remove(order);

        await _db.SaveChangesAsync();

        return NoContent();
    }

    private async Task BroadcastOrderStatusUpdatedAsync(Order order)
    {
        var evt = new OrderStatusUpdatedEvent
        {
            OrderId = order.Id,
            CustomerProfileId = order.CustomerProfileId,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _hubContext.Clients.Group(OrderHub.GetOrderGroup(order.Id))
            .SendAsync("OrderStatusUpdated", evt);

        await _hubContext.Clients.Group(OrderHub.GetCustomerGroup(order.CustomerProfileId))
            .SendAsync("OrderStatusUpdated", evt);
    }

    private IQueryable<Order> OrdersWithChildren()
    {
        return _db.Orders
            .Include(x => x.Items)
                .ThenInclude(x => x.MealSelections)
            .Include(x => x.Items)
                .ThenInclude(x => x.AddonSelections);
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            DateCreated = order.CreatedAtUtc,
            DateUpdated = order.UpdatedAtUtc,
            OrderNumber = order.OrderNumber,
            CustomerProfileId = order.CustomerProfileId,
            AddressId = order.AddressId,
            OrderedAtUtc = order.OrderedAtUtc,
            DeliveryDate = order.DeliveryDate,
            DeliveryTimeSlot = order.DeliveryTimeSlot,
            Status = order.Status.ToString(),
            PaymentMethod = order.PaymentMethod.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            ContactNameSnapshot = order.ContactNameSnapshot,
            ContactEmailSnapshot = order.ContactEmailSnapshot,
            ContactMobileSnapshot = order.ContactMobileSnapshot,
            CustomerNotes = order.CustomerNotes,
            SpecialInstructions = order.SpecialInstructions,
            PromoCodeApplied = order.PromoCodeApplied,
            SubTotal = order.SubTotal,
            DeliveryFee = order.DeliveryFee,
            TaxAmount = order.TaxAmount,
            DiscountAmount = order.DiscountAmount,
            GrandTotal = order.GrandTotal,
            Items = order.Items.Select(MapToDto).ToList()
        };
    }

    private static OrderLineItemDto MapToDto(OrderItem item)
    {
        return new OrderLineItemDto
        {
            Id = item.Id,
            OrderId = item.OrderId,
            ItemType = item.ItemType.ToString(),
            PackageId = item.PackageId,
            PackageSizeId = item.PackageSizeId,
            MealId = item.MealId,
            PackageTitleSnapshot = item.PackageTitleSnapshot,
            SizeLabelSnapshot = item.SizeLabelSnapshot,
            BaseUnitPrice = item.BaseUnitPrice,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            LineTotal = item.LineTotal,
            MealSelections = item.MealSelections.Select(MapToDto).ToList(),
            AddonSelections = item.AddonSelections.Select(MapToDto).ToList()
        };
    }

    private static Contracts.Admin.Orders.OrderItemMealSelectionRequestDto MapToDto(OrderItemMealSelection selection)
    {
        return new Contracts.Admin.Orders.OrderItemMealSelectionRequestDto
        {
            Id = selection.Id,
            OrderItemId = selection.OrderItemId,
            PackageSelectionRuleId = selection.PackageSelectionRuleId,
            MealId = selection.MealId,
            RuleTitleSnapshot = selection.RuleTitleSnapshot,
            MealNameSnapshot = selection.MealNameSnapshot,
            AdditionalPrice = selection.AdditionalPrice
        };
    }

    private static OrderItemAddonSelectionDto MapToDto(OrderItemAddonSelection selection)
    {
        return new OrderItemAddonSelectionDto
        {
            Id = selection.Id,
            OrderItemId = selection.OrderItemId,
            PackageAddonId = selection.PackageAddonId,
            AddonNameSnapshot = selection.AddonNameSnapshot,
            AdditionalPrice = selection.AdditionalPrice
        };
    }

    private static OrderItem MapToEntity(OrderLineItemRequest request)
    {
        if (!Enum.TryParse<OrderItemType>(request.ItemType, true, out var itemType))
            itemType = OrderItemType.Package;

        return new OrderItem
        {
            ItemType = itemType,
            PackageId = request.PackageId,
            PackageSizeId = request.PackageSizeId,
            MealId = request.MealId,
            PackageTitleSnapshot = request.PackageTitleSnapshot,
            SizeLabelSnapshot = request.SizeLabelSnapshot,
            BaseUnitPrice = request.BaseUnitPrice,
            Quantity = request.Quantity <= 0 ? 1 : request.Quantity,
            MealSelections = request.MealSelections.Select(MapToEntity).ToList(),
            AddonSelections = request.AddonSelections.Select(MapToEntity).ToList()
        };
    }

    private static OrderItemMealSelection MapToEntity(OrderItemMealSelectionRequest request)
    {
        return new OrderItemMealSelection
        {
            PackageSelectionRuleId = request.PackageSelectionRuleId,
            MealId = request.MealId,
            RuleTitleSnapshot = request.RuleTitleSnapshot,
            MealNameSnapshot = request.MealNameSnapshot,
            AdditionalPrice = request.AdditionalPrice
        };
    }

    private static OrderItemAddonSelection MapToEntity(OrderItemAddonSelectionRequest request)
    {
        return new OrderItemAddonSelection
        {
            PackageAddonId = request.PackageAddonId,
            AddonNameSnapshot = request.AddonNameSnapshot,
            AdditionalPrice = request.AdditionalPrice
        };
    }

    private void ReplaceOrderItems(Order order, List<OrderLineItemRequest> items)
    {
        foreach (var existingItem in order.Items)
        {
            _db.Set<OrderItemMealSelection>().RemoveRange(existingItem.MealSelections);
            _db.Set<OrderItemAddonSelection>().RemoveRange(existingItem.AddonSelections);
        }

        _db.Set<OrderItem>().RemoveRange(order.Items);

        order.Items.Clear();

        foreach (var item in items)
            order.Items.Add(MapToEntity(item));
    }

    private BadRequestObjectResult? ValidateAndParseEnums(
        string statusValue,
        string paymentMethodValue,
        string paymentStatusValue,
        out OrderStatus status,
        out PaymentMethod paymentMethod,
        out PaymentStatus paymentStatus)
    {
        status = default;
        paymentMethod = default;
        paymentStatus = default;

        if (!Enum.TryParse(statusValue, true, out status))
        {
            return BadRequest(
                $"Invalid order status. Valid values: {string.Join(", ", Enum.GetNames<OrderStatus>())}");
        }

        if (!Enum.TryParse(paymentMethodValue, true, out paymentMethod))
        {
            return BadRequest(
                $"Invalid payment method. Valid values: {string.Join(", ", Enum.GetNames<PaymentMethod>())}");
        }

        if (!Enum.TryParse(paymentStatusValue, true, out paymentStatus))
        {
            return BadRequest(
                $"Invalid payment status. Valid values: {string.Join(", ", Enum.GetNames<PaymentStatus>())}");
        }

        return null;
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}