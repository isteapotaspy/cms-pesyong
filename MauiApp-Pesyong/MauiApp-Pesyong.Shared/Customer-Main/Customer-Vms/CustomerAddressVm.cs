using CMS.Contracts.Customer.Profile;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class CustomerAddressVm
{
    public int Id { get; set; }
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Barangay { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsDefault { get; set; }

    public string Summary =>
        string.Join(", ",
            new[]
            {
                StreetAddress,
                Barangay,
                City
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

    public static CustomerAddressVm FromDto(CustomerAddressDto dto)
    {
        return new CustomerAddressVm
        {
            Id = dto.Id,
            StreetAddress = dto.StreetAddress,
            City = dto.City,
            Barangay = dto.Barangay,
            Landmark = dto.Landmark,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            IsDefault = dto.IsDefault
        };
    }

    public SaveCustomerAddressRequest ToRequest()
    {
        return new SaveCustomerAddressRequest
        {
            StreetAddress = StreetAddress.Trim(),
            City = City.Trim(),
            Barangay = Barangay.Trim(),
            Landmark = Landmark.Trim(),
            Latitude = Latitude,
            Longitude = Longitude,
            IsDefault = IsDefault
        };
    }

    public static CustomerAddressVm Empty()
    {
        return new CustomerAddressVm
        {
            StreetAddress = string.Empty,
            City = string.Empty,
            Barangay = string.Empty,
            Landmark = string.Empty
        };
    }
}