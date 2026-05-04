
using CMS.Contracts.Customer.Orders;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public class FakeCustomerApiClient : ICustomerCatalogService, ICustomerOrderService
{
    public Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<string> categories = new List<string>
        {
            "All Menu",
            "Catering",
            "Short Orders",
            "Authentic Kakanin"
        };

        return Task.FromResult(categories);
    }

    public Task<IReadOnlyList<PackageUiModel>> GetPackagesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PackageUiModel> packages = new List<PackageUiModel>
        {
            new()
            {
                Id = "pkg-1",
                Title = "Grand Fiesta Package A",
                MenuCategory = "Catering",
                CategoryLabel = "Catering Packages",
                Description = "Complete feast with whole lechon, beef caldereta, pancit bihon, and dessert.",
                CardSummary = "Complete feast with whole lechon, beef caldereta, pancit bihon, and dessert.",
                Badge = "Popular",
                Notice = "24-48h Notice",
                ServesLabel = "25-30 Pax",
                InclusionText = "Complete utensils included",
                ImageClass = "food-image-one",
                Rating = 4.8m,
                ReviewCount = 124,
                Sizes = new()
                {
                    new() { Label = "10 Persons", Subtitle = "4 Main Courses", Price = 2800 },
                    new() { Label = "20 Persons", Subtitle = "5 Main Courses", Price = 4500 },
                    new() { Label = "50 Persons", Subtitle = "7 Main Courses", Price = 9800 }
                },
                Addons = new()
                {
                    new() { Name = "Extra Lechon Sauce", Description = "500ml of our signature liver sauce", Price = 150 },
                    new() { Name = "Premium Dessert Tray", Description = "Add assorted Filipino sweets", Price = 350 }
                }
            },
            new()
            {
                Id = "pkg-2",
                Title = "Seafood Boodle Fight",
                MenuCategory = "Catering",
                CategoryLabel = "Catering Packages",
                Description = "Crabs, prawns, mussels, grilled fish, and squid served on fresh banana leaves.",
                CardSummary = "Crabs, prawns, mussels, grilled fish, and squid served on fresh banana leaves.",
                Badge = "Top Rated",
                Notice = "Best Seller",
                ServesLabel = "10-15 Pax",
                InclusionText = "Complete utensils included",
                ImageClass = "food-image-two",
                Rating = 4.9m,
                ReviewCount = 98,
                Sizes = new()
                {
                    new() { Label = "10 Persons", Subtitle = "Seafood Feast", Price = 8900 },
                    new() { Label = "20 Persons", Subtitle = "Expanded Spread", Price = 14500 }
                },
                Addons = new()
                {
                    new() { Name = "Garlic Butter Shrimp Tray", Description = "Extra tray for seafood lovers", Price = 550 }
                }
            }
        };

        return Task.FromResult(packages);
    }

    public async Task<PackageUiModel?> GetPackageByIdAsync(string packageId, CancellationToken cancellationToken = default)
    {
        var packages = await GetPackagesAsync(cancellationToken);
        return packages.FirstOrDefault(x => x.Id == packageId);
    }

    public Task<PlaceOrderResponse> PlaceOrderAsync(CheckoutVm vm, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new PlaceOrderResponse
        {
            OrderId = Guid.NewGuid().ToString("N"),
            OrderNumber = "PS-82910",
            Status = "Out for Delivery",
            OrderedAt = DateTime.Now,
            EstimatedDeliveryTime = DateTime.Now.AddMinutes(45),
            AmountPaid = vm.GrandTotal
        });
    }

    public Task<TrackingVm?> GetTrackingAsync(string orderId, CancellationToken cancellationToken = default)
    {
        var vm = new TrackingVm
        {
            OrderId = orderId,
            OrderNumber = "PS-82910",
            Status = "Out for Delivery",
            OrderedAt = DateTime.Now.AddMinutes(-30),
            EstimatedDeliveryTime = DateTime.Now.AddMinutes(15),
            AmountPaid = 5400m,
            Rider = new RiderVm
            {
                Name = "Juan Dela Cruz",
                Vehicle = "Honda Click 125",
                PlateNumber = "NDL 2024",
                Rating = 4.9m,
                TotalTrips = 2406,
                ContactNumber = "0917 000 0000"
            },
            DeliveryAddress = new DeliveryAddressVm
            {
                StreetAddress = "Unit 1402, High-rise Towers, North Avenue, Quezon City",
                Landmark = "Please leave at the lobby concierge. Look for Guard Santos."
            },
            Steps = new()
            {
                new() { Title = "Order Confirmed", Description = "We have received your order.", Timestamp = DateTime.Now.AddMinutes(-30), State = TrackingStepState.Done },
                new() { Title = "Preparing Food", Description = "Our chefs are crafting your fiesta package.", Timestamp = DateTime.Now.AddMinutes(-20), State = TrackingStepState.Done },
                new() { Title = "Out for Delivery", Description = "Your rider is heading to your location.", Timestamp = DateTime.Now.AddMinutes(-8), State = TrackingStepState.Active },
                new() { Title = "Arriving Soon", Description = "Preparing to serve you at your doorstep.", Timestamp = DateTime.Now.AddMinutes(15), State = TrackingStepState.Pending }
            },
            Items = new()
            {
                new() { Name = "Fiesta Catering Package A", Subtitle = "20 Servings", Amount = 4500m, ImageClass = "food-image-one" },
                new() { Name = "Special Pork Adobo", Subtitle = "5-6 Servings", Amount = 750m, ImageClass = "food-image-three" }
            }
        };

        return Task.FromResult<TrackingVm?>(vm);
    }
}