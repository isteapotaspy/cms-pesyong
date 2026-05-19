namespace CMS.Contracts.Customer.Orders;


public class TrackingRiderDto
{
    public string Name { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int TotalTrips { get; set; }
    public string ContactNumber { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public DateTime? LastUpdatedAtUtc { get; set; }
}