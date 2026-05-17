using System;
using System.Collections.Generic;
using System.Text;
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
    private decimal discountPercentageValue;

    [ObservableProperty]
    private decimal? minimumOrderAmount;

    [ObservableProperty]
    private int? usageLimit;

    [ObservableProperty]
    private int usedCount;

    [ObservableProperty]
    private DateTime? validFromUtc = DateTime.UtcNow.Date;

    [ObservableProperty]
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

    public string ActiveStatusText =>
        IsActive ? "This promo is still active." : "This promo is not active.";

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
