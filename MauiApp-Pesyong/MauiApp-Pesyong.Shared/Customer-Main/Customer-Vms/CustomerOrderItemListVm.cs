using CMS.Contracts.Customer.Orders;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class CustomerOrderListItemVm
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderedAtUtc { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string DeliveryTimeSlot { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public int ItemCount { get; set; }

    public static CustomerOrderListItemVm FromDto(CustomerOrderListItemDto dto)
    {
        return new CustomerOrderListItemVm
        {
            Id = dto.Id,
            OrderNumber = dto.OrderNumber,
            OrderedAtUtc = dto.OrderedAtUtc,
            DeliveryDate = dto.DeliveryDate,
            DeliveryTimeSlot = dto.DeliveryTimeSlot,
            Status = dto.Status,
            PaymentStatus = dto.PaymentStatus,
            GrandTotal = dto.GrandTotal,
            ItemCount = dto.ItemCount
        };
    }
}