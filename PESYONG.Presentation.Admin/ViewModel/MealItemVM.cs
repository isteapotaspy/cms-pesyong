using System.Xml.Linq;
using CMS.Contracts.Admin.Meals;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModels.Meals;

public partial class MealItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int categoryId;

    [ObservableProperty]
    private string categoryName = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string mealType = string.Empty;

    [ObservableProperty]
    private decimal basePrice;

    [ObservableProperty]
    private int stockQuantity;

    [ObservableProperty]
    private int minOrderQuantity = 1;

    [ObservableProperty]
    private string imageUrl = string.Empty;

    [ObservableProperty]
    private bool isAvailable = true;

    [ObservableProperty]
    private bool isViandOption;

    public static MealItemVM FromDto(MealDto dto)
    {
        return new MealItemVM
        {
            Id = dto.Id,
            CategoryId = dto.CategoryId,
            CategoryName = dto.CategoryName,
            Name = dto.Name,
            Description = dto.Description,
            MealType = dto.MealType,
            BasePrice = dto.BasePrice,
            StockQuantity = dto.StockQuantity,
            MinOrderQuantity = dto.MinOrderQuantity,
            ImageUrl = dto.ImageUrl,
            IsAvailable = dto.IsAvailable,
            IsViandOption = dto.IsViandOption
        };
    }

    public void CopyFrom(MealDto dto)
    {
        Id = dto.Id;
        CategoryId = dto.CategoryId;
        CategoryName = dto.CategoryName;
        Name = dto.Name;
        Description = dto.Description;
        MealType = dto.MealType;
        BasePrice = dto.BasePrice;
        StockQuantity = dto.StockQuantity;
        MinOrderQuantity = dto.MinOrderQuantity;
        ImageUrl = dto.ImageUrl;
        IsAvailable = dto.IsAvailable;
        IsViandOption = dto.IsViandOption;
    }

    public CreateMealRequest ToCreateRequest()
    {
        return new CreateMealRequest
        {
            CategoryId = CategoryId,
            Name = Name,
            Description = Description,
            MealType = MealType,
            BasePrice = BasePrice,
            StockQuantity = StockQuantity,
            MinOrderQuantity = MinOrderQuantity,
            ImageUrl = ImageUrl,
            IsAvailable = IsAvailable,
            IsViandOption = IsViandOption
        };
    }

    public UpdateMealRequest ToUpdateRequest()
    {
        return new UpdateMealRequest
        {
            CategoryId = CategoryId,
            Name = Name,
            Description = Description,
            MealType = MealType,
            BasePrice = BasePrice,
            StockQuantity = StockQuantity,
            MinOrderQuantity = MinOrderQuantity,
            ImageUrl = ImageUrl,
            IsAvailable = IsAvailable,
            IsViandOption = IsViandOption
        };
    }
}