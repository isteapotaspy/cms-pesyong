using System.Collections.ObjectModel;
using CMS.Contracts.Admin.Orders;
using CMS.Contracts.Customer.Orders;
using CMS.Domain.Entities.User;
using CMS.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModels.Orders;

public partial class OrderItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private DateTime dateCreated;

    [ObservableProperty]
    private DateTime? dateUpdated;

    [ObservableProperty]
    private string orderNumber = string.Empty;

    [ObservableProperty]
    private int customerProfileId;

    [ObservableProperty]
    private int? addressId;

    [ObservableProperty]
    private DateTime orderedAtUtc = DateTime.UtcNow;

    [ObservableProperty]
    private DateTime deliveryDate = DateTime.Today;

    [ObservableProperty]
    private string deliveryTimeSlot = string.Empty;

    [ObservableProperty]
    private string status = "Pending";

    [ObservableProperty]
    private string paymentMethod = string.Empty;

    [ObservableProperty]
    private string paymentStatus = "Pending";

    [ObservableProperty]
    private string contactNameSnapshot = string.Empty;

    [ObservableProperty]
    private string contactEmailSnapshot = string.Empty;

    [ObservableProperty]
    private string contactMobileSnapshot = string.Empty;

    [ObservableProperty]
    private string customerNotes = string.Empty;

    [ObservableProperty]
    private string specialInstructions = string.Empty;

    [ObservableProperty]
    private string promoCodeApplied = string.Empty;

    [ObservableProperty]
    private decimal subTotal;

    [ObservableProperty]
    private decimal deliveryFee;

    [ObservableProperty]
    private decimal taxAmount;

    [ObservableProperty]
    private decimal discountAmount;

    [ObservableProperty]
    private decimal grandTotal;

    public ObservableCollection<OrderLineItemVM> Items { get; } = new();

    public static OrderItemVM FromDto(OrderDto dto)
    {
        var vm = new OrderItemVM();
        vm.CopyFrom(dto);
        return vm;
    }

    public void CopyFrom(OrderDto dto)
    {
        Id = dto.Id;
        DateCreated = dto.DateCreated;
        DateUpdated = dto.DateUpdated;

        OrderNumber = dto.OrderNumber;

        CustomerProfileId = dto.CustomerProfileId;
        AddressId = dto.AddressId;

        OrderedAtUtc = dto.OrderedAtUtc;
        DeliveryDate = dto.DeliveryDate;
        DeliveryTimeSlot = dto.DeliveryTimeSlot;

        Status = dto.Status;
        PaymentMethod = dto.PaymentMethod;
        PaymentStatus = dto.PaymentStatus;

        ContactNameSnapshot = dto.ContactNameSnapshot;
        ContactEmailSnapshot = dto.ContactEmailSnapshot;
        ContactMobileSnapshot = dto.ContactMobileSnapshot;

        CustomerNotes = dto.CustomerNotes;
        SpecialInstructions = dto.SpecialInstructions;

        PromoCodeApplied = dto.PromoCodeApplied;

        SubTotal = dto.SubTotal;
        DeliveryFee = dto.DeliveryFee;
        TaxAmount = dto.TaxAmount;
        DiscountAmount = dto.DiscountAmount;
        GrandTotal = dto.GrandTotal;

        Items.Clear();
        foreach (var item in dto.Items)
            Items.Add(OrderLineItemVM.FromDto(item));
    }

    public void CopyFrom(OrderItemVM other)
    {
        Id = other.Id;
        DateCreated = other.DateCreated;
        DateUpdated = other.DateUpdated;

        OrderNumber = other.OrderNumber;

        CustomerProfileId = other.CustomerProfileId;
        AddressId = other.AddressId;

        OrderedAtUtc = other.OrderedAtUtc;
        DeliveryDate = other.DeliveryDate;
        DeliveryTimeSlot = other.DeliveryTimeSlot;

        Status = other.Status;
        PaymentMethod = other.PaymentMethod;
        PaymentStatus = other.PaymentStatus;

        ContactNameSnapshot = other.ContactNameSnapshot;
        ContactEmailSnapshot = other.ContactEmailSnapshot;
        ContactMobileSnapshot = other.ContactMobileSnapshot;

        CustomerNotes = other.CustomerNotes;
        SpecialInstructions = other.SpecialInstructions;

        PromoCodeApplied = other.PromoCodeApplied;

        SubTotal = other.SubTotal;
        DeliveryFee = other.DeliveryFee;
        TaxAmount = other.TaxAmount;
        DiscountAmount = other.DiscountAmount;
        GrandTotal = other.GrandTotal;

        Items.Clear();
        foreach (var item in other.Items)
        {
            var copy = new OrderLineItemVM();
            copy.CopyFrom(item);
            Items.Add(copy);
        }
    }

    public CreateOrderRequest ToCreateRequest()
    {
        return new CreateOrderRequest
        {
            OrderNumber = OrderNumber,

            CustomerProfileId = CustomerProfileId,
            AddressId = AddressId,

            OrderedAtUtc = OrderedAtUtc,
            DeliveryDate = DeliveryDate,
            DeliveryTimeSlot = DeliveryTimeSlot,

            Status = Status,
            PaymentMethod = PaymentMethod,
            PaymentStatus = PaymentStatus,

            ContactNameSnapshot = ContactNameSnapshot,
            ContactEmailSnapshot = ContactEmailSnapshot,
            ContactMobileSnapshot = ContactMobileSnapshot,

            CustomerNotes = CustomerNotes,
            SpecialInstructions = SpecialInstructions,

            PromoCodeApplied = PromoCodeApplied,

            SubTotal = SubTotal,
            DeliveryFee = DeliveryFee,
            TaxAmount = TaxAmount,
            DiscountAmount = DiscountAmount,
            GrandTotal = GrandTotal,

            Items = Items.Select(x => x.ToRequest()).ToList()
        };
    }

    public UpdateOrderRequest ToUpdateRequest()
    {
        return new UpdateOrderRequest
        {
            OrderNumber = OrderNumber,

            CustomerProfileId = CustomerProfileId,
            AddressId = AddressId,

            OrderedAtUtc = OrderedAtUtc,
            DeliveryDate = DeliveryDate,
            DeliveryTimeSlot = DeliveryTimeSlot,

            Status = Status,
            PaymentMethod = PaymentMethod,
            PaymentStatus = PaymentStatus,

            ContactNameSnapshot = ContactNameSnapshot,
            ContactEmailSnapshot = ContactEmailSnapshot,
            ContactMobileSnapshot = ContactMobileSnapshot,

            CustomerNotes = CustomerNotes,
            SpecialInstructions = SpecialInstructions,

            PromoCodeApplied = PromoCodeApplied,

            SubTotal = SubTotal,
            DeliveryFee = DeliveryFee,
            TaxAmount = TaxAmount,
            DiscountAmount = DiscountAmount,
            GrandTotal = GrandTotal,

            Items = Items.Select(x => x.ToRequest()).ToList()
        };
    }
}