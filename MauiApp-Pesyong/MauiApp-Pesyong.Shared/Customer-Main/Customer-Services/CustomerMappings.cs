using CMS.Contracts.Customer.Meals;
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
            Id = dto.Id.ToString(),
            PackageId = dto.Id,
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
            IsAvailable = dto.IsAvailable,
            IsCustomizable = dto.IsCustomizable,

            Sizes = dto.Sizes.Select(x => new PackageSizeUiModel
            {
                Id = x.Id,
                Label = x.Label,
                Subtitle = x.Subtitle,
                PaxCount = x.PaxCount,
                Price = x.Price,
                SelectionRules = x.SelectionRules.Select(r => new PackageSelectionRuleUiModel
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    SelectionType = r.SelectionType,
                    AllowedMealType = r.AllowedMealType,
                    MinSelections = r.MinSelections,
                    MaxSelections = r.MaxSelections,
                    IsRequired = r.IsRequired,
                    DisplayOrder = r.DisplayOrder,
                    Options = r.Options.Select(o => new PackageSelectionOptionUiModel
                    {
                        Id = o.Id,
                        MealId = o.MealId,
                        AdditionalPrice = o.AdditionalPrice,
                        IsDefault = o.IsDefault,
                        Meal = new MealOptionUiModel
                        {
                            Id = o.Meal.Id,
                            Name = o.Meal.Name,
                            Description = o.Meal.Description,
                            MealType = o.Meal.MealType,
                            BasePrice = o.Meal.BasePrice,
                            AdditionalPrice = o.Meal.AdditionalPrice,
                            ImageClass = string.IsNullOrWhiteSpace(o.Meal.ImageUrl) ? "food-image-one" : o.Meal.ImageUrl,
                            IsDefault = o.Meal.IsDefault
                        }
                    }).ToList()
                }).ToList()
            }).ToList(),

            Addons = dto.Addons.Select(x => new AddonUiModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                IsAvailable = x.IsAvailable
            }).ToList()
        };
    }

    public static TrackingVm ToVm(this GetOrderTrackingResponse dto)
    {
        return new TrackingVm
        {
            OrderId = dto.OrderId.ToString(),
            OrderNumber = dto.OrderNumber,
            Status = dto.Status,
            OrderedAt = dto.OrderedAtUtc.ToLocalTime(),
            EstimatedDeliveryTime = dto.EstimatedDeliveryTimeUtc.ToLocalTime(),
            AmountPaid = dto.AmountPaid,
            Rider = dto.Rider is null
                ? new RiderVm()
                : new RiderVm
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
                Timestamp = x.TimestampUtc?.ToLocalTime(),
                State = x.State
            }).ToList(),
            Items = dto.Items.Select((x, i) => new TrackingSelectionVm
            {
                Name = x.PackageTitle,
                Subtitle = string.IsNullOrWhiteSpace(x.SizeLabel)
                    ? $"Qty {x.Quantity}"
                    : $"{x.SizeLabel} • Qty {x.Quantity}",
                Amount = x.LineTotal,
                ImageClass = i % 2 == 0 ? "food-image-one" : "food-image-three"
            }).ToList()
        };
    }

    public static ShortOrderMealUiModel ToUiModel(this CustomerMealDto dto)
    {
        return new ShortOrderMealUiModel
        {
            Id = dto.Id,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            CategorySlug = dto.CategorySlug,
            Name = dto.Name,
            Description = dto.Description,
            MealType = dto.MealType,
            BasePrice = dto.BasePrice,
            MinOrderQuantity = dto.MinOrderQuantity,
            ImageClass = string.IsNullOrWhiteSpace(dto.ImageUrl) ? "food-image-one" : dto.ImageUrl,
            IsAvailable = dto.IsAvailable
        };
    }
}