using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CMS.Contracts.Admin.Meals;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace PESYONG.Presentation.Admin.ViewModel;

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

    [ObservableProperty]
    private ImageSource? imagePreviewSource;

    [ObservableProperty]
    private byte[]? uploadedImageBytes;

    [ObservableProperty]
    private string uploadedImageFileName = string.Empty;

    public string ImagePlaceholderText =>
        ImagePreviewSource is null
            ? "No image preview available."
            : string.Empty;

    public string ImageStatusText
    {
        get
        {
            if (UploadedImageBytes is { Length: > 0 })
            {
                var sizeInKb = UploadedImageBytes.Length / 1024.0;

                return string.IsNullOrWhiteSpace(UploadedImageFileName)
                    ? $"Image ready for upload ({sizeInKb:N1} KB)."
                    : $"{UploadedImageFileName} is ready for upload ({sizeInKb:N1} KB).";
            }

            if (!string.IsNullOrWhiteSpace(ImageUrl))
            {
                return "Using the current Image URL.";
            }

            return "No image selected yet.";
        }
    }

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

        UploadedImageBytes = null;
        UploadedImageFileName = string.Empty;

        LoadImagePreviewFromUrl(ImageUrl);
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

    [RelayCommand]
    private async Task PickImageAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Meal Image",
            CheckFileExists = true,
            Multiselect = false,
            Filter = "Image files (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp|All files (*.*)|*.*"
        };

        var result = dialog.ShowDialog();

        if (result != true)
        {
            return;
        }

        var bytes = await File.ReadAllBytesAsync(dialog.FileName);

        UploadedImageBytes = bytes;
        UploadedImageFileName = Path.GetFileName(dialog.FileName);

        LoadImagePreviewFromBytes(bytes);
    }

    [RelayCommand]
    private void ClearUploadedImage()
    {
        UploadedImageBytes = null;
        UploadedImageFileName = string.Empty;

        LoadImagePreviewFromUrl(ImageUrl);

        OnPropertyChanged(nameof(ImageStatusText));
    }

    partial void OnImageUrlChanged(string value)
    {
        if (UploadedImageBytes is null || UploadedImageBytes.Length == 0)
        {
            LoadImagePreviewFromUrl(value);
        }

        OnPropertyChanged(nameof(ImageStatusText));
    }

    partial void OnImagePreviewSourceChanged(ImageSource? value)
    {
        OnPropertyChanged(nameof(ImagePlaceholderText));
        OnPropertyChanged(nameof(ImageStatusText));
    }

    partial void OnUploadedImageBytesChanged(byte[]? value)
    {
        OnPropertyChanged(nameof(ImageStatusText));
    }

    partial void OnUploadedImageFileNameChanged(string value)
    {
        OnPropertyChanged(nameof(ImageStatusText));
    }

    private void LoadImagePreviewFromBytes(byte[] bytes)
    {
        if (bytes.Length == 0)
        {
            ImagePreviewSource = null;
            return;
        }

        using var stream = new MemoryStream(bytes);

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = stream;
        bitmap.EndInit();

        if (bitmap.CanFreeze)
        {
            bitmap.Freeze();
        }

        ImagePreviewSource = bitmap;
    }

    private void LoadImagePreviewFromUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ImagePreviewSource = null;
            return;
        }

        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(value, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            if (bitmap.CanFreeze)
            {
                bitmap.Freeze();
            }

            ImagePreviewSource = bitmap;
        }
        catch
        {
            ImagePreviewSource = null;
        }
    }
}