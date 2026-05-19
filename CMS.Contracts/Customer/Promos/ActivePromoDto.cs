namespace CMS.Contracts.Customer.Promos;

public class ActivePromoDto
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal DiscountPercentageValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }

    public DateTime ValidFromUtc { get; set; }
    public DateTime ValidUntilUtc { get; set; }
}