using CMS.Contracts.Customer.Menu;
using CMS.Contracts.Customer.Orders;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public static class CustomerMappings
{
    public static PackageUiModel ToUiModel(this MenuPackageDto dto)
    {
        return new PackageUiModel
        {
            Id = dto.Id,
            Title = dto.Title,
            MenuCategory = dto.CategoryName,
            CategoryLabel = dto.CategoryName,
            Description = dto.Description,
            CardSummary = dto.CardSummary,
            Badge = dto.Badge,
            Notice = dto.Notice,
            ServesLabel = dto.ServesLabel,
            InclusionText = dto.InclusionText,
            ImageClass = string.IsNullOrWhiteSpace(dto.ImageUrl) ? "food-image-one" : dto.ImageUrl,
            Rating = dto.Rating,
            ReviewCount = dto.ReviewCount,
            Sizes = dto.Sizes.Select(x => new PackageSizeUiModel
            {
                Label = x.Label,
                Subtitle = x.Subtitle,
                Price = x.Price
            }).ToList(),
            Addons = dto.Addons.Select(x => new AddonUiModel
            {
                Name = x.Name,
                Description = x.Description,
                Price = x.Price
            }).ToList()
        };
    }

    public static TrackingVm ToVm(this GetOrderTrackingResponse dto)
    {
        return new TrackingVm
        {
            OrderId = dto.OrderId,
            OrderNumber = dto.OrderNumber,
            Status = dto.Status,
            OrderedAt = dto.OrderedAt,
            EstimatedDeliveryTime = dto.EstimatedDeliveryTime,
            AmountPaid = dto.AmountPaid,
            Rider = new RiderVm
            {
                Name = dto.Rider.Name,
                Vehicle = dto.Rider.Vehicle,
                PlateNumber = dto.Rider.PlateNumber,
                Rating = dto.Rider.Rating,
                TotalTrips = dto.Rider.TotalTrips,
                ContactNumber = dto.Rider.ContactNumber
            },
            DeliveryAddress = new DeliveryAddressVm
            {
                StreetAddress = dto.DeliveryAddress.StreetAddress,
                City = dto.DeliveryAddress.City,
                Barangay = dto.DeliveryAddress.Barangay,
                Landmark = dto.DeliveryAddress.Landmark,
                Latitude = dto.DeliveryAddress.Latitude,
                Longitude = dto.DeliveryAddress.Longitude
            },
            Steps = dto.Steps.Select(x => new TrackingStepVm
            {
                Title = x.Title,
                Description = x.Description,
                Timestamp = x.Timestamp,
                State = x.State
            }).ToList(),
            Items = dto.Items.Select((x, i) => new TrackingSelectionVm
            {
                Name = x.Name,
                Subtitle = x.Subtitle,
                Amount = x.Amount,
                ImageClass = i % 2 == 0 ? "food-image-one" : "food-image-three"
            }).ToList()
        };
    }
}