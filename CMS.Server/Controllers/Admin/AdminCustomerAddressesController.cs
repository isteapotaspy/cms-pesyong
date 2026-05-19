using CMS.Contracts.Admin.Address;
using CMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Controllers.Admin;

[ApiController]
[Route("api/admin/customer-addresses")]
public sealed class AdminCustomerAddressesController : ControllerBase
{
    private readonly CmsDbContext _db;

    public AdminCustomerAddressesController(CmsDbContext db)
    {
        _db = db;
    }

    [HttpGet("customer/{customerProfileId:int}/address/{addressId:int}")]
    public async Task<ActionResult<CustomerAddressStringDto>> GetCustomerAddress(
        int customerProfileId,
        int addressId,
        CancellationToken cancellationToken)
    {
        var customerAddress = await GetCustomerAddressStringDtoAsync(
            customerProfileId,
            addressId,
            cancellationToken);

        if (customerAddress is null)
        {
            return NotFound();
        }

        return Ok(customerAddress);
    }

    private async Task<CustomerAddressStringDto?> GetCustomerAddressStringDtoAsync(
        int customerProfileId,
        int addressId,
        CancellationToken cancellationToken)
    {
        var address = await _db.Addresses
            .AsNoTracking()
            .Where(x =>
                x.Id == addressId &&
                x.CustomerProfileId == customerProfileId)
            .Select(x => new
            {
                x.Id,
                x.CustomerProfileId,
                x.StreetAddress,
                x.Barangay,
                x.City,
                x.Landmark,
                x.Latitude,
                x.Longitude,
                x.IsDefault
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (address is null)
        {
            return null;
        }

        return new CustomerAddressStringDto
        {
            CustomerProfileId = address.CustomerProfileId,
            AddressId = address.Id,

            CustomerDetails = BuildCustomerDetails(address.CustomerProfileId),

            AddressDetails = BuildAddressDetails(
                address.Id,
                address.StreetAddress,
                address.Barangay,
                address.City,
                address.Landmark,
                address.Latitude,
                address.Longitude,
                address.IsDefault)
        };
    }

    private static string BuildCustomerDetails(int customerProfileId)
    {
        return $"Customer #{customerProfileId}";
    }

    private static string BuildAddressDetails(
        int addressId,
        string streetAddress,
        string barangay,
        string city,
        string landmark,
        decimal? latitude,
        decimal? longitude,
        bool isDefault)
    {
        var addressParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(streetAddress))
        {
            addressParts.Add(streetAddress.Trim());
        }

        if (!string.IsNullOrWhiteSpace(barangay))
        {
            addressParts.Add(barangay.Trim());
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            addressParts.Add(city.Trim());
        }

        var fullAddress = addressParts.Count > 0
            ? string.Join(", ", addressParts)
            : "No address details";

        var parts = new List<string>
        {
            $"Address #{addressId}",
            fullAddress
        };

        if (!string.IsNullOrWhiteSpace(landmark))
        {
            parts.Add($"Landmark: {landmark.Trim()}");
        }

        if (latitude.HasValue && longitude.HasValue)
        {
            parts.Add($"Coordinates: {latitude}, {longitude}");
        }

        if (isDefault)
        {
            parts.Add("Default");
        }

        return string.Join(" | ", parts);
    }
}