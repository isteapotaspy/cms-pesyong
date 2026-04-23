using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

public class AdminDataService
{
    public List<MealVm> GetMeals() => new()
    {
        new MealVm { MealId = "4", Title = "Puto", Description = "Soft and fluffy steamed rice cake", Price = 60, DeliveryType = "Delivery", StockQuantity = 50, MinOrder = 6 },
        new MealVm { MealId = "5", Title = "Kutsinta", Description = "Sticky brown rice cake", Price = 50, DeliveryType = "Delivery", StockQuantity = 30, MinOrder = 6 },
        new MealVm { MealId = "6", Title = "Pancit Canton", Description = "Stir-fried noodles with vegetables", Price = 120, DeliveryType = "Delivery", StockQuantity = 25, MinOrder = 3 },
        new MealVm { MealId = "7", Title = "Lumpia Shanghai", Description = "Crispy spring rolls with pork filling", Price = 80, DeliveryType = "Pickup", StockQuantity = 40, MinOrder = 10 },
        new MealVm { MealId = "8", Title = "Adobo", Description = "Classic Filipino braised pork and chicken", Price = 150, DeliveryType = "Same Day", StockQuantity = 20, MinOrder = 5 }
    };

    public List<PackageVm> GetPackages() => new()
    {
        new PackageVm
        {
            PackageId = "9", OwnerId = "OWN-001", PromoId = "PRO-001", Pax = 20,
            IsCateringPackage = true, IsAvailable = true, CustomizablePackage = false,
            Name = "Package 6-7 Viands", Description = "Perfect for reunions", Notes = "Good for 20 pax", Price = 200,
            IncludedMeals = new List<PackageMealVm>
            {
                new PackageMealVm { MealName = "Kutsinta", Price = 50, Quantity = 1, SpecialRequest = "Special request" },
                new PackageMealVm { MealName = "Puto", Price = 60, Quantity = 1, SpecialRequest = "Special request" }
            }
        },
        new PackageVm
        {
            PackageId = "10", OwnerId = "OWN-001", PromoId = "", Pax = 50,
            IsCateringPackage = true, IsAvailable = true, CustomizablePackage = true,
            Name = "Premium Fiesta Package", Description = "Complete fiesta setup", Notes = "Good for 50 pax", Price = 5000,
            IncludedMeals = new List<PackageMealVm>
            {
                new PackageMealVm { MealName = "Adobo", Price = 150, Quantity = 5 },
                new PackageMealVm { MealName = "Lumpia Shanghai", Price = 80, Quantity = 10 },
                new PackageMealVm { MealName = "Pancit Canton", Price = 120, Quantity = 5 }
            }
        }
    };

    public List<DeliveryVm> GetDeliveries() => new()
    {
        new DeliveryVm
        {
            DeliveryId = "DLV-001", OrderId = "ORD-001", DeliveryPersonnelId = "RIDER-001",
            Status = "Pending", DeliveryAddress = "123 Main St", TrackingNumber = "TRK-001",
            ShippingCost = 70, ShippingMethod = "Motorcycle", CarrierName = "Rider A",
            CreatedDate = DateTime.Now, EstimatedDelivery = DateTime.Today.AddDays(1),
            ActualDelivery = DateTime.Today, SignatureRequired = true, ReceivedBy = "",
            ReceivedAt = DateTime.Today, CurrentLocation = "Kitchen Hub",
            LastLocationUpdate = DateTime.Today, SpecialInstructions = "Call before arrival", DeliveryNotes = ""
        },
        new DeliveryVm
        {
            DeliveryId = "DLV-002", OrderId = "ORD-003", DeliveryPersonnelId = "RIDER-002",
            Status = "Delivered", DeliveryAddress = "456 Oak Avenue, Brgy. San Jose",
            TrackingNumber = "TRK-002", ShippingCost = 120, ShippingMethod = "Van",
            CarrierName = "Rider B", CreatedDate = DateTime.Now.AddDays(-2),
            EstimatedDelivery = DateTime.Today.AddDays(-1), ActualDelivery = DateTime.Today.AddDays(-1),
            SignatureRequired = false, ReceivedBy = "Maria Santos", ReceivedAt = DateTime.Today.AddDays(-1),
            CurrentLocation = "Delivered", LastLocationUpdate = DateTime.Today.AddDays(-1),
            SpecialInstructions = "Gate code: 1234", DeliveryNotes = "Left with guard"
        }
    };

    public List<OrderVm> GetOrders() => new()
    {
        new OrderVm
        {
            CustomerName = "Jane Meck", DeliveryType = "Pickup", DeliveryStatus = "Delivered",
            Address = "123 Main St", Total = 200, FirstName = "Jane", LastName = "Meck",
            PhoneNumber = "09125327405", Email = "j@gmail.com",
            OrderId = "59d83b85-77ba-43db-9096-9c91bbb47449",
            CustomerId = "d71aae94-e441-496b-a4d2-551823c86fc1", ReceiptId = "RCT-001",
            OrderDate = new DateTime(2026, 3, 12), EstimatedDeliveryDate = new DateTime(2026, 3, 15),
            ActualDeliveryDate = new DateTime(2026, 3, 15), TrackingNumber = "TRK-001",
            OrderTotalText = "PHP 200.00", CustomerNotes = ""
        },
        new OrderVm
        {
            CustomerName = "Carlos Reyes", DeliveryType = "Delivery", DeliveryStatus = "Pending",
            Address = "456 Oak Avenue", Total = 500, FirstName = "Carlos", LastName = "Reyes",
            PhoneNumber = "09187654321", Email = "carlos@email.com",
            OrderId = "ORD-" + Guid.NewGuid().ToString().Substring(0, 8),
            CustomerId = "CUST-002", ReceiptId = "", OrderDate = DateTime.Today,
            EstimatedDeliveryDate = DateTime.Today.AddDays(2), TrackingNumber = "",
            OrderTotalText = "PHP 500.00", CustomerNotes = "Extra rice please"
        },
        new OrderVm
        {
            CustomerName = "Maria Santos", DeliveryType = "Delivery", DeliveryStatus = "Delivered",
            Address = "789 Pine Street, Brgy. Poblacion", Total = 1200, FirstName = "Maria", LastName = "Santos",
            PhoneNumber = "09223456789", Email = "maria.santos@email.com",
            OrderId = "ORD-" + Guid.NewGuid().ToString().Substring(0, 8),
            CustomerId = "CUST-003", ReceiptId = "RCT-003", OrderDate = DateTime.Today.AddDays(-5),
            EstimatedDeliveryDate = DateTime.Today.AddDays(-3), ActualDeliveryDate = DateTime.Today.AddDays(-3),
            TrackingNumber = "TRK-003", OrderTotalText = "PHP 1,200.00", CustomerNotes = ""
        }
    };

    public List<CustomerVm> GetCustomers() => new()
    {
        new CustomerVm { CustomerId = "CUST-001", FirstName = "Jane", LastName = "Meck", Email = "j@gmail.com", Phone = "09125327405", Address = "123 Main St", Notes = "VIP customer", CreatedDate = new DateTime(2026, 1, 15) },
        new CustomerVm { CustomerId = "CUST-002", FirstName = "Carlos", LastName = "Reyes", Email = "carlos@email.com", Phone = "09187654321", Address = "456 Oak Avenue", Notes = "Prefers delivery", CreatedDate = new DateTime(2026, 2, 3) },
        new CustomerVm { CustomerId = "CUST-003", FirstName = "Maria", LastName = "Santos", Email = "maria.santos@email.com", Phone = "09223456789", Address = "789 Pine Street, Brgy. Poblacion", Notes = "", CreatedDate = new DateTime(2026, 2, 20) },
        new CustomerVm { CustomerId = "CUST-004", FirstName = "Juan", LastName = "Dela Cruz", Email = "juan.dc@email.com", Phone = "09334567890", Address = "321 Rizal Ave", Notes = "Corporate account", CreatedDate = new DateTime(2026, 3, 1) }
    };

    public List<AppUserVm> GetAppUsers() => new()
    {
        new AppUserVm { UserId = "USR-001", Username = "admin", DisplayName = "System Admin", Email = "admin@pesyong.com", Role = "Admin", IsActive = true, LastLogin = DateTime.Now.AddHours(-1) },
        new AppUserVm { UserId = "USR-002", Username = "manager1", DisplayName = "Ana Manager", Email = "ana@pesyong.com", Role = "Manager", IsActive = true, LastLogin = DateTime.Now.AddDays(-1) },
        new AppUserVm { UserId = "USR-003", Username = "staff.kitchen", DisplayName = "Kitchen Staff", Email = "kitchen@pesyong.com", Role = "Staff", IsActive = true, LastLogin = DateTime.Now.AddHours(-3) },
        new AppUserVm { UserId = "USR-004", Username = "rider.a", DisplayName = "Rider A", Email = "rider.a@pesyong.com", Role = "Staff", IsActive = false, LastLogin = DateTime.Now.AddDays(-30) }
    };

    public List<SystemLogVm> GetSystemLogs() => new()
    {
        new SystemLogVm { LogId = "LOG-001", Timestamp = DateTime.Now.AddMinutes(-5), Level = "Info", Source = "OrderService", Message = "Order ORD-001 created successfully" },
        new SystemLogVm { LogId = "LOG-002", Timestamp = DateTime.Now.AddMinutes(-12), Level = "Info", Source = "DeliveryService", Message = "Delivery DLV-001 assigned to RIDER-001" },
        new SystemLogVm { LogId = "LOG-003", Timestamp = DateTime.Now.AddMinutes(-30), Level = "Warning", Source = "PaymentService", Message = "Payment timeout for order ORD-002, retrying..." },
        new SystemLogVm { LogId = "LOG-004", Timestamp = DateTime.Now.AddHours(-1), Level = "Error", Source = "AuthService", Message = "Failed login attempt for user 'unknown_user' from 192.168.1.100" },
        new SystemLogVm { LogId = "LOG-005", Timestamp = DateTime.Now.AddHours(-2), Level = "Info", Source = "MealService", Message = "Meal catalog refreshed, 5 items loaded" },
        new SystemLogVm { LogId = "LOG-006", Timestamp = DateTime.Now.AddHours(-3), Level = "Warning", Source = "InventoryService", Message = "Stock low for Puto (qty: 5)" },
        new SystemLogVm { LogId = "LOG-007", Timestamp = DateTime.Now.AddHours(-4), Level = "Info", Source = "UserService", Message = "User 'manager1' logged in successfully" },
        new SystemLogVm { LogId = "LOG-008", Timestamp = DateTime.Now.AddHours(-5), Level = "Error", Source = "DeliveryService", Message = "GPS tracking unavailable for DLV-002" }
    };
}
