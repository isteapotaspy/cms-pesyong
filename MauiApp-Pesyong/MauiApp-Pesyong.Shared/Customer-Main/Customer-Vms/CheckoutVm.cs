using CMS.Contracts.Customer.Auth;
using CMS.Contracts.Customer.Orders;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;
namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class CheckoutVm
{
    public int? CustomerProfileId { get; set; }

    public ContactInfoVm ContactInfo { get; set; } = new();
    public DeliveryAddressVm DeliveryAddress { get; set; } = new();
    public DeliveryScheduleVm DeliverySchedule { get; set; } = new();

    public string PaymentMethod { get; set; } = "GCash";
    public string PromoCode { get; set; } = string.Empty;
    public string CustomerNotes { get; set; } = string.Empty;
    public string SpecialInstructions { get; set; } = string.Empty;

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

    public List<string> AvailableCities { get; set; } = new()
    {
        "Davao City",
        "Manila",
        "Makati",
        "Pasig",
        "Taguig",
        "Mandaluyong",
        "Marikina",
        "Pasay",
        "San Juan",
        "Caloocan"
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
                FullName = string.Empty,
                EmailAddress = string.Empty,
                MobileNumber = string.Empty
            },
            DeliveryAddress = new DeliveryAddressVm
            {
                StreetAddress = string.Empty,
                City = string.Empty,
                Barangay = string.Empty,
                Landmark = string.Empty
            },
            DeliverySchedule = new DeliveryScheduleVm
            {
                DeliveryDate = DateTime.Today,
                TimeSlot = "11:30 AM"
            },
            Items = cartItems.ToList()
        };
    }

    public void ApplyCustomerProfile(CustomerMeResponse profile)
    {
        CustomerProfileId = profile.CustomerProfileId;

        if (string.IsNullOrWhiteSpace(ContactInfo.FullName))
            ContactInfo.FullName = profile.FullName;

        if (string.IsNullOrWhiteSpace(ContactInfo.EmailAddress))
            ContactInfo.EmailAddress = profile.Email;

        if (string.IsNullOrWhiteSpace(ContactInfo.MobileNumber))
            ContactInfo.MobileNumber = profile.MobileNumber;
    }

    public PlaceOrderRequest ToRequest()
    {
        return new PlaceOrderRequest
        {
            CustomerProfileId = CustomerProfileId,
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
            CustomerNotes = CustomerNotes,
            SpecialInstructions = SpecialInstructions,
            Items = Items.Select(x => new OrderItemRequestDto
            {
                ItemType = x.IsMealItem ? "Meal" : "Package",
                PackageId = x.IsMealItem ? null : x.PackageId,
                PackageSizeId = x.IsMealItem ? null : x.PackageSizeId,
                MealId = x.IsMealItem ? x.MealId : null,
                Quantity = x.Quantity,
                MealSelections = x.IsMealItem
                    ? new List<OrderItemMealSelectionRequestDto>()
                    : x.MealSelections.Select(m => new OrderItemMealSelectionRequestDto
                    {
                        PackageSelectionRuleId = m.PackageSelectionRuleId,
                        MealId = m.MealId
                    }).ToList(),
                AddonSelections = x.IsMealItem
                    ? new List<OrderItemAddonSelectionRequestDto>()
                    : x.AddonSelections.Select(a => new OrderItemAddonSelectionRequestDto
                    {
                        PackageAddonId = a.PackageAddonId
                    }).ToList()
            }).ToList()
        };
    }
}