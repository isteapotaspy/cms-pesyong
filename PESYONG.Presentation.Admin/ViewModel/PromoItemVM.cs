using System;
using CMS.Contracts.Admin.Promos;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PromoItemVM : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    private int id;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateCreatedText))]
    private DateTime? dateCreated;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateUpdatedText))]
    private DateTime? dateUpdated;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    private string code = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DiscountText))]
    private decimal discountPercentageValue;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MinimumOrderText))]
    private decimal? minimumOrderAmount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UsageText))]
    private int? usageLimit;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(UsageText))]
    private int usedCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValidityText))]
    private DateTime? validFromUtc = DateTime.UtcNow.Date;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ValidityText))]
    private DateTime? validUntilUtc = DateTime.UtcNow.Date.AddDays(30);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActiveStatusText))]
    private bool isActive;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(Code)
            ? Id == 0 ? "(New Promo)" : $"Promo #{Id}"
            : Code;

    public string DateCreatedText =>
        DateCreated.HasValue ? DateCreated.Value.ToString("g") : "-";

    public string DateUpdatedText =>
        DateUpdated.HasValue ? DateUpdated.Value.ToString("g") : "-";

    public string DiscountText =>
        $"{DiscountPercentageValue:0.##}%";

    public string MinimumOrderText =>
        MinimumOrderAmount.HasValue
            ? $"₱{MinimumOrderAmount.Value:N2}"
            : "No minimum";

    public string UsageText =>
        UsageLimit.HasValue
            ? $"{UsedCount:N0}/{UsageLimit.Value:N0}"
            : $"{UsedCount:N0}/No limit";

    public string ValidityText
    {
        get
        {
            var fromText = ValidFromUtc.HasValue
                ? ValidFromUtc.Value.ToString("yyyy-MM-dd")
                : "-";

            var untilText = ValidUntilUtc.HasValue
                ? ValidUntilUtc.Value.ToString("yyyy-MM-dd")
                : "-";

            return $"{fromText} to {untilText}";
        }
    }

    public string ActiveStatusText =>
        IsActive ? "This promo is active." : "This promo is inactive.";

    public static PromoItemVM FromDto(PromoDto dto)
    {
        return new PromoItemVM
        {
            Id = dto.Id,
            DateCreated = dto.DateCreated,
            DateUpdated = dto.DateUpdated,
            Code = dto.Code,
            Description = dto.Description,
            DiscountPercentageValue = dto.DiscountPercentageValue,
            MinimumOrderAmount = dto.MinimumOrderAmount,
            UsageLimit = dto.UsageLimit,
            UsedCount = dto.UsedCount,
            ValidFromUtc = dto.ValidFromUtc,
            ValidUntilUtc = dto.ValidUntilUtc,
            IsActive = dto.IsActive
        };
    }

    public void CopyFrom(PromoDto dto)
    {
        Id = dto.Id;
        DateCreated = dto.DateCreated;
        DateUpdated = dto.DateUpdated;
        Code = dto.Code;
        Description = dto.Description;
        DiscountPercentageValue = dto.DiscountPercentageValue;
        MinimumOrderAmount = dto.MinimumOrderAmount;
        UsageLimit = dto.UsageLimit;
        UsedCount = dto.UsedCount;
        ValidFromUtc = dto.ValidFromUtc;
        ValidUntilUtc = dto.ValidUntilUtc;
        IsActive = dto.IsActive;
    }

    public CreatePromoRequest ToCreateRequest()
    {
        return new CreatePromoRequest
        {
            Code = Code.Trim(),
            Description = Description.Trim(),
            DiscountPercentageValue = DiscountPercentageValue,
            MinimumOrderAmount = MinimumOrderAmount,
            UsageLimit = UsageLimit,
            UsedCount = UsedCount,
            ValidFromUtc = ValidFromUtc ?? DateTime.UtcNow.Date,
            ValidUntilUtc = ValidUntilUtc ?? DateTime.UtcNow.Date.AddDays(30)
        };
    }

    public UpdatePromoRequest ToUpdateRequest()
    {
        return new UpdatePromoRequest
        {
            Code = Code.Trim(),
            Description = Description.Trim(),
            DiscountPercentageValue = DiscountPercentageValue,
            MinimumOrderAmount = MinimumOrderAmount,
            UsageLimit = UsageLimit,
            UsedCount = UsedCount,
            ValidFromUtc = ValidFromUtc ?? DateTime.UtcNow.Date,
            ValidUntilUtc = ValidUntilUtc ?? DateTime.UtcNow.Date.AddDays(30)
        };
    }
}