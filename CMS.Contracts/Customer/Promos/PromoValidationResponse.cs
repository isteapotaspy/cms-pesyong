
namespace CMS.Contracts.Customer.Promos;
public class PromoValidationResponse
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal DiscountPercentageValue { get; set; }
    public decimal DiscountAmount { get; set; }

    public decimal SubTotal { get; set; }
    public decimal NewGrandTotal { get; set; }
}