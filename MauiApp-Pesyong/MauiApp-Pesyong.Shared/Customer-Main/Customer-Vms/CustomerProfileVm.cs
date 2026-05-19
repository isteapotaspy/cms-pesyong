using CMS.Contracts.Customer.Auth;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class CustomerProfileVm
{
    public int AppUserId { get; set; }
    public int CustomerProfileId { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;

    public static CustomerProfileVm FromResponse(CustomerMeResponse response)
    {
        return new CustomerProfileVm
        {
            AppUserId = response.AppUserId,
            CustomerProfileId = response.CustomerProfileId,
            UserName = response.UserName,
            Email = response.Email,
            FirstName = response.FirstName,
            LastName = response.LastName,
            FullName = response.FullName,
            MobileNumber = response.MobileNumber
        };
    }
}