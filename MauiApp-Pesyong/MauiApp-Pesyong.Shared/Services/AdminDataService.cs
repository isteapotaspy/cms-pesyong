namespace MauiApp_Pesyong.Shared.Services;

public class AdminDataService
{
    public List<MealVm> GetMeals() => new()
    {
        new MealVm
        {
            MealId = "4",
            Title = "Puto",
            Description = "Soft and fluffy steamed rice cake",
            Price = 60,
            DeliveryType = "Delivery",
            StockQuantity = 50,
            MinOrder = 6
        },
        new MealVm
        {
            MealId = "5",
            Title = "Kutsinta",
            Description = "Sticky brown rice cake",
            Price = 50,
            DeliveryType = "Delivery",
            StockQuantity = 30,
            MinOrder = 6
        }
    };

    public List<PackageVm> GetPackages() => new()
    {
        new PackageVm
        {
            PackageId = "9",
            OwnerId = "OWN-001",
            PromoId = "PRO-001",
            Pax = 20,
            IsCateringPackage = true,
            IsAvailable = true,
            CustomizablePackage = false,
            Name = "Package 6-7 Viands",
            Description = "Perfect for reunions",
            Notes = "Good for 20 pax",
            Price = 200,
            IncludedMeals = new List<PackageMealVm>
            {
                new PackageMealVm
                {
                    MealName = "Kutsinta",
                    Price = 50,
                    Quantity = 1,
                    SpecialRequest = "Special request"
                },
                new PackageMealVm
                {
                    MealName = "Puto",
                    Price = 60,
                    Quantity = 1,
                    SpecialRequest = "Special request"
                }
            }
        }
    };

    public List<DeliveryVm> GetDeliveries() => new()
    {
        new DeliveryVm
        {
            DeliveryId = "DLV-001",
            OrderId = "ORD-001",
            DeliveryPersonnelId = "RIDER-001",
            Status = "Pending",
            DeliveryAddress = "123 Main St",
            TrackingNumber = "TRK-001",
            ShippingCost = 70,
            ShippingMethod = "Motorcycle",
            CarrierName = "Rider A",
            CreatedDate = DateTime.Now,
            EstimatedDelivery = DateTime.Today.AddDays(1),
            ActualDelivery = DateTime.Today,
            SignatureRequired = true,
            ReceivedBy = "",
            ReceivedAt = DateTime.Today,
            CurrentLocation = "Kitchen Hub",
            LastLocationUpdate = DateTime.Today,
            SpecialInstructions = "Call before arrival",
            DeliveryNotes = ""
        }
    };

    public List<OrderVm> GetOrders() => new()
    {
        new OrderVm
        {
            CustomerName = "Jane Meck - 59d83b85",
            DeliveryType = "Pickup",
            DeliveryStatus = "Delivered",
            Address = "123 Main St",
            Total = 200,
            FirstName = "Jane",
            LastName = "Meck",
            PhoneNumber = "09125327405",
            Email = "j@gmail.com",
            OrderId = "59d83b85-77ba-43db-9096-9c91bbb47449",
            CustomerId = "d71aae94-e441-496b-a4d2-551823c86fc1",
            ReceiptId = "RCT-001",
            OrderDate = new DateTime(2026, 3, 12),
            EstimatedDeliveryDate = new DateTime(2026, 3, 15),
            ActualDeliveryDate = new DateTime(2026, 3, 15),
            TrackingNumber = "TRK-001",
            OrderTotalText = "PHP 200.00",
            CustomerNotes = ""
        }
    };
}

public class MealVm
{
    public string MealId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string DeliveryType { get; set; } = "Delivery";
    public int StockQuantity { get; set; }
    public int MinOrder { get; set; }
}

public class PackageVm
{
    public string PackageId { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public string PromoId { get; set; } = "";
    public int Pax { get; set; }
    public bool IsCateringPackage { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public bool CustomizablePackage { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Notes { get; set; } = "";
    public decimal Price { get; set; }
    public List<PackageMealVm> IncludedMeals { get; set; } = new();
}

public class PackageMealVm
{
    public string MealName { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string SpecialRequest { get; set; } = "";
}

public class DeliveryVm
{
    public string DeliveryId { get; set; } = "";
    public string OrderId { get; set; } = "";
    public string DeliveryPersonnelId { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public string DeliveryAddress { get; set; } = "";
    public string TrackingNumber { get; set; } = "";
    public decimal ShippingCost { get; set; }
    public string ShippingMethod { get; set; } = "";
    public string CarrierName { get; set; } = "";
    public DateTime CreatedDate { get; set; }
    public DateTime EstimatedDelivery { get; set; }
    public DateTime ActualDelivery { get; set; }
    public bool SignatureRequired { get; set; } = true;
    public string ReceivedBy { get; set; } = "";
    public DateTime ReceivedAt { get; set; }
    public string CurrentLocation { get; set; } = "";
    public DateTime LastLocationUpdate { get; set; }
    public string SpecialInstructions { get; set; } = "";
    public string DeliveryNotes { get; set; } = "";
}

public class OrderVm
{
    public string CustomerName { get; set; } = "";
    public string DeliveryType { get; set; } = "";
    public string DeliveryStatus { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal Total { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Email { get; set; } = "";
    public string OrderId { get; set; } = "";
    public string CustomerId { get; set; } = "";
    public string ReceiptId { get; set; } = "";
    public DateTime OrderDate { get; set; } = DateTime.Today;
    public DateTime EstimatedDeliveryDate { get; set; } = DateTime.Today;
    public DateTime ActualDeliveryDate { get; set; } = DateTime.Today;
    public string TrackingNumber { get; set; } = "";
    public string OrderTotalText { get; set; } = "PHP 0.00";
    public string CustomerNotes { get; set; } = "";
}