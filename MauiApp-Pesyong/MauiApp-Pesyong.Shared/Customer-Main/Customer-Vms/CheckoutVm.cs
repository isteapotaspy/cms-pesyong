using CMS.Contracts.Customer.Orders;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class CheckoutVm
{
    public ContactInfoVm ContactInfo { get; set; } = new();
    public DeliveryAddressVm DeliveryAddress { get; set; } = new();
    public DeliveryScheduleVm DeliverySchedule { get; set; } = new();
    public string PaymentMethod { get; set; } = "GCash";
    public string PromoCode { get; set; } = string.Empty;

    public List<string> AvailableTimeSlots { get; set; } = new()
    {
        "10:00 AM",
        "10:30 AM",
        "11:00 AM",
        "11:30 AM",
        "12:00 PM",
        "12:30 PM",
        "01:00 PM",
        "01:30 PM"
    };

    public List<string> AvailablePaymentMethods { get; set; } = new()
    {
        "GCash",
        "Credit Card",
        "Cash on Delivery"
    };

    public List<CartLineUiModel> Items { get; set; } = new();

    public decimal SubTotal => Items.Sum(x => x.LineTotal);
    public decimal DeliveryFee { get; set; } = 0m;
    public decimal TaxAmount => Math.Round(SubTotal * 0.12m, 2);
    public decimal GrandTotal => SubTotal + DeliveryFee + TaxAmount;

    public static CheckoutVm FromCart(IEnumerable<CartLineUiModel> cartItems)
    {
        return new CheckoutVm
        {
            ContactInfo = new ContactInfoVm
            {
                FullName = "Juan Dela Cruz",
                EmailAddress = "juan.dc@email.com",
                MobileNumber = "0917 123 4567"
            },
            DeliveryAddress = new DeliveryAddressVm
            {
                StreetAddress = "Unit 402, Blue Residences",
                City = "Quezon City",
                Barangay = "Loyola Heights",
                Landmark = "Near Ateneo Gate 3, call upon arrival"
            },
            DeliverySchedule = new DeliveryScheduleVm
            {
                DeliveryDate = DateTime.Today,
                TimeSlot = "11:30 AM"
            },
            Items = cartItems.ToList()
        };
    }

    public PlaceOrderRequest ToRequest()
    {
        return new PlaceOrderRequest
        {
            ContactInfo = new ContactInfoDto
            {
                FullName = ContactInfo.FullName,
                EmailAddress = ContactInfo.EmailAddress,
                MobileNumber = ContactInfo.MobileNumber
            },
            DeliveryAddress = new DeliveryAddressDto
            {
                StreetAddress = DeliveryAddress.StreetAddress,
                City = DeliveryAddress.City,
                Barangay = DeliveryAddress.Barangay,
                Landmark = DeliveryAddress.Landmark,
                Latitude = DeliveryAddress.Latitude,
                Longitude = DeliveryAddress.Longitude
            },
            DeliverySchedule = new DeliveryScheduleDto
            {
                DeliveryDate = DeliverySchedule.DeliveryDate,
                TimeSlot = DeliverySchedule.TimeSlot
            },
            PaymentMethod = PaymentMethod,
            PromoCode = PromoCode,
            DeliveryFee = DeliveryFee,
            TaxAmount = TaxAmount,
            GrandTotal = GrandTotal,
            Items = Items.Select(x => new OrderItemDto
            {
                PackageId = x.Id,
                PackageTitle = x.Name,
                SizeLabel = x.Notes,
                Notes = x.Notes,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            }).ToList()
        };
    }
}

public class ContactInfoVm
{
    public string FullName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
}

public class DeliveryAddressVm
{
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Barangay { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class DeliveryScheduleVm
{
    public DateTime DeliveryDate { get; set; } = DateTime.Today;
    public string TimeSlot { get; set; } = "11:30 AM";
}