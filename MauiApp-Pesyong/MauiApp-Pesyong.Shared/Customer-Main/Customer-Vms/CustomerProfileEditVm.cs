using CMS.Contracts.Customer.Auth;
using CMS.Contracts.Customer.Profile;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class CustomerProfileEditVm
{
    public int AppUserId { get; set; }
    public int CustomerProfileId { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public static CustomerProfileEditVm FromResponse(CustomerMeResponse response)
    {
        return new CustomerProfileEditVm
        {
            AppUserId = response.AppUserId,
            CustomerProfileId = response.CustomerProfileId,
            UserName = response.UserName,
            Email = response.Email,
            FirstName = response.FirstName,
            LastName = response.LastName,
            MobileNumber = response.MobileNumber
        };
    }

    public UpdateCustomerProfileRequest ToRequest()
    {
        return new UpdateCustomerProfileRequest
        {
            FirstName = FirstName.Trim(),
            LastName = LastName.Trim(),
            MobileNumber = MobileNumber.Trim()
        };
    }
}